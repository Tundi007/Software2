using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Contracts.Authentication
{
    public class TokenRequest
    {
        public string Token { get; set; }
    }

    public class TokenRequest2
    {
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}
