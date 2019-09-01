using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RFID.REST.Areas.Administration.Models;
using RFID.REST.Areas.Auth.Models;
using RFID.REST.Database;

namespace RFID.REST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            this.ConfigureJwtService(services);
            this.ConfigureRfidServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = c =>
                    {
                        var feature = c.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = feature.Error;

                        return Task.CompletedTask;
                    }
                });
            }
        }

        private AuthSettings CreateAuthSettings()
        {
            var authSection = this.Configuration.GetSection("AuthSettings");
            var secret = authSection.GetValue<String>("Secret");
            var issuer = authSection.GetValue<String>("Issuer");
            var audience = authSection.GetValue<String>("Audience");
            var accessExp = authSection.GetValue<int>("AccessExpiration");
            var refreshExp = authSection.GetValue<int>("RefreshExpiration");

            return new AuthSettings(
                secret: secret,
                issuer: issuer,
                audience: audience,
                accessExpiration: TimeSpan.FromMinutes(accessExp),
                refreshExpiration: TimeSpan.FromMinutes(refreshExp)
            );
        }

        private void ConfigureRfidServices(IServiceCollection services)
        {
            // database
            var connectionString = SqlConnectionString.Create(this.Configuration.GetConnectionString("Rfid"));
            services.AddSingleton(connectionString);
            services.AddSingleton<SqlConnectionFactory>();
            services.AddSingleton<Database.Database>();

            // administration
            var passwordHasher = new PasswordHasher<AdministrationUser>();
            services.AddSingleton<IPasswordHasher<AdministrationUser>>(passwordHasher);
            services.AddSingleton<Areas.Administration.Commands.CommandFactory>();

            // auth
            services.AddSingleton<Areas.Auth.Services.Auth>();

            // access control
        }

        private void ConfigureJwtService(IServiceCollection services)
        {
            var authSettings = this.CreateAuthSettings();
            services.AddSingleton(authSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(authSettings.SecretBytes),
                    // ValidIssuer = authSettings.Issuer,
                    // ValidAudience = authSettings.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
