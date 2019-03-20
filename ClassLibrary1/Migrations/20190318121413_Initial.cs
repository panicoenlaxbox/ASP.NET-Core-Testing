using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary1.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalogue");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "Catalogue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    IsSynchronizing = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogueSnapshots",
                schema: "Catalogue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    When = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogueSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogueSnapshots_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Catalogue",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchandisePlannings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CalendarType = table.Column<int>(nullable: false),
                    CatalogueSnapshotId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: true),
                    CurrentPeriod = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 128, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Inflation = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastCalculationDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    NextCalculationDate = table.Column<DateTime>(nullable: true),
                    PeriodType = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchandisePlannings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchandisePlannings_CatalogueSnapshots_CatalogueSnapshotId",
                        column: x => x.CatalogueSnapshotId,
                        principalSchema: "Catalogue",
                        principalTable: "CatalogueSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchandisePlanningProductDimensions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MerchandisePlanningId = table.Column<int>(nullable: false),
                    SelectedClassificationLevel1 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel10 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel2 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel3 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel4 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel5 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel6 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel7 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel8 = table.Column<bool>(nullable: false),
                    SelectedClassificationLevel9 = table.Column<bool>(nullable: false),
                    Typology = table.Column<string>(maxLength: 128, nullable: false),
                    TypologyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchandisePlanningProductDimensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchandisePlanningProductDimensions_MerchandisePlannings_MerchandisePlanningId",
                        column: x => x.MerchandisePlanningId,
                        principalTable: "MerchandisePlannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoizingAssortments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentYearAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearSalesQuantity = table.Column<int>(nullable: true),
                    CurrentYearSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    ForecastAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastSalesQuantity = table.Column<int>(nullable: false),
                    ForecastSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    LastYearAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearSalesQuantity = table.Column<int>(nullable: true),
                    LastYearSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    MerchandisePlanningId = table.Column<int>(nullable: false),
                    PlannedAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedCoverage = table.Column<int>(nullable: false),
                    PlannedFinalStock = table.Column<int>(nullable: false),
                    PlannedGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedInitialStock = table.Column<int>(nullable: false),
                    PlannedMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedOtb = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedSalesQuantity = table.Column<int>(nullable: false),
                    PlannedSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastSalesQuantity = table.Column<int>(nullable: false),
                    RealAndForecastSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedCoverage = table.Column<int>(nullable: false),
                    SuggestedFinalStock = table.Column<int>(nullable: false),
                    SuggestedGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedInitialStock = table.Column<int>(nullable: false),
                    SuggestedMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedOtb = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedSalesQuantity = table.Column<int>(nullable: false),
                    SuggestedSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoizingAssortments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoizingAssortments_MerchandisePlannings_MerchandisePlanningId",
                        column: x => x.MerchandisePlanningId,
                        principalTable: "MerchandisePlannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoizingAssortmentPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentYearAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearSalesQuantity = table.Column<int>(nullable: true),
                    CurrentYearSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    ForecastAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastSalesQuantity = table.Column<int>(nullable: false),
                    ForecastSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    LastYearAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearSalesQuantity = table.Column<int>(nullable: true),
                    LastYearSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    MerchandisePlanningPeriodId = table.Column<int>(nullable: false),
                    PlannedAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedCoverage = table.Column<int>(nullable: false),
                    PlannedFinalStock = table.Column<int>(nullable: false),
                    PlannedGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedInitialStock = table.Column<int>(nullable: false),
                    PlannedMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedOtb = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedSalesQuantity = table.Column<int>(nullable: false),
                    PlannedSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastSalesQuantity = table.Column<int>(nullable: false),
                    RealAndForecastSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RoizingAssortmentId = table.Column<int>(nullable: false),
                    SuggestedAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedCoverage = table.Column<int>(nullable: false),
                    SuggestedFinalStock = table.Column<int>(nullable: false),
                    SuggestedGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedInitialStock = table.Column<int>(nullable: false),
                    SuggestedMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedOtb = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedSalesQuantity = table.Column<int>(nullable: false),
                    SuggestedSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    YearPeriod = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoizingAssortmentPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoizingAssortmentPeriods_RoizingAssortments_RoizingAssortmentId",
                        column: x => x.RoizingAssortmentId,
                        principalTable: "RoizingAssortments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoizingAssortmentRows",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Channel = table.Column<string>(maxLength: 128, nullable: true),
                    ChannelId = table.Column<int>(nullable: false),
                    CurrentYearAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearSalesQuantity = table.Column<int>(nullable: true),
                    CurrentYearSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    ForecastAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastSalesQuantity = table.Column<int>(nullable: false),
                    ForecastSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    LastYearAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearSalesQuantity = table.Column<int>(nullable: true),
                    LastYearSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    PlannedAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedCoverage = table.Column<int>(nullable: false),
                    PlannedFinalStock = table.Column<int>(nullable: false),
                    PlannedGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedInitialStock = table.Column<int>(nullable: false),
                    PlannedMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedOtb = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedSalesQuantity = table.Column<int>(nullable: false),
                    PlannedSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PointOfSale = table.Column<string>(maxLength: 128, nullable: true),
                    PointOfSaleId = table.Column<int>(nullable: false),
                    ProductClassificationLevel1 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel10 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel2 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel3 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel4 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel5 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel6 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel7 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel8 = table.Column<string>(maxLength: 128, nullable: true),
                    ProductClassificationLevel9 = table.Column<string>(maxLength: 128, nullable: true),
                    RealAndForecastAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastSalesQuantity = table.Column<int>(nullable: false),
                    RealAndForecastSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RoizingAssortmentId = table.Column<int>(nullable: false),
                    SuggestedAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedCoverage = table.Column<int>(nullable: false),
                    SuggestedFinalStock = table.Column<int>(nullable: false),
                    SuggestedGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedInitialStock = table.Column<int>(nullable: false),
                    SuggestedMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedOtb = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedSalesQuantity = table.Column<int>(nullable: false),
                    SuggestedSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    Typology = table.Column<string>(maxLength: 128, nullable: true),
                    TypologyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoizingAssortmentRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoizingAssortmentRows_RoizingAssortments_RoizingAssortmentId",
                        column: x => x.RoizingAssortmentId,
                        principalTable: "RoizingAssortments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoizingAssortmentCells",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentYearAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearSalesQuantity = table.Column<int>(nullable: true),
                    CurrentYearSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    CurrentYearTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    ForecastAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastSalesQuantity = table.Column<int>(nullable: true),
                    ForecastSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    ForecastTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    LastYearAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearSalesQuantity = table.Column<int>(nullable: true),
                    LastYearSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    LastYearTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: true),
                    PlannedAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedCoverage = table.Column<int>(nullable: false),
                    PlannedFinalStock = table.Column<int>(nullable: false),
                    PlannedGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedInitialStock = table.Column<int>(nullable: false),
                    PlannedMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedOtb = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedSalesQuantity = table.Column<int>(nullable: false),
                    PlannedSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    PlannedTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastSalesQuantity = table.Column<int>(nullable: true),
                    RealAndForecastSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RealAndForecastTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    RoizingAssortmentId = table.Column<int>(nullable: false),
                    RoizingAssortmentPeriodId = table.Column<int>(nullable: false),
                    RoizingAssortmentRowId = table.Column<int>(nullable: false),
                    SuggestedAverageCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedAveragePrice = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedCoverage = table.Column<int>(nullable: false),
                    SuggestedFinalStock = table.Column<int>(nullable: false),
                    SuggestedGlobalSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedGrossMargin = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedInitialStock = table.Column<int>(nullable: false),
                    SuggestedMarkup = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedOtb = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedPeriodSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedRowSalesTurnoverPercentage = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedSalesQuantity = table.Column<int>(nullable: false),
                    SuggestedSalesTurnover = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    SuggestedTotalCost = table.Column<decimal>(type: "decimal(28, 15)", nullable: false),
                    CurrentYearFinalStock = table.Column<int>(nullable: true),
                    CurrentYearInitialStock = table.Column<int>(nullable: true),
                    ForecastFinalStock = table.Column<int>(nullable: true),
                    ForecastInitialStock = table.Column<int>(nullable: true),
                    LastYearFinalStock = table.Column<int>(nullable: true),
                    LastYearInitialStock = table.Column<int>(nullable: true),
                    RealAndForecastFinalStock = table.Column<int>(nullable: true),
                    RealAndForecastInitialStock = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoizingAssortmentCells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoizingAssortmentCells_RoizingAssortmentPeriods_RoizingAssortmentPeriodId",
                        column: x => x.RoizingAssortmentPeriodId,
                        principalTable: "RoizingAssortmentPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoizingAssortmentCells_RoizingAssortmentRows_RoizingAssortmentRowId",
                        column: x => x.RoizingAssortmentRowId,
                        principalTable: "RoizingAssortmentRows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerchandisePlanningProductDimensions_MerchandisePlanningId",
                table: "MerchandisePlanningProductDimensions",
                column: "MerchandisePlanningId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchandisePlannings_CatalogueSnapshotId",
                table: "MerchandisePlannings",
                column: "CatalogueSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_RoizingAssortmentCells_RoizingAssortmentPeriodId",
                table: "RoizingAssortmentCells",
                column: "RoizingAssortmentPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_RoizingAssortmentCells_RoizingAssortmentRowId",
                table: "RoizingAssortmentCells",
                column: "RoizingAssortmentRowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoizingAssortmentCells_RoizingAssortmentIdIncludingRowPeriodQuantityAndCost",
                table: "RoizingAssortmentCells",
                columns: new[] { "PlannedSalesQuantity", "PlannedTotalCost", "RoizingAssortmentPeriodId", "RoizingAssortmentRowId", "RoizingAssortmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoizingAssortmentPeriods_RoizingAssortmentId",
                table: "RoizingAssortmentPeriods",
                column: "RoizingAssortmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoizingAssortmentRows_RoizingAssortmentId",
                table: "RoizingAssortmentRows",
                column: "RoizingAssortmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoizingAssortmentRows_RoizingAssortmentIdIncludingDimensions",
                table: "RoizingAssortmentRows",
                columns: new[] { "PointOfSaleId", "ProductClassificationLevel1", "ProductClassificationLevel10", "ProductClassificationLevel2", "ProductClassificationLevel3", "ProductClassificationLevel4", "ProductClassificationLevel5", "ProductClassificationLevel6", "ProductClassificationLevel7", "ProductClassificationLevel8", "ProductClassificationLevel9", "TypologyId", "RoizingAssortmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoizingAssortments_MerchandisePlanningId",
                table: "RoizingAssortments",
                column: "MerchandisePlanningId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogueSnapshots_CompanyId",
                schema: "Catalogue",
                table: "CatalogueSnapshots",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "MerchandisePlanningProductDimensions");

            migrationBuilder.DropTable(
                name: "RoizingAssortmentCells");

            migrationBuilder.DropTable(
                name: "RoizingAssortmentPeriods");

            migrationBuilder.DropTable(
                name: "RoizingAssortmentRows");

            migrationBuilder.DropTable(
                name: "RoizingAssortments");

            migrationBuilder.DropTable(
                name: "MerchandisePlannings");

            migrationBuilder.DropTable(
                name: "CatalogueSnapshots",
                schema: "Catalogue");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "Catalogue");
        }
    }
}
