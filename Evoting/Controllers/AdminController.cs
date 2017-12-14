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
    public class AdminController : Controller
    {
        private readonly EvoteEntities1 db = new EvoteEntities1();
        // GET: Admin
        public ActionResult Index()
        {
            var _blocks = db.Blocks.ToList();

            ViewBag.Blocks = _blocks;
            return View();
        }
        
        public PartialViewResult BlockLoad()
        {
            var _blocks = db.Blocks.ToList();

            ViewBag.Blocks = _blocks;
            return PartialView();
        }

        [HttpPost]
        public ActionResult Hash(int _id, string _data)
        {
            var temp = db.Blocks.ToList();
            var initilal = temp.SingleOrDefault(x => x.Block_ID.Equals(_id));
            for(int i = temp.IndexOf(initilal); i < temp.Count(); i++)
            {
                if(i == temp.IndexOf(initilal))
                {
                    HashBlock(temp[i].Block_ID, _data, null);
                }
                else
                {
                    HashBlock(temp[i].Block_ID, temp[i].Data, temp[i-1].Code);
                }
            }


            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }

        public void HashBlock(int id, string data, string prev)
        {
            var item = db.Blocks.SingleOrDefault(x => x.Block_ID.Equals(id));
            if (prev == null || prev == "")
            {
                string stemp = item.Block_key + data + item.Secret_key + item.Prev_ID;
                byte[] bytes = Encoding.UTF8.GetBytes(stemp);
                SHA256Managed hashstring = new SHA256Managed();
                byte[] hash = hashstring.ComputeHash(bytes);
                string hashString = string.Empty;
                foreach (byte x in hash)
                {
                    hashString += String.Format("{0:x2}", x);
                }

                item.Code = hashString;
                item.Data = data;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                string stemp = item.Block_key + data + item.Secret_key + prev;
                byte[] bytes = Encoding.UTF8.GetBytes(stemp);
                SHA256Managed hashstring = new SHA256Managed();
                byte[] hash = hashstring.ComputeHash(bytes);
                string hashString = string.Empty;
                foreach (byte x in hash)
                {
                    hashString += String.Format("{0:x2}", x);
                }

                item.Code = hashString;
                item.Data = data;
                item.Prev_ID = prev;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
                
        }


        [HttpPost]
        public ActionResult IsMine(int _id)
        {

            return View("Index");
        }
    }
}