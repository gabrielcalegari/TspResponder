using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;

namespace TspResponder.Core.Internal
{
    /// <summary>
    /// Adapter from <see cref="ITimeStampResponderRepository"/> for BouncyCastle's library
    /// </summary>
    internal interface IBcTimeStampResponderRepository
    {
        Task<X509Certificate> GetCertificate();

        Task<AsymmetricKeyParameter> GetPrivateKey();

        IEnumerable<X509Extension> GetAcceptedExtensions();

        string GetPolicyOid();

        BigInteger GetNextSerialNumber();

        DateTime GetTimeToSign();

        Task SaveAuditLog(TimeStampAudit audit);
    }
}