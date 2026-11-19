using YourCompany.OLTP.RecordsManagement.DI.EFCore;

namespace MyForum.Persistence
{
    /// <summary>
    /// Маркерная конфигурация контекста базы данных для платформы форума.
    /// Используется фабрикой <see cref="YourCompanyDbContext{TConfiguration}"/> для объединения моделей и плагинов провайдера.
    /// </summary>
    public sealed class MyForumDbContextConfiguration : YourCompanyDbContextConfiguration
    {
    }
}
