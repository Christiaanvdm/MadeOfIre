using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;


namespace Complete {
    public class CardManager : MonoBehaviour
    {


        private SkillManager skillManager;
        private SkillManager_Attack skillManager_Attack;
        private CombatManager combatManager;
        public int cardType;                                        

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Rigidbody rigidBody;
   
        public Vector3 screenPoint;
        public Vector3 offset;
        private EnemyManager enemyManager;
        private float _lockedYPosition;
        public bool isDraggingThisCard;
        public Vector3 initialScale;
        public Vector3 initialOffset;
        //private bool drawingCard = false;
        private GameObject discardStorage;
        private float scaleMultiplier = 0.25f;
        private Canvas canvasCards;

        private float discardTime = 1f;
        public float cooldown = 1.5f;

        private GameObject canvasCardsGameObject;
        private GameObject canvasCardGO;
        private Image canvasCardImage;
        private PlayerManager playerManager;

        private bool isActive = true;
        private Transform cooldownTransform;
        private Vector3 initialCooldownScale;


        // Start is called before the first frame update
        void Start()
        {
            if (cardType == 1) {
                cooldownTransform = transform.Find("Cooldown");
                initialCooldownScale = cooldownTransform.localScale;
            }

            canvasCardsGameObject = GameObject.Find("HUDUICanvasCards");
            
            try
            {
                canvasCardGO = canvasCardsGameObject.transform.Find(this.gameObject.name + "CanvasCard").gameObject;
                canvasCardImage = canvasCardGO.GetComponent<Image>();
                canvasCards = GameObject.Find("HUDUICanvasCards").GetComponent<Canvas>();
                canvasCardImage.fillAmount = 0;
            }
            catch
            {

            }


            playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();

            initialScale = transform.localScale;
            offset = new Vector3(-0.4f - 0.3f, -0.4f);
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
            skillManager = gameObject.GetComponent<SkillManager>();
            rigidBody = gameObject.GetComponent<Rigidbody>();
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
            SnapToAttention();

                                                        

        }

        // Update is called once per frame
        void Update()
        {
        
            //CheckIfEnemyHit();
            if (isDraggingThisCard)
            {
                onDraggingThisCard();
            }
           

        }

        private float cooldownScale = 1;

        private void CooldownCard() 
        {
            if (cooldownTransform != null)
            {
                cooldownScale -= 1 / skillManager.cooldown * Time.deltaTime;
                if (cooldownScale < 0.001)
                    cooldownScale = 0;
                var cooldownFactor = cooldownScale;

                cooldownTransform.localScale = new Vector3(initialCooldownScale.x, initialCooldownScale.y * cooldownScale, initialCooldownScale.z   );
            }  
        }

        private void FixedUpdate()
        {
            if (!isActive)
            {
                if (!this.IsActive())
                {
                    CooldownCard();
                    
                }
                if (Time.time > discardTime)
                {
                    EnableCard();
                }
            }
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void EnableCard()
        {
            isActive = true;
            transform.localScale = initialScale;
            offset = initialOffset;
            //gameObject.transform.Find("CardBase").gameObject.GetComponent<Renderer>().enabled = true;
            gameObject.transform.Find("Sprite").gameObject.GetComponent<Renderer>().enabled = true;
        }

        public void DisableCard()
        {
            cooldownScale = 1;
            //canvasCardImage.fillAmount = 1;
            isActive = false;
            SnapToAttention();
            discardTime = Time.time + skillManager.cooldown;
            
            //gameObject.transform.Find("CardBase").gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.transform.Find("Sprite").gameObject.GetComponent<Renderer>().enabled = false;
        }
        public void SetOffSet()
        {

            //offset = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            offset = new Vector3(-0.4f, -0.3f, -0.4f);
            //screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        }
       
        private bool mouse_is_over;
        private bool mouse_is_over_enemy;
        private bool tick_tock;
        private void SnapToAttention()
        {
            //try
            //{
                float max = 100f;
                float min = 75f;
                float random_x = Random.Range(min, max) / 1000f;
                if (Random.value > 0.5)
                    random_x = random_x * -1;
                float random_y = Random.Range(min, max) / 1000f;
                if (Random.value > 0.5)
                    random_y = random_y * -1;
                float random_z = Random.Range(min, max) / 1000f;
                if (Random.value > 0.5)
                    random_z = random_z * -1;
                transform.localRotation = initialRotation;
                transform.localPosition = initialPosition;
                //rigidBody.angularVelocity = new Vector3(0, 0, 0);

                rigidBody.velocity = new Vector3(0, 0, 0);
                //rigidBody.AddTorque(new Vector3(random_x, random_y, random_z));
            //}
            //catch
            //{

            //}

        }
        Vector3 start_pos;
        private Vector3 cardPositionBeforeDrag;
       
         public void OnMouseDown()
        {
            if (isActive)
            {
                cardPositionBeforeDrag = transform.localPosition;
                screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, screenPoint.y, Input.mousePosition.y));
                initialOffset = offset;
                combatManager.isDraggingACard = true;
                isDraggingThisCard = true;
            }
        }



        private void OnMouseDrag()
        {

        }

        private void onDraggingThisCard()
        {
            if (isActive) { 

                transform.localScale = initialScale * scaleMultiplier;
                //transform.localScale = initialScale * scaleMultiplier;
                offset = initialOffset * scaleMultiplier;
                Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, screenPoint.y, Input.mousePosition.z);
                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
                transform.position = cursorPosition;
                start_pos = new Vector3(Input.mousePosition.x, screenPoint.y, Input.mousePosition.z);
            }
        }

        private bool mouse_is_over_card = false;

   
        public void OnMouseUp()
        {
            if (isActive)
            {
                if (!skillManager)
                {
                    skillManager = gameObject.GetComponent<SkillManager>();
                }
                skillManager.SkillWithoutEnemy();
                gameObject.transform.localPosition = cardPositionBeforeDrag;
                isDraggingThisCard = false;
                if (combatManager) { 
                    combatManager.isDraggingACard = false;
                }

                

                transform.localScale = initialScale;
                offset = initialOffset;
                SnapToAttention();

            }
        }

        public void DiscardCard(bool ShoulDrawCard = true, bool ShouldAddToDiscardPile = true)
        {
            if (ShoulDrawCard)
            {
                DrawCard(ShouldAddToDiscardPile);
            }
            if (isActive)
            {
                DisableCard();
            }
        }

        public void LoadSprite()
        {

        }

        public void DrawCard(bool shouldAddtoDiscardPile = true)
        {
            string cardIdentifier = this.gameObject.name;
            playerManager.DrawCard(cardIdentifier, shouldAddtoDiscardPile);

        }

        private void startDrawCard()
        {
            
        }
    }

   
}