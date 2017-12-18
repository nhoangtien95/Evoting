using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Evoting.ViewModel
{
    public class VoteModel
    {
        public static byte[] RSAEncrypt(byte[] data, string pubKey)
        {
            byte[] encryptedData;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubKey);
            encryptedData = rsa.Encrypt(data, true);
            rsa.Dispose();
            return encryptedData;
        }

        public static string RSADecrypt(byte[] ciphertext, string srcKey)
        {
            byte[] decryptedData;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(srcKey);
            decryptedData = rsa.Decrypt(ciphertext, true);
            rsa.Dispose();
            return Encoding.Unicode.GetString(decryptedData);
        }
    }
}