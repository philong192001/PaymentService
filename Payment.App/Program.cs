using Microsoft.OpenApi.Models;
using Payment.App.Extensions;

namespace Payment.App;

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
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"GettingStartedWithSwaggerUI.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        builder.Services.AddHttpClient();

        builder.Services.RegisterService(builder.Configuration);
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Service");
            });
        //}

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseCors();

        app.MapControllers();

        app.Run();
    }
}