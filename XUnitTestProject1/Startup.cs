using Acheve.AspNetCore.TestHost.Security;
using Acheve.TestHost;
using ClassLibrary1;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTestProject1
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
            ClassLibrary1.ClassLibrary1Configuration.ConfigureServices(services);
            services
                .AddAuthentication(TestServerAuthenticationDefaults.AuthenticationScheme)
                .AddTestServerAuthentication();
            services
                .AddDbContext<ShopContext>(options =>
                {
                    // appsettings.json, Copy to Output Directory: Copy if newer
                    var connectionString = Configuration.GetConnectionString("DefaultConnection");
                    options.UseSqlServer(connectionString);
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            ClassLibrary1Configuration.Configure(app, host =>
            {
                host.UseAuthentication();

                return host;
            });
        }
    }
}
