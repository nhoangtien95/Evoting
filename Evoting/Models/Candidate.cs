//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Evoting.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Candidate
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Public_key { get; set; }
        public Nullable<int> Voted { get; set; }
        public Nullable<int> Coin { get; set; }
    }
}