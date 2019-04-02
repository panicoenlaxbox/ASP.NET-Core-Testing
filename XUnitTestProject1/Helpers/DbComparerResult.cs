using System.Collections.Generic;
using System.Linq;

namespace XUnitTestProject1.Helpers
{
    public class DbComparerResult
    {
        public DbComparerResult()
        {
            Entries = new List<DbComparerEntryResult>();
        }

        public bool Match
        {
            get
            {
                return Entries.All(e => e.Match);
            }
        }

        public IList<DbComparerEntryResult> Entries { get; }

        public static implicit operator bool(DbComparerResult result)
        {
            return result.Match;
        }

        public string Prettify()
        {
            var value = $"Comparison {(!Match ? "failed" : "success")}\n";
            if (!Match)
            {
                value += $"Tables matched {Entries.Count(e => e.Match)}\n";
                value += $"Tables not matched {Entries.Count(e => !e.Match)}\n";
                foreach (var entry in Entries.Where(e => !e.Match))
                {
                    value += $"{entry}\n";
                }
            }
            return value;
        }
    }
}