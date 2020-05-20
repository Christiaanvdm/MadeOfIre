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
        private List<Transform> currentCards;
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
            currentCards = new List<Transform>();
            CanvasPages.Clear();
            canvas = gameObject.GetComponent<Canvas>();
            playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
            Transform[] trs = transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform trf in trs)
            {
                if (trf.gameObject.name == "CurrentDeck")
                {
                    CurrentDeck = trf;
                    CanvasPages.Add(CurrentDeck);
                }
                else if (trf.gameObject.name == "SampleCard")
                {
                    cardSample = trf;
                }
                else if (trf.gameObject.name == "NewCard")
                {
                    NewCard = trf;
                    CanvasPages.Add(NewCard);
                }
                else if (trf.gameObject.name == "Keyboard")
                {
                    Keyboard = trf;
                    CanvasPages.Add(Keyboard);
                }
                else if (trf.gameObject.name == "DrawAndDiscard") {
                    DrawAndDiscard = trf;
                    CanvasPages.Add(DrawAndDiscard);
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
                    CanvasPages.Add(CurrentCards);
                }
                else if (trf.gameObject.name == "NewCardSprite")
                {
                    newCardImage = trf.GetComponent<Image>();
                    CanvasPages.Add(newCardImage.transform);
                }
                else if (trf.gameObject.name == "AddCardButton")
                {
                    AddCardButton = trf;
                    CanvasPages.Add(AddCardButton);
                }
            }
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
                newCard.transform.parent = CardsParent;
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
            Start();
            currentPodium = podium;
            ClearCurrentDeck();

            DisableAllMenus();
            NewCard.gameObject.SetActive(true);
            CurrentCards.gameObject.SetActive(true);
            AddCardButton.gameObject.SetActive(true);
            newCardImage.gameObject.SetActive(true);
            LoadListToPannel(CurrentCards, playerManager.current_deck);
        }

        public void DisableAllMenus()
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
            Start();
            ClearCurrentDeck();

            DisableAllMenus();
            LoadListToPannel(CurrentDeck, playerManager.current_deck);
            CurrentDeck.gameObject.SetActive(true);
        }

        public void ShowDrawPile()
        {
            Start();
            ClearCurrentDeck();

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



        public void AddCardMenu(SkillDetail newCard)
        {
            var newCardSprite = Resources.Load<Sprite>("ActiveCard" + newCard.skill_sprite_name);
            newCardImage.sprite = newCardSprite;
            //ShowAddCards(podium);
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
