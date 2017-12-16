using Evoting.Interfaces;
using Evoting.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Evoting.Controllers
{
    public class ASController : Controller
    {
        private readonly EvoteEntities1 db = new EvoteEntities1();
        private IBlock _iBlock;

        public ASController()
        {
            IBlock _data = _iBlock;
        }

        public ASController(IBlock _data)
        {
            this._iBlock = _data;
        }

        // GET: AS

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult create_BC(string _key, string _username)
        {
            var user = db.Users.FirstOrDefault(x => x.Username.Equals(_username));
            if(user.Public_key == null)
            {
                string _initial = string.Empty;
                int _sNumber = 3;
                for (var x = 0; x < _sNumber; x++)
                {
                    _initial += '0';
                }

                user.Public_key = _key;

                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var _prevBlock = db.Blocks
                                .OrderByDescending(x => x.Block_ID)
                                .Select(x => x.Code)
                                .First().ToString();

                Block b = new Block()
                {
                    Block_key = _key,
                    Prev_ID = _prevBlock,
                    Network = "1"
                };

                var _tempBlock = db.Blocks.SingleOrDefault(x => x.Block_key == _key);

                for (int i = 0; i <= 500000; i++)
                {
                    string stemp = b.Block_key + b.Data + i + b.Prev_ID;
                    byte[] bytes = Encoding.UTF8.GetBytes(stemp);
                    SHA256Managed hashstring = new SHA256Managed();
                    byte[] hash = hashstring.ComputeHash(bytes);
                    string hashString = string.Empty;
                    foreach (byte x in hash)
                    {
                        hashString += String.Format("{0:x2}", x);
                    }
                    if (hashString.Substring(0, _sNumber) == _initial)
                    {
                        b.Secret_key = i.ToString();
                        b.Code = hashString;


                        db.Blocks.Add(b);
                        db.SaveChanges();
                        break;
                    }

                }


                //_iBlock.isMine(_key, b.Data, _prevBlock, b.Block_key);
            }

            
            //Generate a token
            Random rnd = new Random();
            string _token = string.Empty;

            for (int i = 0; i < 10; i++)
            {
                int _tempToken = rnd.Next(0, 9);
                _token += _tempToken.ToString() ;
            }

            //Encrypt token and send with random key to AR server
            string _keyToken = Guid.NewGuid().ToString("N");

            var _encryptToken = encryptToken(_token, _keyToken);
            var _encryptKey = CreateKey(Encoding.Unicode.GetBytes(_keyToken));

            Session["UserSession"] = new UserSession { Username = _username };
            Session["TokenSession"] = new TokenSession { Token = _encryptToken, TokenKey = _encryptKey };
            Session["user_TokenSession"] = new UserSession { Token = _token };
            return RedirectToAction("Index", "Candidates");
        }

        //Encryt Token
        public static string encryptToken(string data, string key)
        {
            var dataBytes = Encoding.Unicode.GetBytes(data);
            var keyBytes = Encoding.Unicode.GetBytes(key);
            var result = Encrypt(dataBytes, keyBytes);
            return Convert.ToBase64String(result);
        }

        //Encrypt code
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            var keyBuffer = CreateKey(key);
            var blockBuffer = new uint[2];
            var result = new byte[NextMultipleOf8(data.Length + 4)];
            var lengthBuffer = BitConverter.GetBytes(data.Length);
            Array.Copy(lengthBuffer, result, lengthBuffer.Length);
            Array.Copy(data, 0, result, lengthBuffer.Length, data.Length);
            using (var stream = new MemoryStream(result))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    for (int i = 0; i < result.Length; i += 8)
                    {
                        blockBuffer[0] = BitConverter.ToUInt32(result, i);
                        blockBuffer[1] = BitConverter.ToUInt32(result, i + 4);
                        EncryptAlgorithm(32, blockBuffer, keyBuffer);
                        writer.Write(blockBuffer[0]);
                        writer.Write(blockBuffer[1]);
                    }
                }
            }
            return result;
        }

        //Encrypt Algorithm
        private static void EncryptAlgorithm(uint rounds, uint[] v, uint[] key)
        {
            uint v0 = v[0], v1 = v[1], sum = 0, delta = 0x9E3779B9;
            for (uint i = 0; i < rounds; i++)
            {
                v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
                sum += delta;
                v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
            }
            v[0] = v0;
            v[1] = v1;
        }

        //Create 4 key for XTEA
        private static uint[] CreateKey(byte[] key)
        {
            var hash = new byte[16];
            for (int i = 0; i < key.Length; i++)
            {
                hash[i % 16] = (byte)((31 * hash[i % 16]) ^ key[i]);
            }

            return new[] {
                BitConverter.ToUInt32(hash, 0), BitConverter.ToUInt32(hash, 4),
                BitConverter.ToUInt32(hash, 8), BitConverter.ToUInt32(hash, 12)
            };
        }

        //XTEA la ma hoa 64 bit => kiem tra boi so cua 64
        private static int NextMultipleOf8(int length)
        {
            return (length + 7) / 8 * 8; 
        }

    }
}