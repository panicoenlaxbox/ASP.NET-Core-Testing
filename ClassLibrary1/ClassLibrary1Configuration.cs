using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary1
{
    public static class ClassLibrary1Configuration
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddAuthorization(options =>
                {
                    options.AddPolicy("mypolicy", policy =>
                    {
                        policy.RequireClaim("myclaim");
                    });
                })
                .AddJsonFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            return services;
        }

        public static void Configure(IApplicationBuilder app, Func<IApplicationBuilder, IApplicationBuilder> configureHost)
        {
            configureHost(app)
                .UseMvc();
        }
    }
}
