using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using BBaB.Utility.Interfaces;
namespace BBaB.Utility
{
    public class StringManipulator
    {
        private IBBaBLogger logger;

        public StringManipulator(IBBaBLogger logger)
        {
            this.logger = logger;
        }

        /**
         * <summary>Takes in a string and one way encrypts it to a 64 character string.</summary>
         * <param name="rawData">Type String</param>
         * <returns>String</returns>
         */
        public string HashPassword(string rawData)
        {
            this.logger.Info("Entering StringManipulator@HashPassword");

            // Create a SHA256   
            this.logger.Info("Creating SHA256 Object");
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                this.logger.Info("Creating and initializing byte array");
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                this.logger.Info("Creating StringBuilder");
                StringBuilder builder = new StringBuilder();
                this.logger.Info("Appending bytes together");
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                this.logger.Info("Returning hashed password");
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
            this.logger.Info("Entering StringManipulator@GenerateString");
            this.logger.Info("Creating Crypto Provider");
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

            this.logger.Info("Creating and initializing byte array");
            byte[] code = new byte[length];

            this.logger.Info("Getting random bytes from Crypto Provider for length of desired string");
            rngCsp.GetBytes(code, 0, length);

            this.logger.Info("Creating String builder");
            StringBuilder builder = new StringBuilder();

            this.logger.Info("Appending random byte information to string");
            for (int i = 0; i < code.Length; i++)
            {
                builder.Append(code[i].ToString("x2"));
            }

            this.logger.Info("Returning random string");
            return builder.ToString().Substring(0, length);
        }
    }
}