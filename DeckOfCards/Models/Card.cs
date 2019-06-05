using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckOfCards.Models
{
    public class Card
    {
        public string Image { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public string Suit { get; set; }
    }
}