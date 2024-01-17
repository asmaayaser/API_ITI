using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
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
            // swager btn
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo", Version = "v1" });
            });

            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 5 Web API",
                    Description = " ITI Projrcy"
                });

                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                });
            });

        //custom service
        builder.Services.AddDbContext<ITIEntity>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });
            builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<ITIEntity>();
            //[Authorize]  use JWT token in check Authantication 
            builder.Services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme   = JwtBearerDefaults.AuthenticationScheme ;
                Options.DefaultChallengeScheme      = JwtBearerDefaults.AuthenticationScheme ;
                Options.DefaultScheme               = JwtBearerDefaults.AuthenticationScheme ; 
            }).AddJwtBearer(Options =>
            {
                Options.SaveToken = true;
                Options.RequireHttpsMetadata = false;
                Options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer=true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidateAudience=true,
                    ValidAudience= builder.Configuration["JWT:ValidAudience"],
                    IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
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

            app.UseAuthentication(); //check JWT token
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}