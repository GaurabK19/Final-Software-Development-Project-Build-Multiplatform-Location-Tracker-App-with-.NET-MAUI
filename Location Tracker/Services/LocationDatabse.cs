using SQLite;
using Location_Tracker.Models;

namespace Location_Tracker.Services;

public class LocationDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public LocationDatabase()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "locations.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<LocationPoint>().Wait();
    }

    public Task<int> AddLocationAsync(LocationPoint point)
        => _database.InsertAsync(point);

    public Task<List<LocationPoint>> GetAllLocationsAsync()
        => _database.Table<LocationPoint>()
                    .OrderBy(p => p.TimestampUtc)
                    .ToListAsync();

    public Task<int> DeleteAllAsync()
        => _database.DeleteAllAsync<LocationPoint>();
}