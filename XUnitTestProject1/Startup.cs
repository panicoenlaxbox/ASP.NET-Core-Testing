using System;
using System.Collections.Generic;
using System.Text;
using Acheve.AspNetCore.TestHost.Security;
using Acheve.TestHost;
using ClassLibrary1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTestProject1
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ClassLibrary1.ClassLibrary1Configuration.ConfigureServices(services);
            services
                .AddAuthentication(TestServerAuthenticationDefaults.AuthenticationScheme)
                .AddTestServerAuthentication();
            services
                .AddDbContext<FooContext>(options =>
                {
                    options.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=Foo;Trusted_Connection=True;",
                        setup =>
                        {
                            //setup.MigrationsAssembly(typeof(Startup).Assembly.FullName);
                        });
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
