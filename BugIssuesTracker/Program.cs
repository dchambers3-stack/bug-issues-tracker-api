using BugIssuesTrackerApi.BugIssuesTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BugIssuesTrackerApi.BugIssuesTracker
{
    public class Program
    {
        private const string AngularClientPolicy = "AngularClient";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<BugIssuesTrackerContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("Default"))
            );

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    AngularClientPolicy,
                    policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors(AngularClientPolicy);

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
