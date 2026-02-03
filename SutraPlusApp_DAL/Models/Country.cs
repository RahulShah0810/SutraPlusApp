using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutraPlusApp_DAL.Models
{
    public class Country
    {
        public int _Id { get; set; }
        public string CountryName { get; set; }
        public Boolean IsActive { get; set; }
    }
}
