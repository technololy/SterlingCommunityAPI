﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SterlingCommunityAPI.Services
{
    public class Encryption
    {

        internal static string RequestSharedKey => "000000010000001000000101000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100001100000000110000010100000111000010110000110100011011";
        internal static string RequestSharedVector => "0000000100000010000000110000010100000111000010110000110100010001";
        private static string EncryptApiRequest(string val)
        {
            using (var ms = new MemoryStream())
            {
                try
                {
                    //string sharedkeyval = "_"; string sharedvectorval = "____";

                    string sharedkeyval = RequestSharedKey;
                    sharedkeyval = BinaryToString(sharedkeyval);

                    string sharedvectorval = RequestSharedVector;
                    sharedvectorval = BinaryToString(sharedvectorval);

                    byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
                    byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

                    var tdes = new TripleDESCryptoServiceProvider();
                    byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

                    CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
                    cs.Write(toEncrypt, 0, toEncrypt.Length);
                    cs.FlushFinalBlock();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return "";
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static string DecryptApiRequest(string val)
        {
            using (var ms = new MemoryStream())
            {
                try
                {
                    //string sharedkeyval = "_"; string sharedvectorval = "____";

                    string sharedkeyval = RequestSharedKey;
                    sharedkeyval = BinaryToString(sharedkeyval);

                    string sharedvectorval = RequestSharedVector;
                    sharedvectorval = BinaryToString(sharedvectorval);

                    byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
                    byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

                    var tdes = new TripleDESCryptoServiceProvider();
                    byte[] toDecrypt = Convert.FromBase64String(val);

                    var cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);

                    cs.Write(toDecrypt, 0, toDecrypt.Length);
                    cs.FlushFinalBlock();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static string BinaryToString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                throw new ArgumentNullException("binary");

            if ((binary.Length % 8) != 0)
                throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

            var builder = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string section = binary.Substring(i, 8);
                int ascii = 0;
                try
                {
                    ascii = Convert.ToInt32(section, 2);
                }
                catch
                {
                    throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
                }
                builder.Append((char)ascii);
            }
            return builder.ToString();
        }

        internal static string EncryptObject(dynamic values)
        {
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(values);
            //var _enc = Functions.EncryptApiRequest(json);
            //var reqq = Functions.Stringify(_enc);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(values);
            return EncryptApiRequest(json);
        }

        internal static string DecryptObject(string data)
        {
            string json2 = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(data);
            return DecryptApiRequest(json2);
        }

        public static string GenerateHash(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            using (var hasher = new HMACSHA1(keyBytes))
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                var hashMessage = hasher.ComputeHash(messageBytes);
                return BitConverter.ToString(hashMessage).Replace("-", string.Empty);
            }
        }

        public static string HmacSHA256(string message, byte[] key)
        {
            using (var sha256 = new HMACSHA256(key))
            {
                //var msgByte = GetMessageByte(message);
                var msgByte = Encoding.UTF8.GetBytes(message);

                var hash = sha256.ComputeHash(msgByte);

                StringBuilder hex = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    hex.AppendFormat("{0:x2}", b);
                var tter = hex.ToString();

                return Convert.ToBase64String(hash);
            }
        }
    }
}
