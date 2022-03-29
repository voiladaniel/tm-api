// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="LEGO System A/S">
//   Copyright (c) LEGO System A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the Startup type of the project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TravianManager
{
    using Castle.Facilities.AspNetCore;
    using Castle.Windsor;
    using Data.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Swagger;
    using TravianManager.Data.Sql.Context;
    using WindsorInstaller;

    /// <summary>
    /// The API Startup Configuration.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The container.
        /// </summary>
        private static readonly WindsorContainer Container = new WindsorContainer();

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            //// Retrieve AppSettings.json Authentication section, and map it to a POCO, and make it available in the container
            var appSettings = this.Configuration.GetSection("ApplicationSettings").Get<AppSettingsPoco>();

            Container.AddFacility<AspNetCoreFacility>(f => f.CrossWiresInto(services));

            services.AddCors(options => options.AddPolicy(
                "CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));
            services.AddMvc()
                //// *****To Allow JSON Formatting*****
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            services.AddDbContext<EntityFrameworkDbContext>(options => options.UseSqlServer("Data Source=travian.c4vwhwtbl5zj.eu-central-1.rds.amazonaws.com,1433;Initial Catalog=travian;User ID=admin;pwd=admin12345;MultipleActiveResultSets=True", b => b.MigrationsAssembly("TravianManager")));

            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("TravianManager", new Info { Title = "Packing API" });
            });
            services.AddHttpClient();
            services.AddWindsor(Container);

            Container.Install(new Installer(appSettings));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="env">
        /// The env.
        /// </param>
        /// /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Container.GetFacility<AspNetCoreFacility>().RegistersMiddlewareInto(app);
            loggerFactory.AddLog4Net();
            app.UseCors("CorsPolicy");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/TravianManager/swagger.json", "Packing API");
            });
        }
    }
}
