using MyForum.Domain.Posts;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyForum.Persistence.Entities
{
    /// <summary>
    /// Модель хранения поста в реляционной базе данных.
    /// Никогда не используется напрямую как доменный агрегат, служит только для проекции данных в EF Core.
    /// </summary>
    [Table(TableName)]
    public sealed class PostEntity : IPostData
    {
        /// <summary>
        /// Имя таблицы в базе данных.
        /// </summary>
        public const string TableName = "Posts";

        /// <summary>
        /// Внутренний суррогатный первичный ключ (скрыт от внешнего API).
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Публичный идентификатор поста для внешних ссылок и API.
        /// </summary>
        public Guid PublicId { get; set; }

        /// <inheritdoc />
        public Guid AuthorPublicId { get; set; }

        /// <inheritdoc />
        public Guid? CommunityPublicId { get; set; }

        /// <inheritdoc />
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc />
        public string ContentJson { get; set; } = string.Empty;

        /// <inheritdoc />
        public PostStatus Status { get; set; }

        /// <inheritdoc />
        public PostFlags Flags { get; set; }

        /// <inheritdoc />
        public int Rating { get; set; }

        /// <inheritdoc />
        public int ViewsCount { get; set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAtUtc { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? PublishedAtUtc { get; set; }
    }
}
