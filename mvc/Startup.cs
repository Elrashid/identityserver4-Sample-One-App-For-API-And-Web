using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace App2
{
    public class Startup
    {


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ////////////////////////////////
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                // Notice the schema name is case sensitive [ cookies != Cookies ]
                options.DefaultScheme = "cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            //https://github.com/leastprivilege/AspNetCoreSecuritySamples/blob/aspnetcore21/OidcAndApi/src/AspNetCoreSecurity/Startup.cs
            .AddCookie("cookies", options => 
                     options.ForwardDefaultSelector = 
                       ctx => 
                       ctx.Request.Path.StartsWithSegments("/api") ? "jwt" : "cookies")
            .AddJwtBearer("jwt", options =>
            {
                options.Authority = "http://localhost:5010";
                options.Audience = "app2api";
                options.RequireHttpsMetadata = false;
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "cookies";
                options.Authority = "http://localhost:5010";
                options.RequireHttpsMetadata = false;
                options.ClientId = "mvc";
                options.SaveTokens = true;

                options.ClientSecret = "secret";
                options.ResponseType = "code id_token";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("app2api");
                options.Scope.Add("offline_access");
                //https://github.com/leastprivilege/AspNetCoreSecuritySamples/blob/aspnetcore21/OidcAndApi/src/AspNetCoreSecurity/Startup.cs
                options.ForwardDefaultSelector = ctx => ctx.Request.Path.StartsWithSegments("/api") ? "jwt" : "oidc";
            });
            /////////////////////////////////
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            /////////////////////////////////
            app.UseAuthentication();
            /////////////////////////////////
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvcWithDefaultRoute();
        }
    }
}