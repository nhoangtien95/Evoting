using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Evoting.Controllers
{
    public class ASController : Controller
    {
        // GET: AS
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult create_BC(string key)
        {
            return View();
        }

    }
}