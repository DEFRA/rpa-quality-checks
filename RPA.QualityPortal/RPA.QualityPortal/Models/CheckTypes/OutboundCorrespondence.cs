using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using RPA.QualityPortal.Attributes;

namespace RPA.QualityPortal.Models.CheckTypes
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class OutboundCorrespondence : Check
    {
        [Display(Name = "Template Reference")]
        [Required]
        public string TemplateReference { get; set; }

        [Display(Name = "Unique Identifier")]
        [Required]
        [Range(100000, 9999999)]
        public string UniqueId { get; set; }

        [Required(ErrorMessage = "Please Select a Unique Identifier prefix")]
        public int UniqueIdentifierPrefixId { get; set; }

        [Display(Name = "Unique Identifier")]
        public string UniqueIdentifierFull
        {
            get
            {
                return string.Format(UniqueIdentifierPrefix.Text + UniqueId);
            }
        }

        [Display(Name = "SBI")]
        [Required]
        [SBI]
        public string SBI { get; set; }

        [Display(Name = "CRM Case Number")]
        [Required]
        [RegularExpression(@"^[0-9]{6}-[a-zA-Z][0-9][a-zA-Z][0-9][a-zA-Z][0-9]$|^[0-9]{7}-[a-zA-Z][0-9][a-zA-Z][0-9][a-zA-Z][0-9]$", ErrorMessage = "Invalid CRM Case Number - should be in the format 123456-X1X2X3 or 1234567-X1X2X3")]
        public string CRMRef { get; set; }

        [Required(ErrorMessage = "Please Select a CRM Case Number prefix")]
        public int CRMRefPrefixId { get; set; }

        [Display(Name = "CRM Case Number")]
        public string CrmReferenceFull
        {
            get
            {
                return string.Format(CRMRefPrefix.Text + CRMRef);
            }
        }

        [DataType(DataType.Date)]
        [Display(Name = "Date Outbound Correspondence Sent")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime? OutboundCorrespondenceDateSent { get; set; }

        [Display(Name = "Line Manager")]
        [Required]
        public string ManagerName { get; set; }

        [Required]
        public string HEO { get; set; }

        [Required]
        public string SEO { get; set; }

        [Display(Name = "Comments (if the result is not approved; the reason(s) must be stated)")]
        public string Comments { get; set; }

        [Required(ErrorMessage = "The Correspondence Type field is required")]
        public int CorrespondenceTypeId { get; set; }

        [Required(ErrorMessage = "The Scheme field is required")]
        public int SchemeId { get; set; }

        [Required(ErrorMessage = "The Business Area field is required")]
        public int BusinessAreaId { get; set; }

        public int? FailReasonId { get; set; }

        [Display(Name = "Recheck Required")]
        public bool ReCheckRequired { get; set; }

        [Display(Name = "Initial Recheck Decision")]
        public bool InitialReCheckDecision { get; set; }

        [Display(Name = "Exclude Quality Check Result")]
        public bool ExcludeQCResult { get; set; }

        public virtual UniqueIdentifierPrefix UniqueIdentifierPrefix { get; set; }

        public virtual CRMRefPrefix CRMRefPrefix { get; set; }

        public virtual CorrespondenceType CorrespondenceType { get; set; }

        public virtual Scheme Scheme { get; set; }

        public virtual BusinessArea BusinessArea { get; set; }

        public virtual FailReason FailReason { get; set; }

        public OutboundCorrespondence()
        {
            CheckTypeId = 1;
            DateQCCompleted = DateTime.Now;
            EmailNotifications = true;
            ReCheckRequired = false;
        }
    }
}