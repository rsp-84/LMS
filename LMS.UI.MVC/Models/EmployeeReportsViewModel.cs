using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LMS.UI.MVC.Models
{
    public class EmployeeReportsViewModel
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; }
        public DateTime LessonCompletedDate { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseImg { get; set; }
        public string CouseCat { get; set; }
        public DateTime CourseCompletedDate { get; set; }

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

        public string GenerateSlug2()
        {
            string phrase = string.Format("{0}-{1}", LessonId, LessonTitle);

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