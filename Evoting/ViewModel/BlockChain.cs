using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Evoting.ViewModel
{
    public class BlockChain
    {
        public string Block_key { get; set; }
        public string Data { get; set; }
        public string Code { get; set; }
        public string Prev_ID { get; set; }
        public string Network { get; set; }
        public string Secret_key { get; set; }
        public BlockChain()
        {

        }
        public BlockChain(string Block_key, string Data, string Code, string Prev_ID, string Network, string Secret_key)
        {
            this.Block_key = Block_key;
            this.Data = Data;
            this.Code = Code;
            this.Prev_ID = Prev_ID;
            this.Network = Network;
            this.Secret_key = Secret_key;
        }
    }
}