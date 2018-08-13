﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Tsp;
using TspResponder.Http;
using TspResponder.Internal;
using TspException = TspResponder.Internal.TspException;

namespace TspResponder
{
    /// <summary>
    /// Implementation of a TSP responder as defined in RFC 3161
    /// <remarks>https://www.ietf.org/rfc/rfc3161.txt</remarks>
    /// </summary>
    public class TimeStampResponder : ITimeStampResponder
    {
        public async Task<TspHttpResponse> Respond(TspHttpRequest tspHttpRequest)
        {
            try
            {
                var tspReqResult = GetTimeStampRequest(tspHttpRequest);
                if (!tspReqResult.IsValid)
                {
                    return CreateResponse(new TimeStampResp(tspReqResult.PkiStatusInfo, null).GetEncoded());
                }

                var contentInfo = await GetTimeStampToken(tspReqResult.TimeStampRequest);
                return CreateResponse(new TimeStampResp(tspReqResult.PkiStatusInfo, contentInfo).GetEncoded());
            }
            catch (Exception e)
            {
                var pkiStatus = new PkiStatusInfo(
                    (int)PkiStatus.Rejection,
                    new PkiFreeText(new DerUtf8String("An internal error ocurred.")),
                    new PkiFailureInfo(PkiFailureInfo.SystemFailure));

                return CreateResponse(new TimeStampResp(pkiStatus, null).GetEncoded());
            }
        }

        /// <summary>
        /// Gets the <see cref="ContentInfo"/> meaning the time stamp token
        /// </summary>
        /// <param name="timeStampRequest"><see cref="TimeStampRequest"/></param>
        /// <returns><see cref="ContentInfo"/></returns>
        private async Task<ContentInfo> GetTimeStampToken(TimeStampRequest timeStampRequest)
        {
            var tokenGenerator = new TimeStampTokenGenerator(
                await BcTimeStampResponderRepository.GetPrivateKey(),
                await BcTimeStampResponderRepository.GetCertificate(),
                NistObjectIdentifiers.IdSha512.Id,
                BcTimeStampResponderRepository.GetPolicyOid()
                );

            var timeStampToken = tokenGenerator.Generate(
                timeStampRequest,
                BcTimeStampResponderRepository.GetNextSerialNumber(),
                DateTime.UtcNow);

            try
            {
                using (var stream = new Asn1InputStream(timeStampToken.ToCmsSignedData().GetEncoded()))
                {
                    return ContentInfo.GetInstance(stream.ReadObject());
                }
            }
            catch (Exception e)
            {
                throw new TspException("Timestamp token cannot be converted to ContentInfo", e);
            }
        }

        /// <summary>
        /// Retrives the <see cref="TimeStampRequest"/> from the <see cref="TspHttpRequest"/>
        /// </summary>
        /// <param name="tspHttpRequest"><see cref="TspHttpRequest"/></param>
        /// <returns><see cref="TspReqResult"/> containing the <see cref="TimeStampRequest"/> and the <see cref="PkiStatusInfo"/></returns>
        private TspReqResult GetTimeStampRequest(TspHttpRequest tspHttpRequest)
        {
            // Validates the header of the request
            if (tspHttpRequest.MediaType != "application/timestamp-query")
            {
                var pkiStatusInfo = new PkiStatusInfo(
                    (int)PkiStatus.Rejection,
                    new PkiFreeText(new DerUtf8String("Content type is not 'application/timestamp-query'.")),
                    new PkiFailureInfo(PkiFailureInfo.BadRequest));

                return new TspReqResult
                {
                    PkiStatusInfo = pkiStatusInfo
                };
            }

            // Try to create  the TimeStampRequest from the http request
            TimeStampRequest timeStampRequest;
            try
            {
                timeStampRequest = new TimeStampRequest(tspHttpRequest.Content);
            }
            catch (Exception)
            {
                var pkiStatusInfo = new PkiStatusInfo(
                    (int)PkiStatus.Rejection,
                    new PkiFreeText(new DerUtf8String("Query in bad format")),
                    new PkiFailureInfo(PkiFailureInfo.BadDataFormat));

                return new TspReqResult
                {
                    PkiStatusInfo = pkiStatusInfo
                };
            }

            // Validates whether the request uses accepted hash algorithms
            if (AcceptedAlgorithms.All(algorithm => algorithm.Id != timeStampRequest.MessageImprintAlgOid))
            {
                var pkiStatusInfo = new PkiStatusInfo(
                    (int)PkiStatus.Rejection,
                    new PkiFreeText(new DerUtf8String("Hash Algorithm is not accepted.")),
                    new PkiFailureInfo(PkiFailureInfo.BadAlg));

                return new TspReqResult
                {
                    PkiStatusInfo = pkiStatusInfo
                };
            }

            // Validates whether the hashed message length matches the digest length of the hash algorithm
            if(timeStampRequest.GetMessageImprintDigest().Length != TspAlgorithmUtil.GetDigestLength(new DerObjectIdentifier(timeStampRequest.MessageImprintAlgOid)))
            {
                var pkiStatusInfo = new PkiStatusInfo(
                    (int)PkiStatus.Rejection,
                    new PkiFreeText(new DerUtf8String("Digest length is not equal the message imprint length.")),
                    new PkiFailureInfo(PkiFailureInfo.BadDataFormat));

                return new TspReqResult
                {
                    PkiStatusInfo = pkiStatusInfo
                };
            }

            // Validates whether the TSA accepts the policy for stamping
            if (timeStampRequest.ReqPolicy != null && timeStampRequest.ReqPolicy != BcTimeStampResponderRepository.GetPolicyOid())
            {
                var pkiStatusInfo = new PkiStatusInfo(
                    (int)PkiStatus.Rejection,
                    new PkiFreeText(new DerUtf8String("TSP policy is unknown.")),
                    new PkiFailureInfo(PkiFailureInfo.UnacceptedPolicy));

                return new TspReqResult
                {
                    PkiStatusInfo = pkiStatusInfo
                };
            }

            // Validates whether the TSA accepts the extensions
            if (timeStampRequest.HasExtensions)
            {
                var acceptedExtensions = BcTimeStampResponderRepository.GetAcceptedExtensions();
                var extensions = timeStampRequest.GetExtensionOids()
                    .Cast<DerObjectIdentifier>()
                    .Select(oid => timeStampRequest.GetExtension(oid));

                if (extensions.Any(e => !acceptedExtensions.Any(a => a.IsCritical == e.IsCritical && Equals(a.Value, e.Value))))
                {
                    var pkiStatusInfo = new PkiStatusInfo(
                        (int)PkiStatus.Rejection,
                        new PkiFreeText(new DerUtf8String("TSP does not recognizes any extensions")),
                        new PkiFailureInfo(PkiFailureInfo.UnacceptedExtension));

                    return new TspReqResult
                    {
                        PkiStatusInfo = pkiStatusInfo
                    };
                }
            }

            // returns the time stamp request with granted status
            return new TspReqResult
            {
                PkiStatusInfo = new PkiStatusInfo((int)PkiStatus.Granted),
                TimeStampRequest = timeStampRequest
            };
        }

        /// <summary>
        /// Creates a <see cref="TspHttpResponse"/> including the tsp response as byte array
        /// </summary>
        /// <param name="tspResponseBytes">tsp response as byte array</param>
        /// <returns><see cref="TspHttpResponse"/></returns>
        private TspHttpResponse CreateResponse(byte[] tspResponseBytes)
        {
            return new TspHttpResponse(tspResponseBytes, "application/timestamp-reply");
        }

        /// <summary>
        /// Hash algorithms for TSP that are currently safe 4
        /// </summary>
        private static DerObjectIdentifier[] AcceptedAlgorithms { get; } =
        {
            NistObjectIdentifiers.IdSha224,
            NistObjectIdentifiers.IdSha256,
            NistObjectIdentifiers.IdSha384,
            NistObjectIdentifiers.IdSha512,
            TeleTrusTObjectIdentifiers.RipeMD128,
            TeleTrusTObjectIdentifiers.RipeMD160,
            TeleTrusTObjectIdentifiers.RipeMD256
        };

        /// <inheritdoc cref="IBcTimeStampResponderRepository"/>
        private IBcTimeStampResponderRepository BcTimeStampResponderRepository { get; }

        public TimeStampResponder(ITimeStampResponderRepository timeStampResponderRepository)
        {
            BcTimeStampResponderRepository = new BcTimeStampResponderRepositoryAdapter(timeStampResponderRepository);
        }
    }
}
