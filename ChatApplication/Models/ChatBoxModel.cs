using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApplication.Models
{
    public class ChatBoxModel
    {
        public UserDTO ToUser { get; set; }
        public List<MessageDTO> Messages { get; set; }

    }
}