using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Fahrkartenautomat
{
    public class CartItem
    {
        public Ticket Ticket { get; set; }
        public bool ReducedPrice { get; set; }
        public int Ammount { get; set; }
    }
    public class Cart
    {
        private List<CartItem> _items = new List<CartItem>();
        public ImmutableList<CartItem> Items => _items.ToImmutableList();

        public void AddItem(CartItem item)
        {
            var found = _items.Find(i => i.Ticket == item.Ticket && i.ReducedPrice == item.ReducedPrice);
            if (found != null) found.Ammount += item.Ammount;
            else _items.Add(item);
        }

        public void Clear() => _items.Clear();
    }
}
