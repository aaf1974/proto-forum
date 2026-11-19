using YourCompany.OLTP.StateOwnership;

namespace MyForum.Domain.Posts
{
    public sealed class Post(IStateAccess<IPostData> stateAccess)
    {
        // Свойство для чтения актуального состояния после завершения транзакции
        public IPostData? Data => stateAccess.ReadOnly.DataAfterAccess;

        // 1. Создание черновика (или нового поста)
        public void CreateDraft(
            Guid authorPublicId,
            string title,
            string contentJson,
            PostFlags flags,
            Guid? communityPublicId,
            DateTimeOffset nowUtc)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Заголовок поста не может быть пустым.", nameof(title));

            Modify(modifying =>
            {
                // Если Status != None, значит пост с таким ID уже существует в БД!
                if (modifying.LockedRecordData.Status != PostStatus.None)
                    throw new InvalidOperationException("Пост с таким идентификатором уже создан.");

                var target = modifying.SettingDataProperties;
                target.AuthorPublicId = authorPublicId;
                target.CommunityPublicId = communityPublicId;
                target.Title = title.Trim();
                target.ContentJson = contentJson;
                target.Flags = flags;
                target.Status = PostStatus.Draft;
                target.CreatedAtUtc = nowUtc;
            });
        }

        // 2. Публикация поста (перевод из черновика в опубликованные)
        public void Publish(Guid actorPublicId, bool requiresPreModeration, DateTimeOffset nowUtc)
        {
            Modify(modifying =>
            {
                var current = modifying.LockedRecordData;

                if (current.Status == PostStatus.None)
                    throw new InvalidOperationException("Пост не найден.");

                if (current.AuthorPublicId != actorPublicId)
                    throw new InvalidOperationException("Опубликовать пост может только его автор.");

                if (current.Status != PostStatus.Draft)
                    throw new InvalidOperationException($"Нельзя опубликовать пост в статусе {current.Status}.");

                var target = modifying.SettingDataProperties;
                target.Status = requiresPreModeration ? PostStatus.PreModeration : PostStatus.Published;

                if (!requiresPreModeration)
                    target.PublishedAtUtc = nowUtc;
            });
        }

        // 3. Изменение рейтинга (голосование +1 / -1)
        public void ApplyVote(int delta)
        {
            if (delta is not (1 or -1))
                throw new ArgumentOutOfRangeException(nameof(delta), "Голос может быть только +1 или -1.");

            Modify(modifying =>
            {
                var current = modifying.LockedRecordData;

                if (current.Status != PostStatus.Published)
                    throw new InvalidOperationException("Голосовать можно только за опубликованные посты.");

                if (current.Flags.HasFlag(PostFlags.NoRating))
                    throw new InvalidOperationException("Этот пост создан без рейтинга.");

                // Шасси подхватит новое значение Rating и обновит только эту колонку в Postgres
                modifying.SettingDataProperties.Rating = current.Rating + delta;
            });
        }

        // Удобная обертка над стандартным EventHandler из IStateAccess
        private void Modify(Action<IStateModifying<IPostData>> action)
        {
            stateAccess.Claim((sender, _) =>
            {
                var modifying = (IStateModifying<IPostData>)sender!;

                // Коллбэк вызывается шасси трижды:
                // 1. До обращения к БД (LockedRecordData == null)
                // 2. Когда строка заблокирована в БД (LockedRecordData != null) -> наш момент истины!
                // 3. В конце транзакции (LockedRecordData == null)
                if (modifying.LockedRecordData is not null)
                {
                    action(modifying);
                }
            });
        }
    }
}
