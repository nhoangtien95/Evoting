using Evoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Evoting.Interfaces
{
    public interface IBlock : IDisposable
    {
        bool isMine(string _key, string _data, string _prevID, string _Block_key);
        
    }
}