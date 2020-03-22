using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Complete {
    public class DeckManager : MonoBehaviour
    {
        public Canvas canvas;
        public PlayerManager playerManager;
        private Transform cardSample;
        private Transform CurrentDeck;
        private Transform NewCard;
        private Transform DrawAndDiscard;
        private Transform Draw;
        private Transform Discard;
        private Transform CurrentCards;
        private Transform Keyboard;

        private List<Transform> CanvasPages = new List<Transform>();
        private List<Transform> currentCards = new List<Transform>();
        private Image newCardImage;
        private SkillDetail cardBeingAdded;
        private CombatManager combatManager;
        private PodiumManager currentPodium;
        private List<SkillDetail> allCards;
        private bool first = true;
        private Transform AddCardButton;
        // Start is called before the first frame update
        void Start()
        {
            CanvasPages.Clear();
            canvas = gameObject.GetComponent<Canvas>();
            playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
            Transform[] trs = transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform trf in trs)
            {
                if (trf.gameObject.name == "CurrentDeck")
                {
                    CurrentDeck = trf;
                }
                else if (trf.gameObject.name == "SampleCard")
                {
                    cardSample = trf;
                }
                else if (trf.gameObject.name == "NewCard")
                {
                    NewCard = trf;
                }
                else if (trf.gameObject.name == "Keyboard")
                {
                    Keyboard = trf;
                }
                else if (trf.gameObject.name == "DrawAndDiscard") {
                    DrawAndDiscard = trf;
                }
                else if (trf.gameObject.name == "Draw")
                {
                    Draw = trf;
                }
                else if (trf.gameObject.name == "Discard")
                {
                    Discard = trf;
                }
                else if (trf.gameObject.name == "CurrentCards")
                {
                    CurrentCards = trf;
                }
                else if (trf.gameObject.name == "NewCardSprite")
                {
                    newCardImage = trf.GetComponent<Image>();
                }
                else if (trf.gameObject.name == "AddCardButton")
                {
                    AddCardButton = trf;
                }
            }
            //cardSample = transform.Find("SampleCard");
            //CurrentDeck = transform.Find("CurrentDeck");
            CanvasPages.Add(CurrentDeck);
            //NewCard = transform.Find("NewCard");
            CanvasPages.Add(NewCard);
            //Keyboard = transform.Find("Keyboard");
            CanvasPages.Add(Keyboard);
            //DrawAndDiscard = transform.Find("DrawAndDiscard");
            CanvasPages.Add(DrawAndDiscard);
            //Draw = DrawAndDiscard.Find("Draw");
            //Discard = DrawAndDiscard.Find("Discard");
            CanvasPages.Add(AddCardButton);
            //LoadPlayerDeckToPannel(CurrentDeck);
            //LoadPlayerDeckToPannel(CurrentCards);
            //CurrentCards = NewCard.Find("CurrentCards");
            CanvasPages.Add(CurrentCards);
            CanvasPages.Add(newCardImage.transform);

            //newCardImage = NewCard.Find("NewCardSprite").GetComponent<Image>();
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
            
        }

  

        void LoadPlayerDeckToPannel(Transform CardsParent)
        {
            int cardCount = 0;
            foreach (SkillDetail nextCard in playerManager.current_deck)
            {
                cardCount += 1;
                Transform newCard = Instantiate(cardSample, cardSample.position, cardSample.rotation);
                newCard.gameObject.SetActive(true);
                newCard.SetParent(CardsParent);
                var ActiveSprite = Resources.Load<Sprite>("ActiveCard" + nextCard.skill_sprite_name);
                Image newSprite = newCard.GetComponent<Image>();
                newSprite.sprite = ActiveSprite;
                currentCards.Add(newCard);
            }
        }

        void LoadListToPannel(Transform CardsParent, List<SkillDetail> Cards)
        {
            int cardCount = 0;
            foreach (SkillDetail nextCard in Cards)
            {
                cardCount += 1;
                Transform newCard = Instantiate(cardSample, cardSample.position, cardSample.rotation);
                newCard.gameObject.SetActive(true);
                newCard.SetParent(CardsParent);
                var ActiveSprite = Resources.Load<Sprite>("ActiveCard" + nextCard.skill_sprite_name);
                Image newSprite = newCard.GetComponent<Image>();
                newSprite.sprite = ActiveSprite;
                currentCards.Add(newCard);
            }
        }

        void LoadPlayerDrawPileAndDiscardPile()
        {
            LoadListToPannel(Draw, playerManager.draw_pile);
            LoadListToPannel(Discard, playerManager.discard_pile);

        }

        public void ShowAddCards(PodiumManager podium)
        {
            currentPodium = podium;
            ClearCurrentDeck();
            Start();       
            DisableAllMenus();
            NewCard.gameObject.SetActive(true);
            CurrentCards.gameObject.SetActive(true);
            AddCardButton.gameObject.SetActive(true);
            newCardImage.gameObject.SetActive(true);
            LoadListToPannel(CurrentCards, playerManager.current_deck);
        }

        private void DisableAllMenus()
        {
            foreach (Transform nextMenu in CanvasPages)
            {
                nextMenu.gameObject.SetActive(false);
            }
        }

        public void ShowKeyboard()
        {

            DisableAllMenus();
            Keyboard.gameObject.SetActive(true);
        }

        public void ShowCurrentDeck()
        {
            ClearCurrentDeck();
            Start();
            DisableAllMenus();
            LoadListToPannel(CurrentDeck, playerManager.current_deck);
            CurrentDeck.gameObject.SetActive(true);
        }

        public void ShowDrawPile()
        {
            ClearCurrentDeck();
            Start();
            DisableAllMenus();
            DrawAndDiscard.gameObject.SetActive(true);
            LoadPlayerDrawPileAndDiscardPile();
          
        }

        public void ClearCurrentDeck()
        {
            foreach (Transform nextCard in currentCards)
            {
                Destroy(nextCard.gameObject);
            }
            currentCards.Clear();
        }



        public void AddCardMenu(SkillDetail newCard, PodiumManager podium)
        {
            newCardImage.gameObject.SetActive(true);
            var newCardSprite = Resources.Load<Sprite>("ActiveCard" + newCard.skill_sprite_name);
            newCardImage.sprite = newCardSprite;
            ShowAddCards(podium);
            cardBeingAdded = newCard;
        }

        void Update()
        {

        }

        public void AddCard()
        {
            playerManager.current_deck.Add(cardBeingAdded);
            playerManager.draw_pile.Add(cardBeingAdded);
            combatManager.LeaveMenu();
            currentPodium.DisablePodium();
        }

        public void LeaveMenu()
        {
            combatManager.LeaveMenu();
        }

      



    }
}
