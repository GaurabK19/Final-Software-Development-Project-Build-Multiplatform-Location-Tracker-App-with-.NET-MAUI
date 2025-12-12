using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Location_Tracker.Services;

namespace Location_Tracker;

public partial class MainPage : ContentPage
{
    private readonly LocationTrackingService _trackingService;
    private readonly HeatMapService _heatMapService;
    private readonly LocationDatabase _database;

    public MainPage()
    {
        InitializeComponent();

        // Manually create services (simpler than DI for now)
        _database = new LocationDatabase();
        _trackingService = new LocationTrackingService(_database);
        _heatMapService = new HeatMapService(_database);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await EnsureLocationPermissionAsync();
        await _heatMapService.RenderAsync(MainMap);
    }

    private async Task EnsureLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Location Permission Needed",
                "This app needs access to your location to track and display the heat map.",
                "OK");
        }
    }

    private async void OnStartTrackingClicked(object sender, EventArgs e)
    {
        await _trackingService.StartAsync();
        await DisplayAlert("Tracking", "Location tracking started.", "OK");
    }

    private void OnStopTrackingClicked(object sender, EventArgs e)
    {
        _trackingService.Stop();
        DisplayAlert("Tracking", "Location tracking stopped.", "OK");
    }

    private async void OnRefreshHeatmapClicked(object sender, EventArgs e)
    {
        await _heatMapService.RenderAsync(MainMap);
    }

    private async void OnClearDataClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Clear Data",
            "Delete all saved locations?",
            "Yes", "No");

        if (confirm)
        {
            await _database.DeleteAllAsync();
            await _heatMapService.RenderAsync(MainMap);
        }
    }
}