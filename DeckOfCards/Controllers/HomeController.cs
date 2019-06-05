using DeckOfCards.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DeckOfCards.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Deck deck = GetDeck();
            List<Card> cards = GetCards(deck);
            Session["CardsInHand"] = cards;

            return View(cards);           
        }
        [HttpPost]
        public ActionResult Index(int count, string Card1, string Card2, string Card3, string Card4, string Card5)
        {
            Deck deck = (Deck)Session["Deck"];
            List<string> newCardList = new List<string>() { Card1, Card2, Card3, Card4, Card5 };
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public Deck GetDeck()
        {
          string URL = $"https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1";

            HttpWebRequest request = WebRequest.CreateHttp(URL);

            //There will sometimes be extra steps here between request and response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());

            string APIText = rd.ReadToEnd();
            Deck deck = ConvertToDeck(APIText);
            return deck;
        }
        public Deck ConvertToDeck (string APIText)
        {
            JToken json = JToken.Parse(APIText);
            Deck deck= new Deck();
            deck.ID = json["deck_id"].ToString();
            Session["deckid"]=deck;
            return deck;
        }

        public List<Card> GetCards(Deck deck, int count = 5)
        {
            List<Card> cards = new List<Card>();

            string URL = $"https://deckofcardsapi.com/api/deck/{deck.ID}/draw/?count=5";

            HttpWebRequest request = WebRequest.CreateHttp(URL);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());
            string APIText = rd.ReadToEnd();
            JToken json = JToken.Parse(APIText);

            Session["Deck"] = deck;

            List<JToken> jtokens = json["cards"].ToList();
            foreach (JToken jtoken in jtokens)
            {
                Card card = ConvertToCard(jtoken);
                cards.Add(card);
            }

            return cards;
        }

        public Card ConvertToCard(JToken jToken)
        {
            Card card = new Card
            {
                Image = jToken["image"].ToString(),
                Value = jToken["value"].ToString(),
                Suit = jToken["suit"].ToString(),
                Code = jToken["code"].ToString()
            };
            return card;
            
        }
     }
}