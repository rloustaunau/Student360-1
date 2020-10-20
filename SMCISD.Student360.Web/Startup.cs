using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMCISD.Student360.Api.Infrastructure.IoC;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using SMCISD.Student360.Web.Filters;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
namespace SMCISD.Student360.Web
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        public Startup(IHostEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IoCConfig.RegisterDependencies(services, Configuration);
            services.AddScoped<SecurityActionFilter>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration["Jwt:Issuer"],
                       ValidAudience = Configuration["Jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                   };
               });


            //services.AddControllers().AddNewtonsoftJson();

            services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                // By Convention we are securring every endpoint.
                o.Filters.Add(new AuthorizeFilter(policy));
            }).AddMvcOptions(options => {
                options.Filters.Add(new SecurityActionFilter());
            });

            services.AddCors(o => o.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            //Register Bold license
            Bold.Licensing.BoldLicenseProvider.RegisterLicense(Configuration["BoldReports:LicenseToken"]); 

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowAllOrigins");

            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;

                        await context.Response.WriteAsync(new ErrorDto()
                        {
                            Code = getCode(ex),
                            Message = ex.Message // or your custom message
                            // other custom data
                        }.ToString(), Encoding.UTF8);
                    }
                });
            });

            if (!env.IsDevelopment())
                app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
        private int getCode(Exception ex)
        {
            if (ex is UnauthorizedAccessException)
                return 403;
            else if (ex is FormatException)
                return 400;

            return 500;
        }
    }
    class ErrorDto
    {
        public int Code { get; set; }
        public string Message { get; set; }

        // other fields

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
