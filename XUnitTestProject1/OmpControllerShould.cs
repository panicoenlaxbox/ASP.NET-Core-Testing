using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibrary1;
using ClassLibrary1.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;
using XUnitTestProject1.Helpers;
using XUnitTestProject1.Infrastructure.Fixtures;

namespace XUnitTestProject1
{
    [Collection("OmpCollection")]
    public class OmpControllerShould
    {
        private readonly OmpFixture _fixture;
        private readonly ITestOutputHelper _logger;

        public OmpControllerShould(OmpFixture fixture, ITestOutputHelper logger)
        {
            _fixture = fixture;
            _logger = logger;
        }

        [Fact]
        [ResetDatabase(nameof(OmpFixture))]
        public async Task works()
        {
            await ExecuteEmbeddedResource(nameof(works), false, _logger);

//            await _fixture.ExecuteDbContextAsync(async context =>
//            {
//                await context.Database.ExecuteSqlCommandAsync(@"
//DECLARE @groupeddimensions dbo.FilterConditions
//INSERT INTO @groupeddimensions VALUES (332,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,303,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL)

//DECLARE @whereconditions dbo.FilterConditions
//INSERT INTO @whereconditions VALUES (332,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,303,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL)

//DECLARE	@return_value int

//EXEC	@return_value = [dbo].[ChangeRoizingAssortmentValue]
//		@roizingassortmentid = 30,
//		@sector = N'cell',
//		@yearperiod = 201912,
//		@previousvalue = 1000,
//		@newvalue = 2000,
//		@expression = N'PlannedQuantity',
//		@relatedexpression = N'PlannedSalesTurnover',
//		@groupeddimensions = @groupeddimensions,
//		@whereconditions = @whereconditions,
//		@user = N'43B1E161-C017-4D57-8A12-AE056EEA260D'
//");
//            });
        }

        private async Task ExecuteEmbeddedResource(string methodName, bool after, ITestOutputHelper logger)
        {
            var assembly = typeof(ResetDatabaseAttribute).Assembly;
            var pattern = $@"{GetType().Name}\.{methodName}_{(after ? "after" : "before")}_?\d*\.sql$";
            var resources = new List<string>();
            foreach (var resource in assembly.GetManifestResourceNames())
            {
                if (Regex.IsMatch(resource, pattern))
                {
                    resources.Add(resource);
                }
            }
            if (!resources.Any())
            {
                throw new Exception($"There are no '{(after ? "after" : "before")}' resources to execute");
            }

            await SqlEmbeddedResourceExecutor.ExecuteAsync(!after ?
                OmpFixture.ConnectionString :
                OmpFixture.ConnectionStringAfter, assembly, resources, _logger);
        }
    }
}