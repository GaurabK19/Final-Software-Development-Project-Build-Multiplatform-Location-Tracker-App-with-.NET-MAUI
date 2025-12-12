using Microsoft.Maui.Controls;          // Colors
using Microsoft.Maui.Controls.Maps;     // Map, Circle, MapSpan (UI control)
using Microsoft.Maui.Devices.Sensors;   // Location, Distance
using System.Linq;

namespace Location_Tracker.Services;

public class HeatMapService
{
    private readonly LocationDatabase _database;

    public HeatMapService(LocationDatabase database)
    {
        _database = database;
    }

    public async Task RenderAsync(Microsoft.Maui.Controls.Maps.Map map)
    {
        if (map == null)
            return;

        map.MapElements.Clear();

        var points = await _database.GetAllLocationsAsync();

        foreach (var p in points)
        {
            var circle = new Circle
            {
                Center = new Location(p.Latitude, p.Longitude),
                Radius = new Distance(50),
                StrokeWidth = 0,
                StrokeColor = Colors.Transparent,
                FillColor = new Color(1f, 0f, 0f, 0.25f)
            };

            map.MapElements.Add(circle);
        }

        if (points.Any())
        {
            var last = points.Last();
            map.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    new Location(last.Latitude, last.Longitude),
                    Distance.FromKilometers(1)));
        }
    }
}
