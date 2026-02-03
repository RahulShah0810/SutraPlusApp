using System;
using System.Collections.Generic;

namespace SutraPlusApp_DAL.Models
{
    public partial class UserSession
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string UserId { get; set; } = null!;
    }
}
