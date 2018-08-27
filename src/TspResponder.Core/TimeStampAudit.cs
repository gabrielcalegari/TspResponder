using System;

namespace TspResponder.Core
{
    /// <summary>
    /// Audit register
    /// </summary>
    public class TimeStampAudit
    {
        /// <summary>
        /// Digest algorithm for the request datum
        /// </summary>
        public string HashAlgorithm { get; set; }

        /// <summary>
        /// Digest message to be asigned time
        /// </summary>
        public byte[] HashMessage { get; set; }

        /// <summary>
        /// Tsa policy under what the token was generated
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// Certificate serial used to sign time
        /// </summary>
        public string TsaSerial { get; set; }

        /// <summary>
        /// Serial token of the signing
        /// </summary>
        public string Serial { get; set; }

        /// <summary>
        /// Time asigned to the datum
        /// </summary>
        public DateTime SignedTime { get; set; }
    }
}
