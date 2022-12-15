using System.Threading.Tasks;
using ELibrary.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SSCMS.Web.Tests.IntegrationTests
{
    public class BasicTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/ping")]
        public async Task PingTests(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("pong", content);
        }
        [Fact]
        public void PdfTest()
        {
          var re=  PdfHandler.PegingFirstAsyn("D:\\sort\\1.pdf");
            System.Console.WriteLine(re);
            System.Console.ReadLine();
        }
        [Fact]
        public void Pdf2Test()
        {
            PdfHandler.ExtractPages("D:\\sort\\iot.pdf", "D:\\sort\\2__iot.pdf", 1,20);
        }
    }
}
