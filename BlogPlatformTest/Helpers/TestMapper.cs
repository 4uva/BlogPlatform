using AutoMapper;
using BlogPlatform.Models;
using EFBlogPost = BlogPlatform.Models.EF_Model_classes.BlogPost;

namespace BlogPlatformTest.Helpers
{
    public class TestMapper : Profile
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<EFBlogPost, FullBlogPost>();
        }

        public static Mapper CreateMapper()
        {
            var mapperConf = new MapperConfiguration(Configure);
            return new Mapper(mapperConf);
        }
    }
}
