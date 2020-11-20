using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.DATA.EF
{
    public class QuestionChoiceMetadata
    {
        [Required]
        [Display(Name = "Right Choice")]
        public bool IsRightChoice { get; set; }

        [Required]
        public string Choice { get; set; }
    }

    [MetadataType(typeof(QuestionChoice))]
    public partial class QuestionChoice { }
}
