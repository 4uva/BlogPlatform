using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlatform.Models.EF_Model_classes
{
    public class BlogPost
    {
        public int BlogPostId { get; set; }
        public string BodyText { get; set; }
        public string HeaderText { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public int BlogId { get; set; }
    }
}
