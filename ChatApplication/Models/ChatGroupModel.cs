using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApplication.Models
{
    public class ChatGroupModel
    {
        public Room toRoom { get; set; }
        public List<MessageDTO> Messages { get; set; }
    }
}