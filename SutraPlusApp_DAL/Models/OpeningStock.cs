using System;
using System.Collections.Generic;

namespace SutraPlusApp_DAL.Models
{
    public partial class OpeningStock
    {
        public int CompanyId { get; set; }
        public int CommodityId { get; set; }
        public double Weight { get; set; }
        public decimal Amount { get; set; }
    }
}
