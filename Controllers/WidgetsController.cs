using System.Security.Claims;
using System.Text.Json;
using DashboardApi.Data;
using DashboardApi.DTOs;
using DashboardApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DashboardApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WidgetsController : ControllerBase
{
    private readonly AppDbContext _db;

    public WidgetsController(AppDbContext db)
    {
        _db = db;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var widgets = await _db.DashboardWidgets
            .Where(w => w.UserId == UserId)
            .OrderBy(w => w.SortOrder)
            .ToListAsync();

        return Ok(widgets.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WidgetDto dto)
    {
        var maxOrder = await _db.DashboardWidgets
            .Where(w => w.UserId == UserId)
            .Select(w => (int?)w.SortOrder)
            .MaxAsync() ?? -1;

        var widget = new DashboardWidget
        {
            Id = Guid.NewGuid().ToString(),
            UserId = UserId,
            Type = dto.Type,
            Title = dto.Title,
            Size = dto.Size,
            LayoutX = dto.Layout.X,
            LayoutY = dto.Layout.Y,
            LayoutW = dto.Layout.W,
            LayoutH = dto.Layout.H,
            ConfigJson = dto.Config.ValueKind == JsonValueKind.Undefined
                ? "{}"
                : dto.Config.GetRawText(),
            SortOrder = maxOrder + 1,
        };

        _db.DashboardWidgets.Add(widget);
        await _db.SaveChangesAsync();

        return Ok(ToDto(widget));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] WidgetDto dto)
    {
        var widget = await _db.DashboardWidgets
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);

        if (widget is null) return NotFound();

        widget.Title = dto.Title;
        widget.Size = dto.Size;
        widget.LayoutX = dto.Layout.X;
        widget.LayoutY = dto.Layout.Y;
        widget.LayoutW = dto.Layout.W;
        widget.LayoutH = dto.Layout.H;

        if (dto.Config.ValueKind != JsonValueKind.Undefined)
            widget.ConfigJson = dto.Config.GetRawText();

        await _db.SaveChangesAsync();
        return Ok(ToDto(widget));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var widget = await _db.DashboardWidgets
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);

        if (widget is null) return NotFound();

        _db.DashboardWidgets.Remove(widget);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static WidgetDto ToDto(DashboardWidget w) => new()
    {
        Id = w.Id,
        Type = w.Type,
        Title = w.Title,
        Size = w.Size,
        Layout = new LayoutDto { X = w.LayoutX, Y = w.LayoutY, W = w.LayoutW, H = w.LayoutH },
        Config = JsonSerializer.Deserialize<JsonElement>(w.ConfigJson),
    };
}
