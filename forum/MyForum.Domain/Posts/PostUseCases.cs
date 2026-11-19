namespace MyForum.Domain.Posts
{
    /// <summary>
    /// Параметры сценария создания нового черновика поста.
    /// Наследуется от <see cref="EventArgs"/> для передачи внутрь транзакционного коллбэка агрегата.
    /// </summary>
    /// <param name="AuthorPublicId">Публичный идентификатор автора, создающего пост.</param>
    public sealed class CreatePostDraftUseCase(Guid AuthorPublicId, Guid? CommunityPublicId, string? Title, string ContentJson, PostFlags Flags, DateTimeOffset NowUtc) : EventArgs
    {
        /// <summary>
        /// Публичный идентификатор автора, создающего пост.
        /// </summary>
        public Guid AuthorPublicId { get; } = AuthorPublicId;
        public Guid? CommunityPublicId { get; internal set; }
        public string? Title { get; internal set; }
        public string ContentJson { get; internal set; }
        public PostFlags Flags { get; internal set; }
        public DateTimeOffset NowUtc { get; internal set; }
    }

    /// <summary>
    /// Параметры сценария публикации поста из черновика.
    /// </summary>
    /// <param name="ActorPublicId">Публичный идентификатор пользователя, инициировавшего публикацию.</param>
    public sealed class PublishPostUseCase(Guid ActorPublicId, bool RequiresPreModeration, DateTimeOffset? NowUtc = null) : EventArgs
    {
        /// <summary>
        /// Публичный идентификатор пользователя, инициировавшего публикацию.
        /// </summary>
        public Guid ActorPublicId { get; } = ActorPublicId;
        public bool RequiresPreModeration { get; internal set; }
        public DateTimeOffset? NowUtc { get; internal set; }
    }

    /// <summary>
    /// Параметры сценария голосования (изменения рейтинга) за пост.
    /// </summary>
    /// <param name="VoterPublicId">Публичный идентификатор голосующего пользователя.</param>
    /// <param name="Delta">Изменение рейтинга (+1 за плюс, -1 за минус).</param>
    public sealed class VotePostUseCase(Guid VoterPublicId, int Delta) : EventArgs
    {
        /// <summary>
        /// Публичный идентификатор голосующего пользователя.
        /// </summary>
        public Guid VoterPublicId { get; } = VoterPublicId;

        /// <summary>
        /// Изменение рейтинга (+1 за плюс, -1 за минус).
        /// </summary>
        public int Delta { get; } = Delta;
    }
}
