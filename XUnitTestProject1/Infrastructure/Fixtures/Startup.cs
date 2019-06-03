using System.Collections;
using System.Collections.Generic;
using Acheve.AspNetCore.TestHost.Security;
using Acheve.TestHost;
using ClassLibrary1;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace XUnitTestProject1.Infrastructure.Fixtures
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
            ClassLibrary1Configuration.ConfigureServices(services);
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
            ClassLibrary1Configuration.Configure(app, host =>
            {
                host.UseAuthentication();

                return host;
            });
        }
    }
}
