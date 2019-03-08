using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkietbaanBE.Helper;
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
            services.AddMvc();
            services.AddDbContext<ModelsContext>
            (options => options.UseSqlServer(Configuration.GetConnectionString("SkietbaanDatabase")));
            services.AddScoped<NotificationMessages>();
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
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (apiDesc.HttpMethod == null) return false;
                    return true;
                });
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

            var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetService<ModelsContext>();

            new ScheduleJob(context);

            if (!true) {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
        }
    }
}
