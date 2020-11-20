using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.DATA.EF
{
    public class QuestionMetadata
    {
        [Required]
        [Display(Name = "Question")]
        public string Question1 { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(QuestionMetadata))]
    public partial class Question { }
}
