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
    }
}