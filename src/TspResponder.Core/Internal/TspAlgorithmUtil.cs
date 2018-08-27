using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.TeleTrust;

namespace TspResponder.Core.Internal
{
    /// <summary>
    /// Utilities for Tsp Algorithms
    /// </summary>
    internal static class TspAlgorithmUtil
    {
        /// <summary>
        /// Gets the digest length of the specified hash algorithm
        /// </summary>
        /// <param name="algorithm">hash algorithm object identifier</param>
        /// <returns>an <see cref="Int32"/> that represents the digest length</returns>
        internal static int GetDigestLength(DerObjectIdentifier algorithm)
        {
            if (!DigestLengths.ContainsKey(algorithm))
                throw new ArgumentException("Invalid algorithm");

            return DigestLengths[algorithm];
        }

        /// <summary>
        /// Digest Lenghts for each hash algorithm
        /// </summary>
        private static Dictionary<DerObjectIdentifier, int> DigestLengths { get; } =
            new Dictionary<DerObjectIdentifier, int>()
            {
                { NistObjectIdentifiers.IdSha224, 28 },
                { NistObjectIdentifiers.IdSha256, 32 },
                { NistObjectIdentifiers.IdSha384, 48 },
                { NistObjectIdentifiers.IdSha512, 64 },
                { TeleTrusTObjectIdentifiers.RipeMD128, 16 },
                { TeleTrusTObjectIdentifiers.RipeMD160, 20 },
                { TeleTrusTObjectIdentifiers.RipeMD256, 32 }
            };
    }
}