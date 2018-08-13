using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TspResponder
{
    /// <summary>
    /// Contract that a TSP Responder uses to stamp datum
    /// </summary>
    public interface ITimeStampResponderRepository
    {
        /// <summary>
        /// Gets the public key certificate of the TSA
        /// </summary>
        /// <returns>A <see cref="X509Certificate2"/></returns>
        Task<X509Certificate2> GetCertificate();

        /// <summary>
        /// Gets the private key of the TSA
        /// </summary>
        /// <returns>A <see cref="AsymmetricAlgorithm"/> that represents the private key of the TSA</returns>
        Task<AsymmetricAlgorithm> GetPrivateKey();

        /// <summary>
        /// Gets the accepted extensions of the TSA
        /// </summary>
        /// <returns></returns>
        IEnumerable<X509Extension> GetAcceptedExtensions();

        /// <summary>
        /// Gets the policy object identifier of the TSA
        /// </summary>
        /// <returns>policy object identifier</returns>
        string GetPolicyOid();

        /// <summary>
        /// Gets the next serial number for the stamping
        /// </summary>
        /// <returns>A <see cref="long"/> that represents the serial number for the stamping</returns>
        long GetNextSerialNumber();
    }
}