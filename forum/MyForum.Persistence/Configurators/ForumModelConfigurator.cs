using Microsoft.EntityFrameworkCore;
using MyForum.Domain.Posts;
using MyForum.Persistence.Entities;
//using YourCompany.OLTP.RecordsManagement.DI.EFCore;
using YourCompany.OLTP.RecordsManagement.Persistence.Linq.EFCore;

namespace MyForum.Persistence.Configurators;

/// <summary>
/// Плагин-конфигуратор модели EF Core, регистрирующий таблицы, индексы и доменные агрегаты форума.
/// </summary>
public sealed class ForumModelConfigurator : YourCompany.OLTP.RecordsManagement.Persistence.Linq.EFCore.YourCompanyDbContextConfigurator
{
    /// <summary>
    /// Перечисляет доменные агрегаты этой сборки для автоматической регистрации <c>IRepository&lt;T&gt;</c> в DI.
    /// </summary>
    public override IEnumerable<Type> EnumerateRecordTypes(YourCompanyDbContextConfiguratorsLoadingContext context)
    {
        yield return typeof(Post);
    }

    /// <inheritdoc />
    public override void OnModelCreating(
        YourCompany.Configuration.EFCore.YourCompanyDbContextConfiguratorsLoadingContext context,
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();

            // Уникальный индекс по публичному ключу для быстрого поиска через UniqueKey
            entity.HasIndex(p => p.PublicId).IsUnique();

            entity.Property(p => p.Title).HasMaxLength(300).IsRequired();
            entity.Property(p => p.ContentJson).HasColumnType("jsonb").IsRequired();

            // Индекс для быстрой курсорной пагинации лент («Свежее», «Горячее»)
            entity.HasIndex(p => new { p.Status, p.PublishedAtUtc, p.Id });
        });
    }
}
