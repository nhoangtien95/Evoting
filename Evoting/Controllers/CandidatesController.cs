using Evoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Evoting.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly EvoteEntities1 db = new EvoteEntities1();
        // GET: Candidates
        public ActionResult Index()
        {
            var candidates = db.Candidates.ToList();
            Random rnd = new Random();
            int r = rnd.Next(candidates.Count());

            ViewBag.RndCandidate = candidates[r];
            ViewBag.Candidates = candidates;
            return View();
        }

        public ActionResult CandidateDetail(int _id = 0)
        {
            if ( _id == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var candidate = db.Candidates.SingleOrDefault(x => x.ID.Equals(_id));
                var candidates = db.Candidates.Where(x => x.ID != _id).Take(6).ToList();

                ViewBag.Candidates = candidates;
                ViewBag.Candidate = candidate;
                return View();
            }
           
        }
    }
}