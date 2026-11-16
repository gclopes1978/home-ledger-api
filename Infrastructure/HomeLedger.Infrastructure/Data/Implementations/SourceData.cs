using HomeLedger.Infrastructure.Data.Factories;
using HomeLedger.Infrastructure.Data.Interfaces.Sources;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Implementations;

public sealed class SourceData(HomeLedgerDbContext db)
    : DataFactory<SourceEntity>(db), ISourceData
{
}
