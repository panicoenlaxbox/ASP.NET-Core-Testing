using ClassLibrary1.Mappings;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        
        public DbSet<Country> Countries { get; set; }

        #region MerchandisePlanning
        public virtual DbSet<CatalogueSnapshots> CatalogueSnapshots { get; set; }
        public virtual DbSet<Companies> Companies { get; set; }
        public virtual DbSet<MerchandisePlanningProductDimensions> MerchandisePlanningProductDimensions { get; set; }
        public virtual DbSet<MerchandisePlannings> MerchandisePlannings { get; set; }
        public virtual DbSet<RoizingAssortmentCells> RoizingAssortmentCells { get; set; }
        public virtual DbSet<RoizingAssortmentPeriods> RoizingAssortmentPeriods { get; set; }
        public virtual DbSet<RoizingAssortmentRows> RoizingAssortmentRows { get; set; }
        public virtual DbSet<RoizingAssortments> RoizingAssortments { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());

            MerchandisePlanning(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void MerchandisePlanning(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatalogueSnapshots>(entity =>
            {
                entity.ToTable("CatalogueSnapshots", "Catalogue");

                entity.HasIndex(e => e.CompanyId);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CatalogueSnapshots)
                    .HasForeignKey(d => d.CompanyId);
            });

            modelBuilder.Entity<Companies>(entity =>
            {
                entity.ToTable("Companies", "Catalogue");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<MerchandisePlanningProductDimensions>(entity =>
            {
                entity.HasIndex(e => e.MerchandisePlanningId);

                entity.Property(e => e.Typology)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.MerchandisePlanning)
                    .WithMany(p => p.MerchandisePlanningProductDimensions)
                    .HasForeignKey(d => d.MerchandisePlanningId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<MerchandisePlannings>(entity =>
            {
                entity.HasIndex(e => e.CatalogueSnapshotId);

                entity.Property(e => e.Description).HasMaxLength(128);

                entity.Property(e => e.Inflation).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.CatalogueSnapshot)
                    .WithMany(p => p.MerchandisePlannings)
                    .HasForeignKey(d => d.CatalogueSnapshotId);
            });

            modelBuilder.Entity<RoizingAssortmentCells>(entity =>
            {
                entity.HasIndex(e => e.RoizingAssortmentPeriodId);

                entity.HasIndex(e => e.RoizingAssortmentRowId);

                entity.HasIndex(e => new { e.PlannedSalesQuantity, e.PlannedTotalCost, e.RoizingAssortmentPeriodId, e.RoizingAssortmentRowId, e.RoizingAssortmentId })
                    .HasName("IX_RoizingAssortmentCells_RoizingAssortmentIdIncludingRowPeriodQuantityAndCost");

                entity.Property(e => e.CurrentYearAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedOtb).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedOtb).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedTotalCost).HasColumnType("decimal(28, 15)");

                entity.HasOne(d => d.RoizingAssortmentPeriod)
                    .WithMany(p => p.RoizingAssortmentCells)
                    .HasForeignKey(d => d.RoizingAssortmentPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RoizingAssortmentRow)
                    .WithMany(p => p.RoizingAssortmentCells)
                    .HasForeignKey(d => d.RoizingAssortmentRowId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RoizingAssortmentPeriods>(entity =>
            {
                entity.HasIndex(e => e.RoizingAssortmentId);

                entity.Property(e => e.CurrentYearAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedOtb).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedOtb).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedTotalCost).HasColumnType("decimal(28, 15)");

                entity.HasOne(d => d.RoizingAssortment)
                    .WithMany(p => p.RoizingAssortmentPeriods)
                    .HasForeignKey(d => d.RoizingAssortmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RoizingAssortmentRows>(entity =>
            {
                entity.HasIndex(e => e.RoizingAssortmentId);

                entity.HasIndex(e => new { e.PointOfSaleId, e.ProductClassificationLevel1, e.ProductClassificationLevel10, e.ProductClassificationLevel2, e.ProductClassificationLevel3, e.ProductClassificationLevel4, e.ProductClassificationLevel5, e.ProductClassificationLevel6, e.ProductClassificationLevel7, e.ProductClassificationLevel8, e.ProductClassificationLevel9, e.TypologyId, e.RoizingAssortmentId })
                    .HasName("IX_RoizingAssortmentRows_RoizingAssortmentIdIncludingDimensions");

                entity.Property(e => e.Channel).HasMaxLength(128);

                entity.Property(e => e.CurrentYearAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedOtb).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PointOfSale).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel1).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel10).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel2).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel3).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel4).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel5).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel6).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel7).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel8).HasMaxLength(128);

                entity.Property(e => e.ProductClassificationLevel9).HasMaxLength(128);

                entity.Property(e => e.RealAndForecastAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedOtb).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.Typology).HasMaxLength(128);

                entity.HasOne(d => d.RoizingAssortment)
                    .WithMany(p => p.RoizingAssortmentRows)
                    .HasForeignKey(d => d.RoizingAssortmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RoizingAssortments>(entity =>
            {
                entity.HasIndex(e => e.MerchandisePlanningId)
                    .IsUnique();

                entity.Property(e => e.CurrentYearAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.CurrentYearTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.ForecastTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.LastYearTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedOtb).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.PlannedTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.RealAndForecastTotalCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedAverageCost).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedAveragePrice).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedGlobalSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedGrossMargin).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedMarkup).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedOtb).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedPeriodSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedRowSalesTurnoverPercentage).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedSalesTurnover).HasColumnType("decimal(28, 15)");

                entity.Property(e => e.SuggestedTotalCost).HasColumnType("decimal(28, 15)");

                entity.HasOne(d => d.MerchandisePlanning)
                    .WithOne(p => p.RoizingAssortments)
                    .HasForeignKey<RoizingAssortments>(d => d.MerchandisePlanningId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}