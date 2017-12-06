using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Evoting.Models
{
    public class TokenSession
    {
        public string Token { get; set; }
        public uint[] TokenKey { get; set; }

    }
}