using Resources.ClientWeb.Feedback;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobalPrint.ClientWeb.Models.FeedbackViewModel
{
    public class FeedbackViewModel
    {
        [Required(ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "SubjectFieldRequiredError")]
        [StringLength(50, ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "SubjectFieldMaxLengthError")]
        [Display(ResourceType = typeof(FeedbackViewResource), Name = "SubjectFieldLabel", Prompt = "SubjectFieldLabel")]
        public string Subject { get; set; }

        [Required(ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "UserNameFieldRequiredError")]
        [StringLength(50, ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "UserNameFieldMaxLengthError")]
        [Display(ResourceType = typeof(FeedbackViewResource), Name = "UserNameFieldLabel", Prompt = "UserNameFieldLabel")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "EmailFieldRequiredError")]
        [EmailAddress(ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [StringLength(50, ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "EmailFieldMaxLengthError")]
        [Display(ResourceType = typeof(FeedbackViewResource), Name = "EmailFieldLabel", Prompt = "EmailFieldLabel")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "MessageFieldRequiredError")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessageResourceType = typeof(FeedbackViewResource), ErrorMessageResourceName = "MessageFieldMaxLengthError")]
        [Display(ResourceType = typeof(FeedbackViewResource), Name = "MessageFieldLabel", Prompt = "MessageFieldLabel")]
        public string Message { get; set; }
    }
}