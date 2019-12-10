using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
using RFID.REST.Common;
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
            ConfigureAuthorizationPolicies(services);
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionMiddleware();
            app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseMvc();
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
            var adminPasswordHasher = new PasswordHasher<AdministrationUser>();
            services.AddSingleton<IPasswordHasher<AdministrationUser>>(adminPasswordHasher);
            services.AddSingleton<Areas.Administration.Commands.CommandFactory>();

            // auth
            services.AddSingleton<Areas.Auth.Services.Auth>();
            var authPasswordHasher = new PasswordHasher<AuthUser>();
            services.AddSingleton<IPasswordHasher<AuthUser>>(authPasswordHasher);
            services.AddSingleton<Areas.Auth.Commands.CommandFactory>();

            // log
            services.AddSingleton<Areas.Log.Commands.CommandFactory>();

            // access control
            services.AddSingleton<Areas.AccessControl.Commands.CommandFactory>();
        }

        private void ConfigureJwtService(IServiceCollection services)
        {
            var authSettings = this.CreateAuthSettings();
            services.AddSingleton(authSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = authSettings.CreateTokenValidationParameters();
            });
        }

        private static void ConfigureAuthorizationPolicies(IServiceCollection services)
        {
            services.AddAuthorization(opt => 
            {
                opt.AddPolicy(PolicyNames.AdminPolicy, b => b.RequireClaim(ClaimTypes.Role, $"{UserRoles.Admin}"));
            });
        }
    }
}
