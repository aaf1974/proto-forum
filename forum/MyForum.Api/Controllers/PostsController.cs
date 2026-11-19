using Microsoft.AspNetCore.Mvc;
using MyForum.Api.Contracts.Posts;
using MyForum.Domain.Posts;
using YourCompany.OLTP.RecordsManagement;
using YourCompany.OLTP.RecordsManagement.Persistence;

namespace MyForum.Api.Controllers;

/// <summary>
/// API-контроллер для управления жизненным циклом постов и голосованием.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class PostsController : ControllerBase
{
    private readonly IRepository<Post> _postRepository;

    /// <summary>
    /// Инициализирует контроллер с внедренным репозиторием агрегата <see cref="Post"/>.
    /// </summary>
    /// <param name="postRepository">Транзакционный репозиторий постов.</param>
    public PostsController(IRepository<Post> postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    /// <summary>
    /// Создает новый черновик поста.
    /// </summary>
    /// <param name="request">Данные нового поста.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Публичный идентификатор созданного черновика.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDraft(
        [FromBody] CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        var postPublicId = Guid.NewGuid();
        var postIdentity = new UniqueKey.Primary.PublicGuidAccompanied.EqualsOnly(postPublicId);

        var useCase = new CreatePostDraftUseCase(
            AuthorPublicId: request.AuthorPublicId,
            CommunityPublicId: request.CommunityPublicId,
            Title: request.Title,
            ContentJson: request.ContentJson,
            Flags: request.Flags,
            NowUtc: DateTimeOffset.UtcNow);

        var transaction = await _postRepository.AccessRecordsBatch(cancellationToken);
        await transaction
            .WithId(postIdentity)
            .TriggerUseCase(useCase, postIdentity)
            .Run(cancellationToken);

        return CreatedAtAction(
            nameof(CreateDraft),
            new { publicId = postPublicId },
            new { PublicId = postPublicId });
    }

    /// <summary>
    /// Публикует ранее созданный черновик поста.
    /// </summary>
    /// <param name="publicId">Публичный идентификатор поста.</param>
    /// <param name="request">Параметры публикации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    [HttpPost("{publicId:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Publish(
        Guid publicId,
        [FromBody] PublishPostRequest request,
        CancellationToken cancellationToken)
    {
        var postIdentity = new UniqueKey.Primary.PublicGuidAccompanied.EqualsOnly(publicId);
        var useCase = new PublishPostUseCase(
            ActorPublicId: request.ActorPublicId,
            RequiresPreModeration: request.RequiresPreModeration,
            NowUtc: DateTimeOffset.UtcNow);

        var transaction = await _postRepository.AccessRecordsBatch(cancellationToken);
        await transaction
            .WithId(postIdentity)
            .TriggerUseCase(useCase, postIdentity)
            .Run(cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Регистрирует голос (+1 или -1) за опубликованный пост.
    /// </summary>
    /// <param name="publicId">Публичный идентификатор поста.</param>
    /// <param name="request">Данные голоса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    [HttpPost("{publicId:guid}/vote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Vote(
        Guid publicId,
        [FromBody] VotePostRequest request,
        CancellationToken cancellationToken)
    {
        var postIdentity = new UniqueKey.Primary.PublicGuidAccompanied.EqualsOnly(publicId);
        var useCase = new VotePostUseCase(request.VoterPublicId, request.Delta);

        var transaction = await _postRepository.AccessRecordsBatch(cancellationToken);
        await transaction
            .WithId(postIdentity)
            .TriggerUseCase(useCase, postIdentity)
            .Run(cancellationToken);

        return Ok();
    }
}
