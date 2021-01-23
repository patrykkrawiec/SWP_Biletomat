using System;
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
        public float ticketPrice;
        public enum TicketType
        {
            Normalny20min,
            Normalny75min,
            Normalny24h,
            Normalny72h,
            Ulgowy20min,
            Ulgowy75min,
            Ulgowy24h,
            Ulgowy72h
        }


        public TicketType findTicketType(String s)
        {
            switch (s)
            {
                case "normalnydwudziestominutowy":
                    ticketType = Ticket.TicketType.Normalny20min;
                    ticketPrice = 3.4f;
                    break;
                case "normalnysiedemdziesięciopięciominutowy":
                    ticketType = Ticket.TicketType.Normalny75min;
                    ticketPrice = 4.4f;
                    break;
                case "normalnydobowy":
                    ticketType = Ticket.TicketType.Normalny24h;
                    ticketPrice = 15f;
                    break;
                case "normalnyweekendowy":
                    ticketType = Ticket.TicketType.Normalny72h;
                    ticketPrice = 36f;
                    break;
                case "ulgowydwudziestominutowy":
                    ticketType = Ticket.TicketType.Ulgowy20min;
                    ticketPrice = 1.7f;
                    break;
                case "ulgowysiedemdziesięciopięciominutowy":
                    ticketType = Ticket.TicketType.Ulgowy75min;
                    ticketPrice = 2.2f;
                    break;
                case "ulgowydobowy":
                    ticketType = Ticket.TicketType.Ulgowy24h;
                    ticketPrice = 7.5f;
                    break;
                case "ulgowyweekendowy":
                    ticketType = Ticket.TicketType.Ulgowy72h;
                    ticketPrice = 18f;
                    break;
                default:
                    break;
            }
            Console.WriteLine(this.ticketType.ToString());
            return this.ticketType;
        }

        public void findTicketPrice()
        {
            switch (ticketType)
            {
                case TicketType.Normalny20min:
                    ticketPrice = 3.4f;
                    break;
                case TicketType.Normalny75min:
                    ticketPrice = 4.4f;
                    break;
                case TicketType.Normalny24h:
                    ticketPrice = 15f;
                    break;
                    case TicketType.Normalny72h:
                    ticketPrice = 36f;
                    break;
                case TicketType.Ulgowy20min:
                    ticketPrice = 1.7f;
                    break;
                case TicketType.Ulgowy75min:
                    ticketPrice = 2.2f;
                    break;
                case TicketType.Ulgowy24h:
                    ticketPrice = 7.5f;
                    break;
                case TicketType.Ulgowy72h:
                    ticketPrice = 18f;
                    break;
                default:
                    break;
            }
            Console.WriteLine(this.ticketPrice.ToString());

        }
    }
}
