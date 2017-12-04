using Evoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

            return View();
        }

    }
}