using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace MackTechGroupProject.Models
{
    public class PaymentInfo
    {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name on card")]
            public string FullName { get; set; }

            [Required]
            [DataType(DataType.CreditCard)]
            [RegularExpression(@"^([1-9]{16})$", ErrorMessage = "Invalid Credit Card Number")]   
            [Display(Name = "Credit Card Number")]
            public string CreditCardNumber { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [RegularExpression(@"^(0[1-9]|[1-9]|1[012])$", ErrorMessage = "Invalid Month")]
            [Display(Name = "Expiration Month (MM)")]
            public int ExpirationMonth { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [RegularExpression(@"^(202[0-9])$", ErrorMessage = "Invalid Year")]
            [Display(Name = "Expiration Year (YYYY)")]
            public int ExpirationYear { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "Invalid CVC")]
            [Display(Name = "CVC")]
            public int CVC { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Payment Amount $")]
            public decimal PaymentAmount { get; set; }
    }
}