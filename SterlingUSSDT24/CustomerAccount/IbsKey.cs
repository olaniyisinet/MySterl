using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerAccount
{
    internal class IbsKey
    {
        public int AppId { get; set; }
        public string Key { get; set; }
        public string Vector { get; set; }
        public IbsKey()
        {
            this.AppId = 10;
            this.Key = "000000010000001000000101000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100001100000000110000010100000111000010110000110100011011";
            this.Vector = "0000000100000010000000110000010100000111000010110000110100000100";
        }
    }
}
