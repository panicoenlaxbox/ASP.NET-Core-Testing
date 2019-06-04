using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Host
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
            ApiConfiguration.ConfigureServices(services)
                .AddCors()
                .AddDbContext<ShopContext>(options =>
                {
                    options.UseSqlServer(@"Server=.;Database=Shop;Trusted_Connection=True;");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ApiConfiguration.Configure(app, host =>
            {
                if (env.IsDevelopment())
                {
                    host.UseDeveloperExceptionPage();
                }
                else
                {
                    host.UseHsts();
                }

                host.UseHttpsRedirection();

                host.UseAuthentication();

                host.UseCors();
                host.UseStaticFiles();

                return host;
            });
        }
    }
}
