using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using WebApplication1.Services;
using WebApplication1.Models;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();          //Adiciona os controllers do Projeto
            builder.Services.AddScoped<UserService>(); //Adiciona a funcao (service) de Registracao
            builder.Services.AddScoped<IUsersDatabaseSettings, UsersDatabaseSettings>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            
            
            app.MapControllers();//Mapear os controllers do meu app


            app.Run();
        }
    }
}