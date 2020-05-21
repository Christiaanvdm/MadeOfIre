using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Complete
{
    public class PodiumManager : MonoBehaviour
    {
        private GameObject player;
        private Animator anim;
        private List<GameObject> cards = new List<GameObject>();
        private Vector3 originalScale;
        private Light light;
        private CombatManager combatManager;
        private DeckManager deckManager;
        private SkillDetail rewardCard = new SkillDetail();
        private bool isActive = true;
        private SpriteRenderer cardSprite;
        private float cardScale = 1.5f;
        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("Player");
            anim = gameObject.transform.Find("Sprite").gameObject.GetComponent<Animator>();
            SetupCards();
            light = transform.Find("Light").GetComponent<Light>();
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
            deckManager = Resources.FindObjectsOfTypeAll<DeckManager>()[0];

            loadCardReward();
        }


        private void SetupCards() {
            cards.Add(gameObject.transform.Find("Card1").gameObject);
            cards.Add(gameObject.transform.Find("Card2").gameObject);
            cards.Add(gameObject.transform.Find("Card3").gameObject);
            originalScale = cards[0].transform.localScale;
        }



        void loadCardReward()
        {
            foreach (var card in cards) {
                rewardCard = getRandomSkillDetailFromAllCards();
                var newCardImage = Resources.Load<Sprite>("ActiveCard" + rewardCard.skill_sprite_name);
                card.GetComponentInChildren<SpriteRenderer>().sprite = newCardImage;
                card.GetComponent<PodiumCard>().skillDetail = rewardCard;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public TextAsset AllCards;
        private SkillDetail getRandomSkillDetailFromAllCards()
        {
            //string path = "Assets/AllCards.json";
            //StreamReader reader = new StreamReader(path);
            //string readerString = reader.ReadToEnd();
            //reader.Close();
            string readerString = AllCards.text;
            PlayerState allCards = JsonUtility.FromJson<PlayerState>(readerString);
            return allCards.getRandomCard();
        }

        private void FixedUpdate()
        {
            if (isActive)
            {
                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (distance < 2)
                {
                    EnlargeRewards();
                }
                else
                {
                    ShrinkReward();
                }
            }
        }



        public void ShowAddCards(SkillDetail skillDetail) {
            if (isActive)
            {
                // Show add card menu
                deckManager.ShowAddCards(this);
                combatManager.EnterMenu(skillDetail);
            }
        }

        private void OnMouseOver()
        {
            if (isActive)
            {
                EnlargeRewards();
            }
        }

        private void OnMouseExit()
        {
            ShrinkReward();
        }

        private void EnlargeRewards()
        {
            //foreach (var card in cards) {
            //    card.transform.localScale = originalScale * cardScale;
            //}

            //anim.SetBool("Highlight", true);
            turnLightOn();
        }

        private void ShrinkReward()
        {
            //foreach (var card in cards)
            //{
            //    card.transform.localScale = originalScale * (1 / cardScale);
            //}

            //anim.SetBool("Highlight", false);
            turnLightOff();
        }

        private void turnLightOff()
        {
            try
            {
                light.gameObject.SetActive(false);
            }
            catch
            {

            }
        }

        private void turnLightOn()
        {
            try
            {
                light.gameObject.SetActive(true);
            }
            catch { }
        }


        public void DisablePodium()
        {
            isActive = false;
            foreach (var card in cards) {
                card.SetActive(false);
            }
        }
    }
}