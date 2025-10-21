using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flights.Domain.Users
{
    public class Role
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
