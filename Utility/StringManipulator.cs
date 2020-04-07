using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BBaB.Utility
{
    public class StringManipulator
    {
        public StringManipulator()
        {

        }

        /**
         * <summary>Takes in a string and one way encrypts it to a 64 character string.</summary>
         * <param name="rawData">Type String</param>
         * <returns>String</returns>
         */
        public string HashPassword(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /**
         * <summary>Generates a random string.</summary>
         * <param name="length"></param>
         * <returns>string</returns>
         */
        public string GenerateString(int length)
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] code = new byte[length];
            rngCsp.GetBytes(code, 0, length);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < code.Length; i++)
            {
                builder.Append(code[i].ToString("x2"));
            }
            return builder.ToString().Substring(0, length);
        }
    }
}