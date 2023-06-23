using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApplication.Models
{
    public class MessageDTO
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public string Class { get; set; }
        public System.DateTime Date { get; set; }
    }
}