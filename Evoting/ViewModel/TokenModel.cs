using Evoting.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Evoting.ViewModel
{
    public class TokenModel
    {
        //Create 4 key for XTEA
        public static uint[] CreateKey(byte[] key)
        {
            var hash = new byte[16];
            for (int i = 0; i < key.Length; i++)
            {
                // cast byte ( 4 byte ), get last byte and convert to decimal.
                hash[i % 16] = (byte)((31 * hash[i % 16]) ^ key[i]);
            }

            //get 4 coutinous byte and backward.
            return new[] {
                BitConverter.ToUInt32(hash, 0), BitConverter.ToUInt32(hash, 4),
                BitConverter.ToUInt32(hash, 8), BitConverter.ToUInt32(hash, 12)
            };
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
        private static byte[] Encrypt(byte[] data, byte[] key)
        {
            var keyBuffer = CreateKey(key);
            var blockBuffer = new uint[2];
            var result = new byte[NextMultipleOf8(data.Length + 4)]; // + 4 stand 4 bytes of length for decrypt.
            var lengthBuffer = BitConverter.GetBytes(data.Length); // add 4 byte of length to buffer.
            Array.Copy(lengthBuffer, result, lengthBuffer.Length); // copy buffer to result with range = buffer length.
            Array.Copy(data, 0, result, lengthBuffer.Length, data.Length); // copy data to result, start at data[0] into result[buffer length]  with range = data length.
            using (var stream = new MemoryStream(result)) // stream store memory, non-resizable.
            {
                using (var writer = new BinaryWriter(stream)) // writing byte in stream.
                {
                    for (int i = 0; i < result.Length; i += 8) // writing byte in stream.
                    {
                        //Encrypt each 4 bytes of data, backward.
                        blockBuffer[0] = BitConverter.ToUInt32(result, i);
                        blockBuffer[1] = BitConverter.ToUInt32(result, i + 4);
                        EncryptAlgorithm(32, blockBuffer, keyBuffer);
                        writer.Write(blockBuffer[0]); // write 4 bytes of buffer to result, advances the position by 4 byte.
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


        //XTEA la ma hoa 64 bit => kiem tra boi so cua 64
        private static int NextMultipleOf8(int length)
        {
            return (length + 7) / 8 * 8;
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