using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public partial class Companies
    {
        public Companies()
        {
            CatalogueSnapshots = new HashSet<CatalogueSnapshots>();
        }

        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public bool IsSynchronizing { get; set; }

        public virtual ICollection<CatalogueSnapshots> CatalogueSnapshots { get; set; }
    }
}