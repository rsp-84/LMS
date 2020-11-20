using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.DATA.EF
{
    public class LessonMetadata
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Title")]
        public string LessonTitle { get; set; }

        [StringLength(300)]
        public string Introduction { get; set; }

        [StringLength(500)]
        [Display(Name = "Video URL")]
        public string VideoURL { get; set; }

        [StringLength(100)]
        [Display(Name = "PDF File")]
        public string PdfFilename { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(LessonMetadata))]
    public partial class Lesson { }
}
