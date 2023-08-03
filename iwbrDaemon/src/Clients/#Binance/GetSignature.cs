using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        protected static string GetSignature(RequestParameters parameters)
        {
            var signature = "";
            if (parameters.Count > 0)
            {
                foreach (var item in parameters)
                {
                    if (item.Name != "X-MBX-APIKEY")
                        signature += $"{item.Name}={item.Value}&";
                }

                if (signature != string.Empty)
                {
                    signature = signature.Substring(0, signature.Length - 1);
                }
            }


            byte[] keyBytes = Encoding.UTF8.GetBytes(_ApiSecretKey);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(signature);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(queryStringBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
