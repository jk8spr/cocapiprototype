using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoCAPI.Models
{
    public class CocRequest
    {
        public int SomeNum { get; set; }
        public string SomeStr { get; set; }
        public List<Question> AnswerList { get; set; }
    }
}
