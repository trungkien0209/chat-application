//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChatApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MessageRoom
    {
        public int Id { get; set; }
        public Nullable<int> FromUser { get; set; }
        public Nullable<int> ToRoom { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string FromUserName { get; set; }
    }
}
