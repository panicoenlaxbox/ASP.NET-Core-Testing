namespace XUnitTestProject1
{
    public class DbComparerEntryResult
    {
        public string Schema { get; set; }
        public string Table { get; set; }
        public bool Match => SourceChecksum == TargetChecksum;
        public int SourceChecksum { get; set; }
        public int TargetChecksum { get; set; }
        public int? SourceCount { get; set; }
        public int? TargetCount { get; set; }

        public override string ToString()
        {
            return $"{nameof(Schema)}: {Schema}, {nameof(Table)}: {Table}, {nameof(Match)}: {Match}, {nameof(SourceChecksum)}: {SourceChecksum}, {nameof(TargetChecksum)}: {TargetChecksum}, {nameof(SourceCount)}: {SourceCount}, {nameof(TargetCount)}: {TargetCount}";
        }
    }
}