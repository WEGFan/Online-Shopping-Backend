using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingBackend.Utils
{
    public class PasswordEncryptor
    {
        public PasswordEncryptor()
        {

        }

        private string sha256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        public string generateSalt()
        {
            return sha256("yeKut(>{7Xgu{J:^oNaz@}.otllitNZ3[U@" + Guid.NewGuid().ToString() +
                        "y@erK3ustt(`>l{H7.XYgzug{oJU:i^{aodIXo}`kZKM");
        }

        public string encryptPassword(string password, string salt)
        {
            byte[] s = Encoding.UTF8.GetBytes("&Rgb\"h1#PSV{)EOnXBI2b" + password +
                            "ov#2T=uiFEs$qVKX<:Y.NV.Y84Wx" + salt +
                            "tL.$3*;1N&11[uoAPXxj2=#\"%1\"%B");
            return Convert.ToBase64String(SHA512.Create().ComputeHash(s));
        }
    }
}
