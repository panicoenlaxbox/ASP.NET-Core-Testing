using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public partial class MerchandisePlannings
    {
        public MerchandisePlannings()
        {
            MerchandisePlanningProductDimensions = new HashSet<MerchandisePlanningProductDimensions>();
        }

        public int Id { get; set; }
        public int CalendarType { get; set; }
        public int CatalogueSnapshotId { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CurrentPeriod { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Inflation { get; set; }
        public DateTime? LastCalculationDate { get; set; }
        public string Name { get; set; }
        public DateTime? NextCalculationDate { get; set; }
        public int PeriodType { get; set; }
        public DateTime StartDate { get; set; }
        public int Status { get; set; }

        public virtual CatalogueSnapshots CatalogueSnapshot { get; set; }
        public virtual RoizingAssortments RoizingAssortments { get; set; }
        public virtual ICollection<MerchandisePlanningProductDimensions> MerchandisePlanningProductDimensions { get; set; }
    }
}