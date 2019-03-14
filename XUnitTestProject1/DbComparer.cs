using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace XUnitTestProject1
{
    public class DbComparerEntryResult
    {
        public string TableName { get; set; }
        public bool Match { get; set; }
        public int SourceChecksum { get; set; }
        public int TargetChecksum { get; set; }
    }

    public class DbComparerResult
    {
        public bool Match {
            get
            {
                return Entries.All(e => e.Match);
            }
        }

        public IEnumerable<DbComparerEntryResult> Entries { get; set; }

        public static implicit operator bool(DbComparerResult result)
        {
            return result.Match;
        }
    }

    public class DbComparer
    {
        private readonly string _sourceConnectionString;
        private readonly string _targetConnectionString;
        private readonly string[] _tablesToExclude = new[] { "__EFMigrationsHistory" };
        private readonly string[] _fieldsToExclude = new[] { "Id" };
        private readonly string[] _typesToExclude = new[] { "cursor", "image", "ntext", "text", "XML" };

        public DbComparer(string sourceConnectionString, string targetConnectionString)
        {
            _sourceConnectionString = sourceConnectionString;
            _targetConnectionString = targetConnectionString;
        }

        public DbComparerResult Compare()
        {
            var result = new DbComparerResult();
            using (var connection = new SqlConnection(_sourceConnectionString))
            {
                using (var tablesCommand = connection.CreateCommand())
                {
                    tablesCommand.CommandText = @"SELECT 
                                                ";
                }
            }

                //TODO: Leer schema y excluir tablas
                //TODO: Ejecutar para cada uno el checksum excluyendo fields y types
                //TODO: Return entry
                return result;
        }

    }
}