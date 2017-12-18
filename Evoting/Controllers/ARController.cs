using Evoting.Models;
using Evoting.ViewModel;
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


        #region Index

        /// <summary>
        ///     Verify user by decrypt token and redirect encrypt vote
        /// </summary>
        /// <param name="id">Block id</param>
        /// <param name="_dataBase64">Encrypted vote</param>
        /// <returns>Verify user + passing encrypt vote</returns>
        public ActionResult Index(int? _id ,string _dataBase64)
        {
            var user = Session["UserSession"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Index", "Vote");
            }
            var _token = Session["TokenSession"] as TokenSession;
            var _userToken = Session["user_TokenSession"] as UserSession;

            var _decryptedToken = TokenModel.DecryptToken(_token.Token, _token.TokenKey);

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

        #endregion



    }
}