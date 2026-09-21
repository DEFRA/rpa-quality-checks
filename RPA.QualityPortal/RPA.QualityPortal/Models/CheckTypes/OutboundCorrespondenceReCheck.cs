using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models.CheckTypes
{
    [ExcludeFromCodeCoverage]
    public class OutboundCorrespondenceReCheck : Check
    {

        //CheckId of the linked OutboundCorrespondence
        public int OutboundCorrespondenceId { get; set; }

        public bool ReCheckActive { get; set; }

        [Display(Name = "Recheck Comments")]
        public string ReCheckComments { get; set; }

        [Display(Name = "Recheck Completed By?")]
        [Required]
        public string ReCheckCompletedBy { get; set; }

        [Display(Name = "Is a Further Recheck Required?")]
        public bool FurtherReCheckRequired { get; set; }

        public OutboundCorrespondenceReCheck()
        {
            CheckTypeId = 2;
            EmailNotifications = true;
            FurtherReCheckRequired = false;
        }

        public OutboundCorrespondenceReCheck(int CheckId) : this()
        {
            OutboundCorrespondenceId = CheckId;
        }

    }
}