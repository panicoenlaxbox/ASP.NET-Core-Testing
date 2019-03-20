using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using XUnitTestProject1.Helpers;

namespace XUnitTestProject1
{
    public class SqlBatchParserShould
    {
        [Fact]
        public void create_a_list_with_an_item_for_each_batch_and_disabled_foreign_keys()
        {
            const string script = @"
SET IDENTITY_INSERT [dbo].[Table_1] ON
INSERT [dbo].[Table_1] (Id, Name) VALUES (1, 'a')
INSERT [dbo].[Table_1] (Id, Name) VALUES (2, 'b')
INSERT [dbo].[Table_1] (Id, Name) VALUES (3, 'c')
SET IDENTITY_INSERT [dbo].[Table_1] OFF
SET IDENTITY_INSERT [dbo].[Table_2] ON
INSERT [dbo].[Table_2] (Id, Name) VALUES (1, 'a')
INSERT [dbo].[Table_2] (Id, Name) VALUES (2, 'b')
INSERT [dbo].[Table_2] (Id, Name) VALUES (3, 'c')
SET IDENTITY_INSERT [dbo].[Table_2] OFF
";
            using (var reader = CreateStreamReader(script))
            {
                var statements = SqlBatchParser.ParseScriptData(reader, batchSize: 2).ToList();
                statements.Should().BeEquivalentTo(new ParsedStatementSet[]
                {
                    new ParsedStatementSet(
                        new ParsedStatement[] {
                            new ParsedStatement("[dbo].[Table_1]", "ALTER TABLE [dbo].[Table_1] NOCHECK CONSTRAINT ALL;",ParsedStatementKind.NoCheckConstraint),
                            new ParsedStatement("[dbo].[Table_2]", "ALTER TABLE [dbo].[Table_2] NOCHECK CONSTRAINT ALL;",ParsedStatementKind.NoCheckConstraint),
                        }),
                    new ParsedInsertStatementSet(
                        new ParsedStatement[] {
                            new ParsedStatement("[dbo].[Table_1]", "SET IDENTITY_INSERT [dbo].[Table_1] ON;",ParsedStatementKind.IdentityInsertOn),
                            new ParsedStatement("[dbo].[Table_1]", "INSERT [dbo].[Table_1] (Id, Name) VALUES\n(1, 'a'),\n(2, 'b');",ParsedStatementKind.Insert,2),
                            new ParsedStatement("[dbo].[Table_1]", "INSERT [dbo].[Table_1] (Id, Name) VALUES\n(3, 'c');",ParsedStatementKind.Insert,1),
                            new ParsedStatement("[dbo].[Table_1]", "SET IDENTITY_INSERT [dbo].[Table_1] OFF;",ParsedStatementKind.IdentityInsertOff)
                        }),
                    new ParsedInsertStatementSet(
                        new ParsedStatement[] {
                            new ParsedStatement("[dbo].[Table_2]", "SET IDENTITY_INSERT [dbo].[Table_2] ON;",ParsedStatementKind.IdentityInsertOn),
                            new ParsedStatement("[dbo].[Table_2]", "INSERT [dbo].[Table_2] (Id, Name) VALUES\n(1, 'a'),\n(2, 'b');",ParsedStatementKind.Insert,2),
                            new ParsedStatement("[dbo].[Table_2]", "INSERT [dbo].[Table_2] (Id, Name) VALUES\n(3, 'c');",ParsedStatementKind.Insert,1),
                            new ParsedStatement("[dbo].[Table_2]", "SET IDENTITY_INSERT [dbo].[Table_2] OFF;",ParsedStatementKind.IdentityInsertOff),
                        }),
                    new ParsedStatementSet(
                        new ParsedStatement[] {
                            new ParsedStatement("[dbo].[Table_1]", "ALTER TABLE [dbo].[Table_1] WITH CHECK CHECK CONSTRAINT ALL;",ParsedStatementKind.CheckConstraint),
                            new ParsedStatement("[dbo].[Table_2]", "ALTER TABLE [dbo].[Table_2] WITH CHECK CHECK CONSTRAINT ALL;",ParsedStatementKind.CheckConstraint)
                        })
                });

            }
        }

        private static StreamReader CreateStreamReader(string value)
        {
            byte[] bytes = Encoding.Default.GetBytes(value);
            var ms = new MemoryStream(bytes);
            var reader = new StreamReader(ms);
            return reader;
        }
    }
}