using Evoting.Models;
using System;
using System.Collections.Generic;
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
        // GET: AS
        
    public ActionResult Index()
        {
            return View();
        }

        public ActionResult create_BC(string _key, string _username)
        {
            string _initial = string.Empty;
            int _sNumber = 3;
            for (var x = 0; x < _sNumber; x++)
            {
                _initial += '0';
            }

            var user = db.Candidates.FirstOrDefault(x => x.Username.Equals(_username));
            user.Public_key = _key;

            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var _prevBlock = db.Blocks
                            .OrderByDescending(x => x.Block_ID)
                            .Select(x => x.Block_key)
                            .First().ToString();

            Block b = new Block()
            {
                Block_key = _key,
                Prev_ID = _prevBlock
            };

            db.Blocks.Add(b);
            db.SaveChanges();

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
                    _tempBlock.Secret_key = i.ToString();
                    _tempBlock.Code = hashString;
                    db.Entry(_tempBlock).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

            }

            return View();
        }

    }
}