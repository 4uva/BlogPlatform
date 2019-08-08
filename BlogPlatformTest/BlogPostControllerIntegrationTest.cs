using BlogPlatform;
using BlogPlatform.Models;
using BlogPlatformTest.Helpers;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BlogPlatformIntegrationTest
{
    // based on articles
    // https://fullstackmark.com/post/20/painless-integration-testing-with-aspnet-core-web-api
    // and
    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2
    public class BlogPostControllerIntegrationtTest
        : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        CustomWebApplicationFactory<Startup> factory;

        public BlogPostControllerIntegrationtTest(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task TestGetExistingBlogPost()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("api/BlogPosts/1");
            response.EnsureSuccessStatusCode();
            var post = await response.Content.ReadAsAsync<FullBlogPost>();
            // check
            Assert.NotNull(post);
            Assert.Equal(DbCreator.postText, post.BodyText);
            Assert.Equal(DbCreator.postHeader, post.HeaderText);
        }
    }
}
