using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Core.Model
{
    public class TokenInfo
    {
        public bool Valid { set; get; }
        public string UserName { set; get; }
        public string[] Scope { set; get; }
    }
}
