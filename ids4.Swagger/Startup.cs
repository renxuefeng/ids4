using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using ids4.Common.Configuration;
using ids4.Model.Entitys;
using ids4.Repository;
using ids4.Swagger.Configuration.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ids4.Swagger
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
            services.AddControllers();
            var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
            services.AddSingleton(adminApiConfiguration);
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            //services.AddIdentity<UserInfo, UserIdentityRole>(options => { options.User.RequireUniqueEmail = true; })
            //    //.AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            })
            //.AddJwtBearer("Bearer", options =>
            //{
            //    //identityserver4 地址 也就是本项目地址
            //    options.Authority = "http://localhost:5000";
            //    options.RequireHttpsMetadata = false;
            //    options.Audience = "http://localhost:5000/resources";
            //    //options.TokenValidationParameters = new TokenValidationParameters
            //    //{
            //    //    ValidateAudience = true,
            //    //    //ValidIssuer = audienceConfiguration.Issuer,
            //    //    //ValidateIssuer = true,
            //    //    //IssuerSigningKeys = new List<SecurityKey> { keyByteArray },
            //    //    //ValidateIssuerSigningKey = true,
            //    //    //ValidateLifetime = true,
            //    //};
            //});
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = adminApiConfiguration.IdentityServerBaseUrl;
                options.ApiName = adminApiConfiguration.OidcApiName;//"http://localhost:5000/resources";
                options.RequireHttpsMetadata = adminApiConfiguration.RequireHttpsMetadata;
            });
            services.AddAuthorization();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        if (adminApiConfiguration.CorsAllowAnyOrigin)
                        {
                            builder.AllowAnyOrigin();
                        }
                        else
                        {
                            builder.WithOrigins(adminApiConfiguration.CorsAllowOrigins);
                        }

                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(adminApiConfiguration.ApiVersion, new OpenApiInfo { Title = adminApiConfiguration.ApiName, Version = adminApiConfiguration.ApiVersion });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{adminApiConfiguration.IdentityServerBaseUrl}/connect/authorize"),
                            Scopes = new Dictionary<string, string> {
                                { adminApiConfiguration.OidcApiName, adminApiConfiguration.ApiName },
                                { "sss", adminApiConfiguration.ApiName },
                            }
                        }
                    }
                });
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            //.AddIdentityServerAuthentication(x =>
            // {
            //     x.Authority = "http://localhost:5000/resources";//adminApiConfiguration.IdentityServerBaseUrl;//鉴权服务地址
            //     x.RequireHttpsMetadata = false;
            //     x.ApiName = adminApiConfiguration.OidcApiName;//鉴权范围
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AdminApiConfiguration adminApiConfiguration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{adminApiConfiguration.ApiBaseUrl}/swagger/v1/swagger.json", adminApiConfiguration.ApiName);

                c.OAuthClientId(adminApiConfiguration.OidcSwaggerUIClientId);
                c.OAuthAppName(adminApiConfiguration.ApiName);
            });
            app.UseRouting();
            app.UseCors(x =>
            {
                x.AllowAnyOrigin();
                x.AllowAnyMethod();
                x.AllowAnyHeader();
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
