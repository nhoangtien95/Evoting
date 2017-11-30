using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Evoting.Controllers
{
    public class VoteController : Controller
    {
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
        public ActionResult Verify()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login()
        {
            //generate public key > send to AS 
            string key = Guid.NewGuid().ToString("N");
            return RedirectToAction("create_BC", "AS", new { key = key });
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