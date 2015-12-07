using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService;

namespace Client
{
    using System;
    using System.Collections.Generic;

    public partial class User
    {
        public User()
        {
            this.Contact = new HashSet<Contact>();
            this.Contact1 = new HashSet<Contact>();
            this.Message = new HashSet<Message>();
            this.Message1 = new HashSet<Message>();
        }

        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Info { get; set; }
        public bool Online { get; set; }

        public virtual ICollection<Contact> Contact { get; set; }
        public virtual ICollection<Contact> Contact1 { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<Message> Message1 { get; set; }
    }
}
