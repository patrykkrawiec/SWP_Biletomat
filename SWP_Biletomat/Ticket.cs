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
        public string ticketTypeName;
        public string ticketType1;
        public string ticketType2;
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


        public TicketType setTicketType(String s)
        {
            switch (s)
            {
                case "normalnydwudziestominutowy":
                    ticketType = Ticket.TicketType.Normalny20min;
                    break;
                case "normalnysiedemdziesięciopięciominutowy":
                    ticketType = Ticket.TicketType.Normalny75min;
                    break;
                case "normalnydobowy":
                    ticketType = Ticket.TicketType.Normalny24h;
                    break;
                case "normalnyweekendowy":
                    ticketType = Ticket.TicketType.Normalny72h;
                    break;
                case "ulgowydwudziestominutowy":
                    ticketType = Ticket.TicketType.Ulgowy20min;
                    break;
                case "ulgowysiedemdziesięciopięciominutowy":
                    ticketType = Ticket.TicketType.Ulgowy75min;
                    break;
                case "ulgowydobowy":
                    ticketType = Ticket.TicketType.Ulgowy24h;
                    break;
                case "ulgowyweekendowy":
                    ticketType = Ticket.TicketType.Ulgowy72h;
                    break;
                default:
                    break;
            }
            return this.ticketType;
        }

        public void setTicketPrice()
        {
            switch (ticketType)
            {
                case TicketType.Normalny20min:
                    ticketPrice = 3.4f;
                    ticketType1 = "normaln";
                    ticketType2 = "dwudziestominutow";
                    break;
                case TicketType.Normalny75min:
                    ticketPrice = 4.4f;
                    ticketType1 = "normaln";
                    ticketType2 = "siedemdziesięciominutow";
                    break;
                case TicketType.Normalny24h:
                    ticketPrice = 15f;
                    ticketType1 = "normaln";
                    ticketType2 = "dobow";
                    break;
                case TicketType.Normalny72h:
                    ticketPrice = 36f;
                    ticketType1 = "normaln";
                    ticketType2 = "weekendow";
                    break;
                case TicketType.Ulgowy20min:
                    ticketPrice = 1.7f;
                    ticketType1 = "ulgow";
                    ticketType2 = "dwudziestominutow";
                    break;
                case TicketType.Ulgowy75min:
                    ticketPrice = 2.2f;
                    ticketType1 = "ulgow";
                    ticketType2 = "siedemdziesięciominutow";
                    break;
                case TicketType.Ulgowy24h:
                    ticketPrice = 7.5f;
                    ticketType1 = "ulgow";
                    ticketType2 = "dobow";
                    break;
                case TicketType.Ulgowy72h:
                    ticketPrice = 18f;
                    ticketType1 = "ulgow";
                    ticketType2 = "weekendow";
                    break;
                default:
                    break;
            }
        }

        public void setTicketTypeName()
        {
            if (count == 1) ticketTypeName = ticketType1+"y "+ticketType2+"y ";
            else if (count >= 2 && count <= 4 ) ticketTypeName = ticketType1 + "e " + ticketType2 + "e ";
            else if (count >= 5) ticketTypeName = ticketType1 + "ych " + ticketType2 + "ych ";

        }
    }
}
