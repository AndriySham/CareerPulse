using CareerPulse.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Tests.TestHelpers;

public static class TestDbContext
{
    public static CareerPulseDbContext CreateInMemory()
    {
        var options = new DbContextOptionsBuilder<CareerPulseDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CareerPulseDbContext(options);
    }
}
