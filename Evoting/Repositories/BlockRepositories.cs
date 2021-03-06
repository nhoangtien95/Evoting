﻿using Evoting.Interfaces;
using Evoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Evoting.Repositories
{
    public class BlockRepositories : IBlock, IDisposable
    {
        private readonly EvoteEntities1 db = new EvoteEntities1();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool isMine(string _key, string _data, string _prevID, string _Block_key)
        {
            var _tempBlock = db.Blocks.SingleOrDefault(x => x.Block_key == _key);
            string _initial = string.Empty;
            int _sNumber = 3;
            for (var x = 0; x < _sNumber; x++)
            {
                _initial += '0';
            }

            for (int i = 0; i <= 500000; i++)
            {
                string stemp = _Block_key + _data + i + _prevID;
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
                    _tempBlock.Secret_key = i.ToString();
                    _tempBlock.Code = hashString;
                    db.Entry(_tempBlock).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return true;
                    break;
                }               
            }
            return false;
        }
    }
}