
using Microsoft.OpenApi.Models;

namespace WebAPI_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V10", new OpenApiInfo
                {
                    Title = "My Custom API",
                    Version = "V10",
                    Description = "A Brief Description of My APIs",
                    TermsOfService = new Uri("https://dotnettutorials.net/privacy-policy/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Support",
                        Email = "support@dotnettutorials.net",
                        Url = new Uri("https://dotnettutorials.net/contact/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use Under XYZ",
                        Url = new Uri("https://dotnettutorials.net/about-us/")
                    }
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/V10/swagger.json", "My API V10");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
