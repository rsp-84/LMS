using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.DATA.EF
{
    class UserQuestionAnswerMetadata
    {
        [DisplayFormat(DataFormatString = "{0:g}")]
        public Nullable<System.DateTime> AnswerTime { get; set; }
    }

    [MetadataType(typeof(UserQuestionAnswerMetadata))]
    public partial class UserQuestionAnswer { }

}
