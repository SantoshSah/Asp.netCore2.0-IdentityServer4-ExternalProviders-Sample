using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SellerSystem.Data;
using SellerSystem.Services;
using IdentityServer4.AccessTokenValidation;
using Autofac;
using SellerSystem.BLL.Service;
using SellerSystem.BLL.Service.Interface;
using Autofac.Extensions.DependencyInjection;
using SellerSystem.Helpers;
using IdentityServer4;


namespace SellerSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {

                options.AddPolicy("CORS_POLICY",
                    policy => policy.WithOrigins("http://localhost:3000").AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
                );

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddIdentityServer();

           
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });

           

            //services.AddCors();

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddIdentityServer()
                        .AddDeveloperSigningCredential(filename: "tempkey.rsa")
                        .AddInMemoryApiResources(Config.GetApiResources())
                        .AddInMemoryIdentityResources(Config.GetIdentityResources())
                        .AddInMemoryClients(Config.GetClients())
                        //.AddTestUsers(Config.GetUsers())
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddExtensionGrantValidator<GoogleGrant>()
                    .AddExtensionGrantValidator<FacebookGrant>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.JwtAuthenticationScheme)
                        .AddIdentityServerAuthentication(options =>
                        {
                            options.Authority = "http://localhost:5000"; // Auth Server
                            //options.Authority = "http://localhost:44391";
                            options.RequireHttpsMetadata = false;
                            options.ApiName = "allowedscopeapi"; // API Resource Id
                            //options.
                        });

            /*
			services.AddAuthentication()
        	.AddGoogle("Google", options =>
        	{
        		options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

        		options.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
        		options.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
        	});
            */

            // Create the container builder.
            var builder = new ContainerBuilder();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.Populate(services);
            this.ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CORS_POLICY");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
