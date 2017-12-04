using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Evoting.Models
{
    public class UserSession
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}