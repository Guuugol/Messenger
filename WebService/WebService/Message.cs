//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebService
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message
    {
        public System.Guid ID { get; set; }
        public System.Guid FromID { get; set; }
        public System.Guid ToID { get; set; }
        public string Text { get; set; }
        public byte Recieved { get; set; }
        public Nullable<System.DateTime> Date_time { get; set; }
        public byte DeletedBySender { get; set; }
        public byte DeletedByReciever { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
