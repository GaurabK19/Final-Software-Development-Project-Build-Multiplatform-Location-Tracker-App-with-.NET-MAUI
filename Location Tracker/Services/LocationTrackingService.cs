using Microsoft.Maui.Devices.Sensors;       // Geolocation APIs
using System.Threading;                      // CancellationTokenSource
using System.Threading.Tasks;                // Task, async/await
using Location_Tracker.Models;               // LocationPoint

namespace Location_Tracker.Services;

public class LocationTrackingService
{
    private readonly LocationDatabase _database;
    private CancellationTokenSource? _cts;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    public LocationTrackingService(LocationDatabase database)
    {
        _database = database;
    }

    public async Task StartAsync()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
            return;

        _cts = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            while (!_cts!.IsCancellationRequested)
            {
                try
                {
                    var request = new GeolocationRequest(
                        GeolocationAccuracy.Best,
                        TimeSpan.FromSeconds(10));

                    var location = await Geolocation.GetLocationAsync(request, _cts.Token);

                    if (location != null)
                    {
                        await _database.AddLocationAsync(new LocationPoint
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            TimestampUtc = DateTime.UtcNow
                        });
                    }
                }
                catch
                {
                    // ignore for now
                }

                await Task.Delay(_interval, _cts.Token);
            }
        }, _cts.Token);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts = null;
    }
}