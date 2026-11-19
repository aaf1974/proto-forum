using MyForum.Persistence;

namespace MyForum.Api;

/// <summary>
/// Фабрика контекста времени разработки (Design-Time) для генерации и применения миграций PostgreSQL.
/// </summary>
public sealed class MyForumPostgreSqlDesignTimeFactory
    : YourCompany.OLTP.RecordsManagement.Persistence.Linq.EFCore.YourCompanyDbContextFactory<MyForumDbContextConfiguration>.CustomConfigurationDesignTimeForMigrations
{
}