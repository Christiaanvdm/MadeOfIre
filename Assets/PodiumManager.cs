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
        private GameObject card;
        private Vector3 originalScale;
        private Light light;
        private CombatManager combatManager;
        private DeckManager deckManager;
        private SkillDetail rewardCard = new SkillDetail();
        private bool isActive = true;
        private SpriteRenderer cardSprite;
        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("Player");
            anim = gameObject.transform.Find("Sprite").gameObject.GetComponent<Animator>();
            card = gameObject.transform.Find("Card").gameObject;
            originalScale = card.transform.localScale;
            light = transform.Find("Light").GetComponent<Light>();
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
            deckManager = Resources.FindObjectsOfTypeAll<DeckManager>()[0];
            GameObject spriteRenderGO = card.transform.Find("Sprite").gameObject;
            cardSprite = spriteRenderGO.GetComponent<SpriteRenderer>();
            cardSprite = card.transform.Find("Sprite").GetComponent<SpriteRenderer>();
            loadCardReward();
        }



        void loadCardReward()
        {
            rewardCard = getRandomSkillDetailFromAllCards();
            var newCardImage = Resources.Load<Sprite>("ActiveCard" + rewardCard.skill_sprite_name);
            cardSprite.sprite = newCardImage;
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
                    EnlargeReward();
                }
                else
                {
                    ShrinkReward();
                }
            }
        }

        void OnMouseDown()
        {

            if (isActive)
            {
            
                // Show add card menu
                deckManager.ShowAddCards(this);
                combatManager.EnterMenu(rewardCard, this);
            }
        
        }

        private void OnMouseOver()
        {
            if (isActive)
            {
                EnlargeReward();
            }
        }

        private void OnMouseExit()
        {

            ShrinkReward();
        }

        private void EnlargeReward()
        {
            card.transform.localScale = originalScale * 2;
            anim.SetBool("Highlight", true);
            turnLightOn();
        }

        private void ShrinkReward()
        {
            card.transform.localScale = originalScale * 1 / 2;
            anim.SetBool("Highlight", false);
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
            card.SetActive(false);
        }
    }
}