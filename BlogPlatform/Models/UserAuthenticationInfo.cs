using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlatform.Models
{
    public class UserAuthenticationInfo : UserBasicInfo
    {
        public string Password { get; set; }
    }
}
