using System;
using System.Collections.Generic;
using System.Text;
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
                .AddJsonFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("mypolicy", policy =>
                {
                    policy.RequireClaim("myclaim");
                });
            });

            return services;
        }

        //public static void Configure(IApplicationBuilder app)
        //{
        //    //app.UseMvc();
        //}
    }
}
