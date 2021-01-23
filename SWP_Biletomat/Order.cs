using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP_Biletomat
{
    class Order
    {
        public List<Ticket> Tickets { get; set; }
        public bool isDone = false;
        public bool isPaid = false;
    }
}
