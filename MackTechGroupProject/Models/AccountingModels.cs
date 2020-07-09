using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MackTechGroupProject.Models
{
    public class Accounting
    {
        public int id { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime PaymentDate { get; set; }
        public int TotalCreditHours { get; set; }
        public decimal TotalBalance { get; set; }
    }
}