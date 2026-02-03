using System;
using System.Collections.Generic;

namespace SutraPlusApp_DAL.Models
{
    public partial class CloseSession
    {
        public long Companyid { get; set; }
        public DateTime ClosedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
