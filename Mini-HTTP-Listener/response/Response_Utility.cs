using System.Security.Cryptography;
using System.Text;

namespace Mini_HTTP_Listener.response
{
    internal partial class Response
    {
        private string CreateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input.Normalize(NormalizationForm.FormKC));
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("X2"));

            return sb.ToString().ToLower();
        }
    }
}
