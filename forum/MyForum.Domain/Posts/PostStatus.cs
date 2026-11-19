namespace MyForum.Domain.Posts
{
    public enum PostStatus
    {
        None = 0,          // Значение по умолчанию для еще не созданной записи в БД
        Draft = 1,         // Черновик
        PreModeration = 2, // На проверке у модератора
        Published = 3,     // Опубликован (в «Свежем» / «Горячем»)
        Hidden = 4         // Скрыт автором или удален модератором
    }

    [Flags]
    public enum PostFlags
    {
        None = 0,
        Nsfw = 1 << 0,       // 18+ / Клубничка
        Gore = 1 << 1,       // Жесть
        NoRating = 1 << 2,   // Без рейтинга
        Original = 1 << 3    // Тег [Моё] / Авторский контент
    }
}
