using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Evoting.Models;

namespace Evoting.Controllers
{
    public class VoteController : Controller
    {
        private EvoteEntities1 db = new EvoteEntities1();
        // GET: Vote
        #region Index

        /// <summary>
        ///     Home page
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {            
            return View();
        }
        #endregion


        #region SignUp

        /// <summary>
        ///     SignUp page + Verify citizen
        /// </summary>        /// 
        /// <param name="key">Citizen ID</param>
        /// <param name="username">User's username</param>
        /// <param name="password">User's password</param>
        /// <returns>View</returns>
        public ActionResult SignUp()
        {
            var user = Session["UserSession"] as UserSession;
            if (user != null)
            {
                return RedirectToAction("Index", "Candidates");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Verify(string key)
        {
            var id = db.Citizens.SingleOrDefault(x => x.ID.Equals(key));
            if (id == null)
            {
                ModelState.AddModelError("", "Your ID is invalid !");
            }
            else
            {
                return RedirectToAction("");
            }
            return View("SignUp");
        }

        [HttpPost]
        public ActionResult checkID(string key)
        {
            var id = db.Citizens.SingleOrDefault(x => x.ID.Equals(key));
            if (id == null)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new
                {
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        [HttpPost]
        public ActionResult SingUp(string username, string password, string key)
        {
            var checkID = db.Citizens.SingleOrDefault(x => x.ID.Equals(key));

            if (checkID.Account != 1)
            {
                if (username == null || password == null || username == "" || password == "")
                {
                    var checkUsername = db.Users.SingleOrDefault(x => x.Username.Equals(username));
                    if (checkUsername != null)
                    {
                        return Content("Please select another username.");
                    }
                    else return Content("Please fill in !");
                }
                else
                {
                    var user = new User
                    {
                        Username = username,
                        Password = password,
                        Coin = 1
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    var citizen = db.Citizens.SingleOrDefault(x => x.ID.Equals(key));

                    citizen.Account = 1;

                    db.Entry(citizen).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    return Json(new
                    {
                        success = true
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Content("You already created an account.");
            }

        }

        #region SignIn

        /// <summary>
        ///     SignIn page
        /// <param name="Username">User's username</param>
        /// <param name="Password">User's password</param>
        /// </summary>
        /// <returns>View</returns>
        public ActionResult SignIn()
        {
            var user = Session["UserSession"] as UserSession;
            if (user != null)
            {
                return RedirectToAction("Index", "Candidates");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            if (Username == "admin" && Password == "admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var user = db.Users.FirstOrDefault(x => x.Username.Equals(Username));
                if (user == null)
                {
                    ModelState.AddModelError("", "Your account not exist !");
                    return View("SignIn");
                }
                else if (user.Password.Equals(Password))
                {
                    //Generate public key > send to AS 
                    string key = Guid.NewGuid().ToString("N");
                    return RedirectToAction("create_BC", "AS", new { _key = key, _username = Username });
                }
                else
                {
                    ModelState.AddModelError("", "Your username or password is not correct!");
                    return View("SignIn");
                }
            }            
        }

        #endregion



        #region VoteSuccess

        /// <summary>
        ///     VoteSuccess page
        /// </summary>
        /// <returns>View</returns>        
        public ActionResult VoteSuccess()
        {
            return View();
        }

        #endregion

        #region SignUpSuccess

        /// <summary>
        ///     SignUpSuccess page
        /// </summary>
        /// <returns>View</returns>  
        public ActionResult SignUpSuccess()
        {
            return View();
        }

        #endregion

        #region Logout

        /// <summary>
        ///     User logout
        /// </summary>
        /// <returns>View</returns>  
        public ActionResult Logout()
        {
            Session["UserSession"] = null;
            Session["TokenSession"] = null;
            Session["user_TokenSession"] = null;
            return RedirectToAction("Index", "Vote");
        }

        #endregion
    }
}