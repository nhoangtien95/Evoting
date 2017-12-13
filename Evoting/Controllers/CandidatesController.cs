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
    public class CandidatesController : Controller
    {
        private readonly EvoteEntities1 db = new EvoteEntities1();
        // GET: Candidates
        public ActionResult Index()
        {
            var user = Session["UserSession"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Index", "Vote");
            }
            var candidates = db.Candidates.ToList();
            Random rnd = new Random();
            int r = rnd.Next(candidates.Count());

            ViewBag.RndCandidate = candidates[r];
            ViewBag.Candidates = candidates;
            return View();
        }

        public ActionResult CandidateDetail(int _id = 0)
        {
            var user = Session["UserSession"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Index", "Vote");
            }
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

        public ActionResult CandidateVote(int _id)
        {
            var user = Session["UserSession"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Index", "Vote");
            }
            var checkVote = db.Users.SingleOrDefault(x => x.Username.Equals(user.Username));
            if(checkVote.Voted != 1)
            {
                var _candidate = db.Candidates.SingleOrDefault(x => x.ID.Equals(_id));

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                string _pubkey = rsa.ToXmlString(false);
                string _prikey = rsa.ToXmlString(true);

                Session["PrivateKeySession"] = new PrivateKeySession { PrivateKey = _prikey };
                var encryptedVote = Convert.ToBase64String(RSAEncrypt(Encoding.Unicode.GetBytes(_candidate.Name), _pubkey));

                return RedirectToAction("Index", "AR", new { _id = _id, _dataBase64 = encryptedVote });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public static byte[] RSAEncrypt(byte[] data, string pubKey)
        {
            byte[] encryptedData;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubKey);
            encryptedData = rsa.Encrypt(data, true);
            rsa.Dispose();
            return encryptedData;
        }
    }
}