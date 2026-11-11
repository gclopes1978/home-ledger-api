using HomeLedger.Infrastructure.Data.Factories;
using HomeLedger.Infrastructure.Data.Interfaces.Locations;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Implementations;

public sealed class LocationData(HomeLedgerDbContext db)
    : DataFactory<LocationEntity>(db), ILocationData
{
}
