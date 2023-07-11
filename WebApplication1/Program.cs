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
            }); //Adiciona os controllers do Projeto
            builder.Services.AddScoped<UserService>(); //Adiciona a funcao (service) de Registracao
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<JwtUtils>();
            builder.Services.AddScoped<IUsersDatabaseSettings, UsersDatabaseSettings>();

        /*builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });*/

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
                       
            app.MapControllers(); //Mapear os controllers do meu app

            app.Run();
        }
    }
}