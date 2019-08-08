using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlatform.Models.EF_Model_classes
{
    public class Comment
    {
        public int CommentId { get; set; }

        public string Text { get; set; }

        public Blog Author { get; set; }

        public int BlogPostId { get; set; }
    }
}
