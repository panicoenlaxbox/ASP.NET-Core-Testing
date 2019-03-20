using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public partial class MerchandisePlanningProductDimensions
    {
        public int Id { get; set; }
        public int MerchandisePlanningId { get; set; }
        public bool SelectedClassificationLevel1 { get; set; }
        public bool SelectedClassificationLevel10 { get; set; }
        public bool SelectedClassificationLevel2 { get; set; }
        public bool SelectedClassificationLevel3 { get; set; }
        public bool SelectedClassificationLevel4 { get; set; }
        public bool SelectedClassificationLevel5 { get; set; }
        public bool SelectedClassificationLevel6 { get; set; }
        public bool SelectedClassificationLevel7 { get; set; }
        public bool SelectedClassificationLevel8 { get; set; }
        public bool SelectedClassificationLevel9 { get; set; }
        public string Typology { get; set; }
        public int TypologyId { get; set; }

        public virtual MerchandisePlannings MerchandisePlanning { get; set; }
    }
}