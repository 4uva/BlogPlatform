using AutoMapper;
using BlogPlatform.Models;

using EFUser = BlogPlatform.Models.EF_Model_classes.User;
using EFBlog = BlogPlatform.Models.EF_Model_classes.Blog;
using EFBlogPost = BlogPlatform.Models.EF_Model_classes.BlogPost;
using EFComment = BlogPlatform.Models.EF_Model_classes.Comment;

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

            // commentid, text, author.blogid -> id, text, author
            CreateMap<EFComment, FullComment>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.BlogId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CommentId));
            CreateMap<Comment, EFComment>();
        }
    }
}
