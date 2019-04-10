using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AspNetCoreTemplateExtended.Config;
using AspNetCoreTemplateExtended.Config.Options;
using AspNetCoreTemplateExtended.Data;
using AspNetCoreTemplateExtended.Data.Entities;
using AspNetCoreTemplateExtended.Middleware;
using Autofac;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCoreTemplateExtended
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(Configuration.GetConnectionString("Default")));

      services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

      JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
      services
        .AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(cfg =>
        {
          cfg.RequireHttpsMetadata = false;
          cfg.SaveToken = true;
          cfg.TokenValidationParameters = new TokenValidationParameters
          {
            ValidIssuer = Configuration["Auth:Issuer"],
            ValidAudience = Configuration["Auth:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Key"])),
            ClockSkew = TimeSpan.Zero // remove delay of token when expire
          };
        });

      services
        .AddMvc()
        .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true)
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info { Title = "AspNetCoreTemplateExtended API", Version = "v1" });
      });

      services.AddSpaStaticFiles(configuration => { configuration.RootPath = "client-app/build"; });

      services.Configure<AuthOptions>(Configuration.GetSection("Auth"));
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
      builder.RegisterModule(new AutofacModule());
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, AppDbContext dbContext)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
      }

      app.UseMiddleware<ErrorHandlingMiddleware>();
      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();
      app.UseAuthentication();
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCoreTemplateExtended API V1"));
      app.UseMvc(routes => routes.MapRoute("default", "{controller}/{action=Index}/{id?}"));
      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "client-app";

        if (env.IsDevelopment())
        {
          spa.UseReactDevelopmentServer(npmScript: "start");
        }
      });

      dbContext.Database.EnsureCreated();
    }
  }
}