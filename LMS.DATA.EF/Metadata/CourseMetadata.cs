using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class Course
    {
        public string GenerateSlug()
        {
            string phrase = string.Format("{0}-{1}", CourseId, CourseName);

            string str = RemoveAccent(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        private string RemoveAccent(string text)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
