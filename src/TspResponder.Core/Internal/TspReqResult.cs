using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;

namespace TspResponder.Core.Internal
{
    internal class TspReqResult
    {
        public PkiStatusInfo PkiStatusInfo { get; set; }

        public TimeStampRequest TimeStampRequest { get; set; }

        public bool IsValid => PkiStatusInfo != null &&
                               (PkiStatusInfo.Status.Equals(BigInteger.Zero) || 
                                PkiStatusInfo.Status.Equals(BigInteger.One));
    }
}