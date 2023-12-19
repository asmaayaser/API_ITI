using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI
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
            builder.Services.AddSwaggerGen();
            //custom service
            builder.Services.AddDbContext<ITIEntity>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("MyPolicy", CorsPolicyBuilder =>
                {
                    // certain one
                    //CorsPolicyBuilder.WithOrigins("http://www.face.com");

                    // anyone have url  i can response to him
                    //CorsPolicyBuilder.AllowAnyOrigin();

                    //with certain meyhods 
                    // get , post   --put them in array of string  and replace
                    //CorsPolicyBuilder.AllowAnyOrigin().WithMethods("Get");

                    // any method 
                    //CorsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod();

                    // don’t  need a certain header
                    // anyone can access
                    CorsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();


                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //html, css , vidoes, images, extensions
            app.UseStaticFiles();

            //setting cors policy
            app.UseCors("MyPolicy"); // policy block or open 

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}