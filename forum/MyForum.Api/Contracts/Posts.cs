using MyForum.Domain.Posts;

namespace MyForum.Api.Contracts.Posts;

/// <summary>
/// Модель входящего HTTP-запроса на создание черновика поста.
/// </summary>
/// <param name="AuthorPublicId">Публичный идентификатор автора.</param>
/// <param name="Title">Заголовок поста.</param>
/// <param name="ContentJson">Содержимое поста из блочного редактора в формате JSON.</param>
/// <param name="Flags">Специальные атрибуты контента (NSFW, Жесть, Авторское и др.).</param>
/// <param name="CommunityPublicId">Необязательный публичный идентификатор сообщества (лиги).</param>
public sealed record CreatePostRequest(
    Guid AuthorPublicId,
    string Title,
    string ContentJson,
    PostFlags Flags = PostFlags.None,
    Guid? CommunityPublicId = null);

/// <summary>
/// Модель входящего HTTP-запроса на публикацию черновика поста.
/// </summary>
/// <param name="ActorPublicId">Публичный идентификатор пользователя, публикующего пост.</param>
/// <param name="RequiresPreModeration">Признак необходимости предварительной проверки модератором.</param>
public sealed record PublishPostRequest(
    Guid ActorPublicId,
    bool RequiresPreModeration = false);

/// <summary>
/// Модель входящего HTTP-запроса на изменение рейтинга поста (голосование).
/// </summary>
/// <param name="VoterPublicId">Публичный идентификатор голосующего пользователя.</param>
/// <param name="Delta">Значение голоса (+1 за плюс, -1 за минус).</param>
public sealed record VotePostRequest(
    Guid VoterPublicId,
    int Delta);

