using YourCompany.OLTP.StateOwnership;

namespace MyForum.Domain.Posts;

/// <summary>
/// Доменный агрегат поста. Инкапсулирует жизненный цикл публикации и правила изменения рейтинга.
/// Реагирует на доменные сценарии (<see cref="EventArgs"/>) внутри транзакционного замыкания.
/// </summary>
public sealed class Post
{
    private readonly IStateAccess<IPostData> _stateAccess;

    /// <summary>
    /// Инициализирует агрегат поста и регистрирует обработчики транзакционных сценариев.
    /// </summary>
    /// <param name="stateAccess">Механизм изолированного доступа к состоянию записи.</param>
    public Post(IStateAccess<IPostData> stateAccess)
    {
        _stateAccess = stateAccess ?? throw new ArgumentNullException(nameof(stateAccess));

        // Регистрируем единое замыкание на все модифицирующие сценарии этого агрегата
        _stateAccess.Claim(OnTransactionCallback);
    }

    /// <summary>
    /// Актуальные данные поста после завершения транзакции.
    /// </summary>
    public IPostData? Data => _stateAccess.ReadOnly.DataAfterAccess;

    private void OnTransactionCallback(object? sender, EventArgs e)
    {
        var modifying = (IStateModifying<IPostData>)sender!;

        // Выполняем бизнес-логику только в фазе, когда запись заблокирована в БД (SELECT ... FOR UPDATE)
        if (modifying.LockedRecordData is null)
            return;

        switch (e)
        {
            case CreatePostDraftUseCase createCmd:
                HandleCreateDraft(modifying, createCmd);
                break;

            case PublishPostUseCase publishCmd:
                HandlePublish(modifying, publishCmd);
                break;

            case VotePostUseCase voteCmd:
                HandleVote(modifying, voteCmd);
                break;
        }
    }

    private static void HandleCreateDraft(IStateModifying<IPostData> modifying, CreatePostDraftUseCase cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.Title))
            throw new ArgumentException("Заголовок поста не может быть пустым.");

        if (modifying.LockedRecordData.Status != PostStatus.None)
            throw new InvalidOperationException("Пост с таким идентификатором уже создан.");

        var target = modifying.SettingDataProperties;
        target.AuthorPublicId = cmd.AuthorPublicId;
        target.CommunityPublicId = cmd.CommunityPublicId;
        target.Title = cmd.Title.Trim();
        target.ContentJson = cmd.ContentJson;
        target.Flags = cmd.Flags;
        target.Status = PostStatus.Draft;
        target.CreatedAtUtc = cmd.NowUtc;
    }

    private static void HandlePublish(IStateModifying<IPostData> modifying, PublishPostUseCase cmd)
    {
        var current = modifying.LockedRecordData;

        if (current.Status == PostStatus.None)
            throw new InvalidOperationException("Пост не найден.");

        if (current.AuthorPublicId != cmd.ActorPublicId)
            throw new InvalidOperationException("Опубликовать пост может только его автор.");

        if (current.Status != PostStatus.Draft)
            throw new InvalidOperationException($"Нельзя опубликовать пост в статусе {current.Status}.");

        var target = modifying.SettingDataProperties;
        target.Status = cmd.RequiresPreModeration ? PostStatus.PreModeration : PostStatus.Published;

        if (!cmd.RequiresPreModeration)
            target.PublishedAtUtc = cmd.NowUtc;
    }

    private static void HandleVote(IStateModifying<IPostData> modifying, VotePostUseCase cmd)
    {
        if (cmd.Delta is not (1 or -1))
            throw new ArgumentOutOfRangeException(nameof(cmd.Delta), "Голос может быть только +1 или -1.");

        var current = modifying.LockedRecordData;

        if (current.Status != PostStatus.Published)
            throw new InvalidOperationException("Голосовать можно только за опубликованные посты.");

        if (current.Flags.HasFlag(PostFlags.NoRating))
            throw new InvalidOperationException("Этот пост создан без рейтинга.");

        modifying.SettingDataProperties.Rating = current.Rating + cmd.Delta;
    }
}
