namespace DashboardApi.Models;

public class DashboardWidget
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public AppUser User { get; set; } = null!;
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Size { get; set; } = "";
    public int LayoutX { get; set; }
    public int LayoutY { get; set; }
    public int LayoutW { get; set; }
    public int LayoutH { get; set; }
    public string ConfigJson { get; set; } = "{}";
    public int SortOrder { get; set; }
}
