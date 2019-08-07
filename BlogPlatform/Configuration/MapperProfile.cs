using AutoMapper;
using BlogPlatform.Models;

using EFUser = BlogPlatform.Models.EF_Model_classes.User;
using EFBlog = BlogPlatform.Models.EF_Model_classes.Blog;
using EFBlogPost = BlogPlatform.Models.EF_Model_classes.BlogPost;

namespace BlogPlatform.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EFUser, UserExtendedInfo>().IncludeMembers(u => u.Blog);
            CreateMap<EFBlog, UserExtendedInfo>();

            CreateMap<EFBlogPost, BlogPost>();
            CreateMap<BlogPost, EFBlogPost>();
            CreateMap<EFBlogPost, FullBlogPost>();
            // back mapping should not be needed BlogPostWithComments->EFBlogPost
        }
    }
}
