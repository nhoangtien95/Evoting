﻿using Evoting.Models;
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
    public class BlockController : Controller
    {
        private readonly EvoteEntities1 db = new EvoteEntities1();
        // GET: Block
        #region Index

        /// <summary>
        ///     Decrypt encrypted vote and passing data to blockchain
        /// </summary>
        /// <param name="_id">Block id</param>
        /// <param name="_dataBase64">Encrypted vote</param>
        /// <returns>Success vote</returns>
        public ActionResult Index(int _id, string _dataBase64)
        {
            var user = Session["UserSession"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Index", "Vote");
            }
            if (_dataBase64 == null)
            {
                return RedirectToAction("SignIn", "Vote");
            }
            else
            {
                var _data = Convert.FromBase64String(_dataBase64);
                var _privKey = Session["PrivateKeySession"] as PrivateKeySession;
                var _voteData = VoteModel.RSADecrypt(_data, _privKey.PrivateKey);

                var _temp = Session["UserSession"] as UserSession;
                var _user = db.Users.SingleOrDefault(x => x.Username.Equals(_temp.Username));
                var _block = db.Blocks.SingleOrDefault(x => x.Block_key.Equals(_user.Public_key));

                _block.Data = _voteData;
                db.Entry(_block).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var block = new BlockChainModel();
                block.isBlock(_user.Public_key, _block.Data, _block.Prev_ID, _block.Block_key);

                /////////////
                _user.Voted = 1;
                db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                /////////////

                var candidate = db.Blocks.SingleOrDefault(x => x.Block_key == _voteData);
                if(candidate != null)
                {
                    if(candidate.Data == null)
                    {
                        candidate.Data = "1";
                        db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        candidate.Data = (int.Parse(candidate.Data) + 1).ToString();
                        db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("VoteSuccess", "Vote");
            }
        }

        #endregion
    }
}