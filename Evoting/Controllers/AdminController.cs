using Evoting.Models;
using Evoting.ViewModel;
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

        #region Index

        /// <summary>
        ///     Show blocks in blockchain
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            var _blocks = db.Blocks.Where(x => x.Network == "1").ToList();

            ViewBag.Blocks = _blocks;
            return View();
        }
        #endregion

        #region Candidates

        /// <summary>
        ///     Show blocks of candidates in blockchain
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Candidates()
        {
            var _blocks = db.Blocks.Where(x => x.Network == "2" &&  x.Code.Substring(0, 3) == "000").ToList();

            ViewBag.Blocks = _blocks;
            return View();
        }
        #endregion

        #region NewCandidate

        /// <summary>
        ///     Create new candidate block
        /// </summary>
        /// <returns>View</returns>
        public ActionResult NewCandidate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewCandidate(AdminModel admin)
        {
            Block b = new Block()
            {
                Block_key = admin.Name,
                Network = "2"
            };

            var block = new BlockChainModel();
            block.createBlock(b);


            return View("Candidates");
        }

        #endregion


        public PartialViewResult BlockLoad()
        {
            var _blocks = db.Blocks.Where(x => x.Network == "1").ToList();

            ViewBag.Blocks = _blocks;
            return PartialView();
        }


        #region Hash

        /// <summary>
        ///     Hash each block to SHA256
        /// </summary>        /// 
        /// <param name="_id">Block id</param>
        /// <param name="_data">Block data </param>
        /// <returns>Hash code</returns>
        [HttpPost]
        public ActionResult Hash(int _id, string _data)
        {
            var temp = db.Blocks.Where(x => x.Network == "1").ToList();
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

        #endregion


        #region IsMine

        /// <summary>
        ///     Correct hash code
        /// </summary>        /// 
        /// <param name="_id">Block id</param>
        /// <returns>Correct hash code</returns>
        [HttpPost]
        public ActionResult IsMine(int _id)
        {
            var temp = db.Blocks.ToList();
            var initilal = temp.SingleOrDefault(x => x.Block_ID.Equals(_id));
            int num = temp.IndexOf(initilal);
            var preBlock = temp[num - 1];


            var _tempBlock = db.Blocks.SingleOrDefault(x => x.Block_ID == _id);
            string _initial = string.Empty;
            int _sNumber = 3;
            for (var x = 0; x < _sNumber; x++)
            {
                _initial += '0';
            }

            for (int i = 0; i <= 500000; i++)
            {
                string stemp = _tempBlock.Block_key + _tempBlock.Data + i + preBlock.Code;
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
                    _tempBlock.Prev_ID = preBlock.Code;
                    db.Entry(_tempBlock).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
            }



            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}