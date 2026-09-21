using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Attributes
{
    public class SBI : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (int.TryParse(value.ToString(), out int sbi))
                {
                    if (sbi > 105000000 && sbi < 999999999)
                    {
                        return null;
                    }
                }
                else if (value.ToString().ToUpper() == "UNKNOWN")
                {
                    return null;
                }
                else if (value.ToString().ToUpper() == "MULTIPLE")
                {
                    return null;
                }
            }

            return new ValidationResult("Invalid SBI Number - should be a 9 digit number or the word Multiple or Unknown");
        }
    }
}