﻿using System;
using System.IO;
using ClassLibrary1.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClassLibrary1
{
    public static class ClassLibrary1Configuration
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                // https://stackoverflow.com/a/53314810
                .AddApplicationPart(typeof(CustomersController).Assembly)
                .AddAuthorization(options =>
                {
                    options.AddPolicy("mypolicy", policy =>
                    {
                        policy.RequireClaim("myclaim");
                    });
                })
                .AddJsonFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return services;
        }

        public static void Configure(IApplicationBuilder app, Func<IApplicationBuilder, IApplicationBuilder> configureHost)
        {
            configureHost(app)
                .UseMvc();
        }
    }
}
