using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack; // Thư viện để phân tích HTML

namespace ChatChit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MessagesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetLinkPreview(string url)
        {
            try
            {
                // Tạo một HttpClient để gửi yêu cầu HTTP
                var client = _httpClientFactory.CreateClient();

                // Gửi yêu cầu HTTP để lấy HTML của trang web
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Đảm bảo yêu cầu thành công

                var html = await response.Content.ReadAsStringAsync();

                // Sử dụng thư viện HtmlAgilityPack để phân tích HTML và trích xuất các thông tin cần thiết
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var titleNode = htmlDocument.DocumentNode.SelectSingleNode("//title");
                var descriptionNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='description']");
                var imageNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[@property='og:image']");

                var title = titleNode?.InnerText;
                var description = descriptionNode?.Attributes["content"]?.Value;
                var image = imageNode?.Attributes["content"]?.Value;

                // Trả về thông tin trích dẫn dưới dạng JSON
                return Ok(new
                {
                    Title = title,
                    Description = description,
                    Image = image
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching link preview");
            }
        }
    }
}
