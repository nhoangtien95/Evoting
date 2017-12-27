using Evoting.Interfaces;
using Evoting.Models;
using Evoting.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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


        #region create_BC

        /// <summary>
        ///     Initial when user login + create token key to verify
        /// </summary>
        /// <param name="_key">Key generated and add to block</param>
        /// <param name="_username">User username</param>
        /// <returns>Redirect to Candidates view + token key</returns>
        public ActionResult create_BC(string _key, string _username)
        {
            var user = db.Users.FirstOrDefault(x => x.Username.Equals(_username));
            var block = new BlockChainModel();

            if(user.Public_key == null)
            {
                block.initialBlock(_key, user);
            }

            
            //Generate a token
            Random rnd = new Random();
            int _num = rnd.Next(0, 9);
            string _token = Membership.GeneratePassword(24, _num);

            //Encrypt token and send with random key to AR server
            string _keyToken = Guid.NewGuid().ToString("N");

            var _encryptToken = TokenModel.encryptToken(_token, _keyToken);
            var _encryptKey = TokenModel.CreateKey(Encoding.Unicode.GetBytes(_keyToken)); // 64 bytes


            Session["UserSession"] = new UserSession { Username = _username };
            Session["TokenSession"] = new TokenSession { Token = _encryptToken, TokenKey = _encryptKey };
            Session["user_TokenSession"] = new UserSession { Token = _token };
            return RedirectToAction("Index", "Candidates");
        }

        #endregion
    }
}