using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.DATA.EF
{
    public class CourseMetadata
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [StringLength(500)]
        [Display(Name = "Course Description")]
        public string CourseDescription { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Required]
        [StringLength(500)]
        public string Category { get; set; }

        [StringLength(1000)]
        [Display(Name = "Course Image")]
        public string CourseImg { get; set; }
    }

    [MetadataType(typeof(CourseMetadata))]
    public partial class Course { }
}
