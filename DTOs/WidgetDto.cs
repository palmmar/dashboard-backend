using System.Text.Json;

namespace DashboardApi.DTOs;

public class WidgetDto
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Size { get; set; } = "";
    public LayoutDto Layout { get; set; } = new();
    public JsonElement Config { get; set; }
}

public class LayoutDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
}
