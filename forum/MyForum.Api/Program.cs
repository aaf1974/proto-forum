[assembly: Microsoft.Extensions.Configuration.UserSecrets.UserSecretsId("c0964406-4ee9-4b3c-81ab-5a629397be28")]

namespace MyForum.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
