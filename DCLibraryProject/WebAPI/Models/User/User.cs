using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models.User {
    public class User {

        public uint acctNo { get; set; }
        public uint pin { get; set; }
        public int balance { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public byte[]? imageBytes { get; set; }
    }
}
