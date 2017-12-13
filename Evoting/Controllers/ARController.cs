using Evoting.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Evoting.Controllers
{
    public class ARController : Controller
    {
        private readonly EvoteEntities1 db = new EvoteEntities1();
        // GET: AR
        public ActionResult Index(int? _id ,string _dataBase64)
        {
            var user = Session["UserSession"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Index", "Vote");
            }
            var _token = Session["TokenSession"] as TokenSession;
            var _userToken = Session["user_TokenSession"] as UserSession;

            var _decryptedToken = DecryptToken(_token.Token, _token.TokenKey);

            if( _token != null && _userToken != null)
            {
                 if (_userToken.Token == _decryptedToken )
                 {
                    if (_dataBase64 == null)
                    {
                        return RedirectToAction("CandidateVote", "Candidates", new { _id = _id });
                    }
                    else return RedirectToAction("Index", "Block", new { _id = _id, _dataBase64 = _dataBase64 });
                    
                 }                 
                 else return RedirectToAction("SignIn", "Vote");
            }
           

            return RedirectToAction("SignIn", "Vote");

        }

  

        //Decrypt Token
        public static string DecryptToken(string data, uint[] key)
        {
            var dataBytes = Convert.FromBase64String(data);
            var result = Decrypt(dataBytes, key);
            return Encoding.Unicode.GetString(result);
        }

        //Decrypt code
        public static byte[] Decrypt(byte[] data, uint[] key)
        {
            if (data.Length % 8 != 0) throw new ArgumentException("Encrypted data length must be a multiple of 8 bytes.");
            var blockBuffer = new uint[2];
            var buffer = new byte[data.Length];
            Array.Copy(data, buffer, data.Length);
            using (var stream = new MemoryStream(buffer))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    for (int i = 0; i < buffer.Length; i += 8)
                    {
                        blockBuffer[0] = BitConverter.ToUInt32(buffer, i);
                        blockBuffer[1] = BitConverter.ToUInt32(buffer, i + 4);
                        DecryptAlgorithm(32, blockBuffer, key);
                        writer.Write(blockBuffer[0]);
                        writer.Write(blockBuffer[1]);
                    }
                }
            }
            // verify valid length
            var length = BitConverter.ToUInt32(buffer, 0);
            if (length > buffer.Length - 4) throw new ArgumentException("Invalid encrypted data");
            var result = new byte[length];
            Array.Copy(buffer, 4, result, 0, length);
            return result;
        }

        //Decrypt Algorithm
        private static void DecryptAlgorithm(uint rounds, uint[] v, uint[] key)
        {
            uint v0 = v[0], v1 = v[1], delta = 0x9E3779B9, sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
                sum -= delta;
                v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
            }
            v[0] = v0;
            v[1] = v1;
        }
    }
}