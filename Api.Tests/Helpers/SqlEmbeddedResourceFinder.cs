using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Api.Tests.Helpers
{
    public class SqlEmbeddedResourceFinder
    {
        public IDictionary<string, IEnumerable<string>> Find(Assembly assembly, string type, string methodName,
            IEnumerable<string> suffixes)
        {
            var result = new Dictionary<string, IEnumerable<string>>();
            foreach (var suffix in suffixes)
            {
                result.Add(suffix, Discover(assembly, type, methodName, suffix));
            }
            return result;
        }

        private static IEnumerable<string> Discover(Assembly assembly, string type, string methodName, string suffix)
        {
            var pattern = $@"{type}\.{methodName}_{suffix}_?\d*\.sql(.zip)?$";
            var result = new List<string>();
            foreach (var resource in assembly.GetManifestResourceNames().OrderBy(n => n))
            {
                if (!Regex.IsMatch(resource, pattern))
                {
                    continue;
                }

                result.Add(resource);
            }

            return result;
        }
    }
}