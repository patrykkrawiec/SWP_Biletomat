﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP_Biletomat
{
    class Ticket
    {
        public int count = 0;
        public TicketType ticketType;
        public enum TicketType
        {
            Normalny,
            Ulgowy
        }
    }
}
