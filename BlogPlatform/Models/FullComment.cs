using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlatform.Models
{
    public class FullComment : Comment
    {
        public int Author { get; set; }
        public int Id { get; set; }
    }
}
