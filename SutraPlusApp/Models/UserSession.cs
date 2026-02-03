using System;
using System.Collections.Generic;

namespace SutraPlusApp.Models
{
    public partial class UserSession
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string UserId { get; set; } = null!;
        public string? SessiondData { get; set; }
        public bool? IsActive { get; set; }
    }
}
