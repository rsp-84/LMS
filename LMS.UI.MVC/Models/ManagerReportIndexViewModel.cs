using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LMS.UI.MVC.Models
{
    public class ManagerReportIndexViewModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int NumCoursesCompletedYTD { get; set; }
        public int NumLessonsCompletedYTD { get; set; }
        public List<CoursesCompleted> CoursesCompletedYTD { get; set; }
        public List<LessonsCompleted> LessonsCompletedYTD { get; set; }
    }

    public class CoursesCompleted
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCat { get; set; }
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

        private string RemoveAccent(string text)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return Encoding.ASCII.GetString(bytes);
        }

    }

    public class LessonsCompleted
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public DateTime LessonCompletedDate { get; set; }

        public string GenerateSlug()
        {
            string phrase = string.Format("{0}-{1}", LessonId, LessonName);

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