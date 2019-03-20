using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public partial class RoizingAssortmentCells
    {
        public int Id { get; set; }
        public decimal? CurrentYearAverageCost { get; set; }
        public decimal? CurrentYearAveragePrice { get; set; }
        public decimal? CurrentYearGlobalSalesTurnoverPercentage { get; set; }
        public decimal? CurrentYearGrossMargin { get; set; }
        public decimal? CurrentYearMarkup { get; set; }
        public decimal? CurrentYearPeriodSalesTurnoverPercentage { get; set; }
        public decimal? CurrentYearRowSalesTurnoverPercentage { get; set; }
        public int? CurrentYearSalesQuantity { get; set; }
        public decimal? CurrentYearSalesTurnover { get; set; }
        public decimal? CurrentYearTotalCost { get; set; }
        public decimal ForecastAverageCost { get; set; }
        public decimal ForecastAveragePrice { get; set; }
        public decimal ForecastGlobalSalesTurnoverPercentage { get; set; }
        public decimal ForecastGrossMargin { get; set; }
        public decimal ForecastMarkup { get; set; }
        public decimal ForecastPeriodSalesTurnoverPercentage { get; set; }
        public decimal ForecastRowSalesTurnoverPercentage { get; set; }
        public int? ForecastSalesQuantity { get; set; }
        public decimal ForecastSalesTurnover { get; set; }
        public decimal ForecastTotalCost { get; set; }
        public decimal? LastYearAverageCost { get; set; }
        public decimal? LastYearAveragePrice { get; set; }
        public decimal? LastYearGlobalSalesTurnoverPercentage { get; set; }
        public decimal? LastYearGrossMargin { get; set; }
        public decimal? LastYearMarkup { get; set; }
        public decimal? LastYearPeriodSalesTurnoverPercentage { get; set; }
        public decimal? LastYearRowSalesTurnoverPercentage { get; set; }
        public int? LastYearSalesQuantity { get; set; }
        public decimal? LastYearSalesTurnover { get; set; }
        public decimal? LastYearTotalCost { get; set; }
        public decimal PlannedAverageCost { get; set; }
        public decimal PlannedAveragePrice { get; set; }
        public int PlannedCoverage { get; set; }
        public int PlannedFinalStock { get; set; }
        public decimal PlannedGlobalSalesTurnoverPercentage { get; set; }
        public decimal PlannedGrossMargin { get; set; }
        public int PlannedInitialStock { get; set; }
        public decimal PlannedMarkup { get; set; }
        public decimal PlannedOtb { get; set; }
        public decimal PlannedPeriodSalesTurnoverPercentage { get; set; }
        public decimal PlannedRowSalesTurnoverPercentage { get; set; }
        public int PlannedSalesQuantity { get; set; }
        public decimal PlannedSalesTurnover { get; set; }
        public decimal PlannedTotalCost { get; set; }
        public decimal RealAndForecastAverageCost { get; set; }
        public decimal RealAndForecastAveragePrice { get; set; }
        public decimal RealAndForecastGlobalSalesTurnoverPercentage { get; set; }
        public decimal RealAndForecastGrossMargin { get; set; }
        public decimal RealAndForecastMarkup { get; set; }
        public decimal RealAndForecastPeriodSalesTurnoverPercentage { get; set; }
        public decimal RealAndForecastRowSalesTurnoverPercentage { get; set; }
        public int? RealAndForecastSalesQuantity { get; set; }
        public decimal RealAndForecastSalesTurnover { get; set; }
        public decimal RealAndForecastTotalCost { get; set; }
        public int RoizingAssortmentId { get; set; }
        public int RoizingAssortmentPeriodId { get; set; }
        public int RoizingAssortmentRowId { get; set; }
        public decimal SuggestedAverageCost { get; set; }
        public decimal SuggestedAveragePrice { get; set; }
        public int SuggestedCoverage { get; set; }
        public int SuggestedFinalStock { get; set; }
        public decimal SuggestedGlobalSalesTurnoverPercentage { get; set; }
        public decimal SuggestedGrossMargin { get; set; }
        public int SuggestedInitialStock { get; set; }
        public decimal SuggestedMarkup { get; set; }
        public decimal SuggestedOtb { get; set; }
        public decimal SuggestedPeriodSalesTurnoverPercentage { get; set; }
        public decimal SuggestedRowSalesTurnoverPercentage { get; set; }
        public int SuggestedSalesQuantity { get; set; }
        public decimal SuggestedSalesTurnover { get; set; }
        public decimal SuggestedTotalCost { get; set; }
        public int? CurrentYearFinalStock { get; set; }
        public int? CurrentYearInitialStock { get; set; }
        public int? ForecastFinalStock { get; set; }
        public int? ForecastInitialStock { get; set; }
        public int? LastYearFinalStock { get; set; }
        public int? LastYearInitialStock { get; set; }
        public int? RealAndForecastFinalStock { get; set; }
        public int? RealAndForecastInitialStock { get; set; }

        public virtual RoizingAssortmentPeriods RoizingAssortmentPeriod { get; set; }
        public virtual RoizingAssortmentRows RoizingAssortmentRow { get; set; }
    }
}