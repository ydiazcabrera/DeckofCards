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
            Deck deck = (Deck)Session["deck"];
            List<Card> cardsInHand = new List<Card>((List<Card>) Session["CardsInHand"]);
            List<string> KeepingTheseCards = new List<string>() { Card1, Card2, Card3, Card4, Card5 };

            List<Card> aNewHand = new List<Card>();

            for( int i=0; i<cardsInHand.Count; i++)
            {
                for(int index=0; index < KeepingTheseCards.Count;index++)
                {
                    if (cardsInHand[i].Code == KeepingTheseCards[index])
                    {
                        aNewHand.Add(cardsInHand[i]);
                    }
                }
            }

            count -= aNewHand.Count;

            GetCards(deck, count).ForEach(x => aNewHand.Add(x));
            Session["CardsInHand"] = aNewHand;
            return View(aNewHand);
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
            deck.Remaining = int.Parse(json["remaining"].ToString());
            Session["deck"]=deck;
            return deck;
        }

        public List<Card> GetCards(Deck deck, int count = 5)
        {
            List<Card> cards = new List<Card>();

            string URL = $"https://deckofcardsapi.com/api/deck/{deck.ID}/draw/?count={count}";

            HttpWebRequest request = WebRequest.CreateHttp(URL);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());
            string APIText = rd.ReadToEnd();

            JToken json = JToken.Parse(APIText);

            bool success = json["success"].ToString() == "true" ? true : false;
            deck.Remaining = int.Parse(json["remaining"].ToString());
            
            Session["deck"] = deck;

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