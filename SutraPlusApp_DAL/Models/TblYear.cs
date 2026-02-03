using System;
using System.Collections.Generic;

namespace SutraPlusApp_DAL.Models
{
    public partial class TblYear
    {
        public int Id { get; set; }
        public string? FinYear { get; set; }
        public string? Dbname { get; set; }
        public bool? IsActive { get; set; }
    }
}
