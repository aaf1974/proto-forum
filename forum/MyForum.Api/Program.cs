using MyForum.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем классические контроллеры ASP.NET Core
builder.Services.AddControllers();

// Подключаем Swagger для удобного ручного тестирования API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Подключаем фабрику DbContext и провайдер доступа к данным EF Core для нашей конфигурации
builder.Services.AddYourCompanyEFCoreSingleScopedRecordsDataAccessProvider<MyForumDbContextConfiguration>();

// 2. Автоматически регистрируем доменные агрегаты (включая Post) и IRepository<T>
builder.Services.AddYourCompanyEFCoreScopedDbContextFactoryLoadedDomainTypes<MyForumDbContextConfiguration>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Подключаем маршрутизацию на контроллеры
app.MapControllers();

app.Run();