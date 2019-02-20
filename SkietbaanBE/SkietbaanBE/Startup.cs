﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkietbaanBE.Lib;
using SkietbaanBE.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace SkietbaanBE
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
            services.AddMvc(); services.AddDbContext<ModelsContext>
                 (options => options.UseSqlServer(Configuration.GetConnectionString("SkietbaanDatabase")));
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("Skietbaan", new Info { Title = "Skietbaan API", Version = "v1" });
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Skietbaan/swagger.json", "SkietbaanBE");
            });
            app.UseSwagger();

            if (!true) {
                using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                    var context = scope.ServiceProvider.GetService<ModelsContext>();
                    context.Database.EnsureDeleted();
                    context.Database.Migrate();
                    DataSeeder.Seed(context, true);
                }
            }
        }
    }
}
