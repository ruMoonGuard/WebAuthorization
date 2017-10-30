using System;
using Authorization.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Authorization
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // издатель
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        
                        // потребитель
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
 
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                        
                        ValidateLifetime = true,
                        //5 minute tolerance for the expiration date
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });

            services.AddCors();
            
            services.AddMvc();
            
            services.AddScoped<IDbInitializer, DbInitializer>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDbInitializer dbInitializer)
        {
            app.UseAuthentication();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //dbContext.Database.Migrate(); //this will generate the db if it does not exist
            }

            //Generate EF Core Seed Data
            dbInitializer.Initialize();

            app.UseCors(builder => builder.AllowAnyOrigin());
            
            app.UseMvcWithDefaultRoute();
        }
    }
}