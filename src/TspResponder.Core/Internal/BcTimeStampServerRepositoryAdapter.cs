using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace TspResponder.Core.Internal
{
    /// <inheritdoc />
    internal class BcTimeStampResponderRepositoryAdapter : IBcTimeStampResponderRepository
    {
        /// <inheritdoc />
        public async Task<X509Certificate> GetCertificate()
        {
            var certificate = await TimeStampResponderRepository.GetCertificate();
            return DotNetUtilities.FromX509Certificate(certificate);
        }

        /// <inheritdoc />
        public async Task<AsymmetricKeyParameter> GetPrivateKey()
        {
            var privateKey = await TimeStampResponderRepository.GetPrivateKey();
            return DotNetUtilities.GetKeyPair(privateKey).Private;
        }

        /// <inheritdoc />
        public IEnumerable<X509Extension> GetAcceptedExtensions()
        {
            var extensions = TimeStampResponderRepository.GetAcceptedExtensions();
            return extensions.Select(extension =>
                new X509Extension(extension.Critical, new DerOctetString(extension.RawData)));
        }

        /// <inheritdoc />
        public string GetPolicyOid()
        {
            return TimeStampResponderRepository.GetPolicyOid();
        }

        /// <inheritdoc />
        public BigInteger GetNextSerialNumber()
        {
            long serial = TimeStampResponderRepository.GetNextSerialNumber();
            return BigInteger.ValueOf(serial);
        }

        /// <inheritdoc />
        public DateTime GetTimeToSign()
        {
            return TimeStampResponderRepository.GetTimeToSign();
        }

        /// <inheritdoc />
        public Task SaveAuditLog(TimeStampAudit audit)
        {
            return TimeStampResponderRepository.SaveAuditLog(audit);
        }

        /// <see cref="TimeStampResponderRepository"/>
        private ITimeStampResponderRepository TimeStampResponderRepository { get; }

        internal BcTimeStampResponderRepositoryAdapter(ITimeStampResponderRepository timeStampResponderRepository)
        {
            TimeStampResponderRepository = timeStampResponderRepository;
        }
    }
}