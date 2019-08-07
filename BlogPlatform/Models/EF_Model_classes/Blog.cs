using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models.EF_Model_classes
{
    public class Blog
    {
        public int BlogId { get; set; }
       
        public ICollection<BlogPost> BlogPosts { get; set; }

        // needed for 1:1 relationship
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
