//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LMS.DATA.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserQuestionAnswer
    {
        public int UserAnswerId { get; set; }
        public string UserId { get; set; }
        public int ChoiceId { get; set; }
        public bool IsRight { get; set; }
        public Nullable<System.DateTime> AnswerTime { get; set; }
    
        public virtual QuestionChoice QuestionChoice { get; set; }
        public virtual UserDetail UserDetail { get; set; }
    }
}
