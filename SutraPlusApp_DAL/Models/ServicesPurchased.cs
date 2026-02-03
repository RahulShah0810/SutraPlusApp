using System;
using System.Collections.Generic;

namespace SutraPlusApp_DAL.Models
{
    public partial class ServicesPurchased
    {
        public string CustomerId { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public decimal ServiceCost { get; set; }
    }
}
