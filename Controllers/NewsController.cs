using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace DashboardApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public NewsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("svt")]
    public async Task<IActionResult> GetSvtNews()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; DashboardBot/1.0)");

        string xml;
        try
        {
            xml = await client.GetStringAsync("https://www.svt.se/nyheter/rss.xml");
        }
        catch (Exception ex)
        {
            return StatusCode(502, new { error = "Failed to fetch SVT news feed.", detail = ex.Message });
        }

        XDocument doc;
        try
        {
            doc = XDocument.Parse(xml);
        }
        catch (Exception ex)
        {
            return StatusCode(502, new { error = "Failed to parse SVT news feed.", detail = ex.Message });
        }

        var items = doc.Descendants("item")
            .Take(20)
            .Select(item => new SvtNewsItem(
                Title: item.Element("title")?.Value ?? "",
                Link: item.Element("link")?.Value ?? "",
                Description: StripHtml(item.Element("description")?.Value ?? ""),
                PubDate: item.Element("pubDate")?.Value ?? ""
            ))
            .ToList();

        return Ok(items);
    }

    private static string StripHtml(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return System.Text.RegularExpressions.Regex.Replace(input, "<[^>]*>", "").Trim();
    }
}

record SvtNewsItem(string Title, string Link, string Description, string PubDate);
