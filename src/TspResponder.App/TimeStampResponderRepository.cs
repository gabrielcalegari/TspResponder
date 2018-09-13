using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TspResponder.Core;

namespace TspResponder.App
{
    public class TimeStampResponderRepository : ITimeStampResponderRepository
    {
        public Task<X509Certificate2> GetCertificate()
        {
            return Task.FromResult(Certificate);
        }

        public Task<AsymmetricAlgorithm> GetPrivateKey()
        {
            return Task.FromResult(Certificate.PrivateKey);
        }

        public IEnumerable<X509Extension> GetAcceptedExtensions()
        {
            return new List<X509Extension>();
        }

        public string GetPolicyOid()
        {
            return "1.3.6.1.4.1.78878789.1.1";
        }

        public long GetNextSerialNumber()
        {
            return DateTime.Now.Ticks;
        }

        public DateTime GetTimeToSign()
        {
            return DateTime.UtcNow;
        }

        public Task SaveAuditLog(TimeStampAudit audit)
        {
            return Task.CompletedTask;
        }

        private static byte[] _certificate;

        private X509Certificate2 Certificate
        {
            get
            {
                if (_certificate == null)
                {
                    _certificate = GetFromFile();
                }
                
                return new X509Certificate2(_certificate, "12", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
            }
        }

        private static byte[] GetFromFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("TspResponder.App.tsp.pfx"))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            Certificate.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~TimeStampResponderRepository()
        {
            ReleaseUnmanagedResources();
        }
    }
}
