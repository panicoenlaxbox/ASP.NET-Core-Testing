using Acheve.AspNetCore.TestHost.Security;
using Acheve.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Infrastructure.Fixtures
{
    class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ApiConfiguration.ConfigureServices(services);
            services
                .AddAuthentication(TestServerDefaults.AuthenticationScheme)
                .AddTestServer();

            services
                .AddDbContext<ShopContext>(options =>
                {
                    // appsettings.json, Copy to Output Directory: Copy if newer
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            ApiConfiguration.Configure(app, host =>
            {
                host.UseAuthentication();

                return host;
            });
        }
    }
}
