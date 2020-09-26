using System;
using System.Collections.Generic;
using System.Text;

namespace Fahrkartenautomat
{
    public enum TicketType
    {
        SINGLE, WEEK, MONTH
    }

    public class Ticket
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public float ReducedPrice { get; set; }
        public TicketType Type { get; set; }
    }
}
