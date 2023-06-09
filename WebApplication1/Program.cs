using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using WebApplication1.Services;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using WebApplication1.Authorization;
using Microsoft.Extensions.Configuration;
using WebApplication1.Helpers;

namespace WebApplication1
{
    public class Program

    {
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);



        // Add services to the container.           
        builder.Services.AddRazorPages();
            builder.Services.AddControllers().AddJsonOptions(x =>
            {
                // serialize enums as strings in api responses (e.g. Role)
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddScoped<Utils>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<JwtUtils>();
            builder.Services.AddScoped<IUsersDatabaseSettings, UsersDatabaseSettings>();

        builder.Services.AddAuthorization();
            
            var app = builder.Build();

                // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");

                app.UseHsts();
            }

            app.UseMiddleware<JwtMiddleware>();

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
                       
            app.MapControllers();

            app.Run();
        }
    }
}