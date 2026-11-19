namespace MyForum.Domain.Posts
{
    public interface IPostData
    {
        Guid AuthorPublicId { get; set; }
        Guid? CommunityPublicId { get; set; } // Лига / Сообщество (если есть)

        string Title { get; set; }
        string ContentJson { get; set; }      // Блоки редактора в JSON

        PostStatus Status { get; set; }
        PostFlags Flags { get; set; }

        int Rating { get; set; }
        int ViewsCount { get; set; }

        DateTimeOffset CreatedAtUtc { get; set; }
        DateTimeOffset? PublishedAtUtc { get; set; }
    }
}
