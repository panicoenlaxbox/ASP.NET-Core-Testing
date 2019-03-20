using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public partial class CatalogueSnapshots
    {
        public CatalogueSnapshots()
        {
            MerchandisePlannings = new HashSet<MerchandisePlannings>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public bool Enabled { get; set; }
        public DateTime When { get; set; }

        public virtual Companies Company { get; set; }
        public virtual ICollection<MerchandisePlannings> MerchandisePlannings { get; set; }
    }
}