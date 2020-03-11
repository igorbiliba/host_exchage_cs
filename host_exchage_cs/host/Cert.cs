using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Host
{
    public class Cert
    {
        public X509Certificate2 cert;

        public const string USER = "hostexchange";
        const string ISSUER = "CN=" + USER;

        public string fileName
        {
            get
            {
                return Environment.CurrentDirectory + "\\ssl.cert";
            }
        }
        
        public int GetPort(string address)
        {
            string[] scope = address
                    .Replace("https://", "")
                    .Replace("http://", "")
                    .Replace("/", "")
                    .Split(':');

            return int.Parse(scope[1]);
        }

        public void RegenCert(string address)
        {
            MakeCert();
            cert = NetshCert(address);
            RemoveOldCerts();
            AddCertToTrust();
        }

        void RemoveOldCerts()
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            var list = store.Certificates;

            List<X509Certificate2> forRemove = new List<X509Certificate2>();
            foreach (X509Certificate2 i in list)
                if(i.GetName() == ISSUER)
                    forRemove.Add(i);

            foreach (X509Certificate2 i in forRemove)
                store.Remove(i);

            store.Close();
        }

        void AddCertToTrust()
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(fileName)));
            store.Close();
        }

        void MakeCert()
        {
            string arguments =
                string.Format(
                    "-n \"{0}\" -ss MY -sr LocalMachine -b 01/01/{1} -e 01/01/{2} -a sha256 -sky exchange -r -pe \"{3}\"",
                    ISSUER, DateTime.Now.Year, DateTime.Now.Year+2, fileName
                );

            Process p = Process.Start(App.settings.data.makecert_path, arguments);
            p.WaitForExit();
        }

        X509Certificate2 NetshCert(string address)
        {
            var app = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\netsh.exe";

            X509Certificate2 certificate = new X509Certificate2(fileName);
            var args = string.Format("http add sslcert ipport=0.0.0.0:{0} certhash={1} appid={{{2}}}", GetPort(address), certificate.Thumbprint, Guid.NewGuid());
            Process bindPortToCertificate = Process.Start(app, args);
            bindPortToCertificate.WaitForExit();

            return certificate;
        }
    }
}
