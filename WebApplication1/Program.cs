using MongoDB.Driver;
using WebApplication1.Services;
using WebApplication1.Models;
using System.Text.Json.Serialization;
using WebApplication1.Authorization;
using WebApplication1.Helpers;
using WebApplication1.Exceptions;
using WebApplication1.Interfaces;
using WebApplication1.Models.Email;
using WebApplication1.Services.EmailServices;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.           
        builder.Services.AddRazorPages();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Serialize enums as strings in API responses (e.g., Role)
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                // Add custom converter for System.DateOnly
                options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
            });

            //Scopes
            builder.Services.AddSingleton<Utils>();            
            
            builder.Services.AddSingleton<ITaskService, TaskService>();
            builder.Services.AddSingleton<IUserService, UserService>();            
            builder.Services.AddSingleton<IProjectService, ProjectService>();
            
            builder.Services.AddSingleton<IJwtUtils, JwtUtils>();
            builder.Services.AddSingleton<IUsersDatabaseSettings, UsersDatabaseSettings>();

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            
            builder.Services.AddSingleton<IEmailService, EmailService>();

            builder.Services.AddHostedService<BackgroundEmailService>();

            var settings = builder.Configuration.GetSection("UsersDatabaseSettings").Get<AppSettings>();
            
            var dbName = settings.DatabaseName;
            var connectionString = settings.ConnectionString;
            
            //Configuring the Mongodb collections
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(dbName);

            //Users collection
            var usersCollection = db.GetCollection<User>("Users");
            builder.Services.AddSingleton<IMongoDatabase>(db);
            builder.Services.AddSingleton<IMongoCollection<User>>(usersCollection);

            //Projects collection
            var projectsCollection = db.GetCollection<Project>("Projects");
            builder.Services.AddSingleton<IMongoCollection<Project>>(projectsCollection);

            builder.Services.AddAuthorization();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<JwtMiddleware>();

            if(app.Environment.IsDevelopment()) 
            {               
                app.UseHttpsRedirection(); 
            }
        
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