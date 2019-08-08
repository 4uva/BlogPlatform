using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlatform.Models
{
    public class FullBlogPost : BlogPost
    {
        public List<FullComment> Comments { get; set; }
        public int BlogPostId { get; set; }
    }
}
