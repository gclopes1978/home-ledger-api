using HomeLedger.Infrastructure.Data.Factories;
using HomeLedger.Infrastructure.Data.Interfaces.Groupings;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Implementations;

public sealed class GroupingData(HomeLedgerDbContext db)
    : DataFactory<GroupingEntity>(db), IGroupingData
{
}
