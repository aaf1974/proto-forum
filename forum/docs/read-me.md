миграции
* создание:
- "ASPNETCORE_ENVIRONMENT": "Development"
- "YOURCOMPANY_INFRA_OBJECT_NAMES_PREFIX": "dev_"
- dotnet ef migrations add InitialPosts --project forum/MyForum.Persistence.PostgreSQL --startup-project forum/MyForum.Api

* применение:
- dotnet ef database update --project forum/MyForum.Persistence.PostgreSQL --startup-project forum/MyForum.Api
* создание БД:
```sql
CREATE DATABASE "myforum.dev"
    WITH OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'C'
    LC_CTYPE = 'C'
    TEMPLATE = template0;
```