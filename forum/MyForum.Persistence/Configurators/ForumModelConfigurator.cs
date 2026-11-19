using Microsoft.EntityFrameworkCore;
using MyForum.Persistence.Entities;
using YourCompany.OLTP.RecordsManagement.DI.EFCore;

namespace MyForum.Persistence.Configurators
{
    /// <summary>
    /// Плагин-конфигуратор модели EF Core, регистрирующий таблицы и индексы домена форума.
    /// Автоматически подхватывается загрузчиком контекста <see cref="YourCompanyDbContext{TConfiguration}"/>.
    /// </summary>
    public sealed class ForumModelConfigurator : YourCompanyDbContextConfigurator
    {
        /// <inheritdoc />
        public override void OnModelCreating(
            YourCompany.Configuration.EFCore.YourCompanyDbContextConfiguratorsLoadingContext context,
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();

                // Уникальный индекс по публичному ключу для быстрого поиска через Identity.FromPublic(guid)
                entity.HasIndex(p => p.PublicId).IsUnique();

                entity.Property(p => p.Title).HasMaxLength(300).IsRequired();
                entity.Property(p => p.ContentJson).HasColumnType("jsonb").IsRequired();

                // Индекс для быстрой курсорной пагинации лент («Свежее», «Горячее»)
                entity.HasIndex(p => new { p.Status, p.PublishedAtUtc, p.Id });
            });
        }
    }
}
