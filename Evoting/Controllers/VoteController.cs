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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult SignIn()
        {
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

        [HttpPost]
        public ActionResult SingUp(string username, string password)
        {
            if (username != null)
            {
                return Content("Please fill in !");

            }
            else
            {
                return Json(new
                {
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            var user = db.Users.FirstOrDefault(x => x.Username.Equals(Username));
            if (user == null)
            {
                ModelState.AddModelError("", "Your account not exist !");
                return View("SignIn");
            }
            else if (user.Password.Equals(Password))
            {
                Session["UserSession"] = new UserSession { Username = Username, Password = Password };

                //generate public key > send to AS 
                string key = Guid.NewGuid().ToString("N");
                return RedirectToAction("create_BC", "AS", new { _key = key, _username = Username });
            }
            else
            {
                ModelState.AddModelError("", "Your username or password is not correct!");
                return View("SignIn");
            }
        }

        public ActionResult Vote()
        {
            return View();
        }

        public ActionResult Candidate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult abc()
        {
            return View();
        }
    }
}