namespace XUnitTestProject1
{
    public class DbComparerEntryResult
    {
        public string TableName { get; set; }

        public bool Match => SourceChecksum == TargetChecksum;

        public int SourceChecksum { get; set; }
        public int TargetChecksum { get; set; }

        public override string ToString()
        {
            return $"{nameof(TableName)}: {TableName}, {nameof(Match)}: {Match}, {nameof(SourceChecksum)}: {SourceChecksum}, {nameof(TargetChecksum)}: {TargetChecksum}";
        }
    }
}