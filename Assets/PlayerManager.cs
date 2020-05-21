using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

namespace Complete
{
    public class PlayerManager : MonoBehaviour
    {

        private float dodgeRollDuration = 0.69f;
        private float health = 6f;
        //private float maximum_health = 100f;
        private float shields = 0f;
        private CombatMovement2D combatMovement;
        private Animator anim;
        private bool inDodgeRoll = false;
        private float nextUpdateForDodgeRoll = 1f;
        private GameObject character;
        private GameObject player;
        private Vector3 beforeDodgeRollPosition;
        private Rigidbody rigidBody;
        private string current_orientation = "down";
        private CombatManager combatManager;
        private float DamageTimerEnd;
        private bool TakingDamage = false;
        private float nextFlash;
        private bool flashOn = true;
        private float DamageTimerDuration = 0.5f;
        public List<SkillDetail> current_deck = new List<SkillDetail>();
        public List<SkillDetail> draw_pile = new List<SkillDetail>();
        public List<SkillDetail> discard_pile = new List<SkillDetail>();
        private List<GameObject> playerCardGameObjectList = new List<GameObject>();
        private List<SkillManager> playerCardSkillManagerList = new List<SkillManager>();
        private List<CardManager> playerCardManagerList = new List<CardManager>();
        private SpriteRenderer characterSprite;
        private Vector3 spriteOriginalScale;
        public bool ControlsDisabled = false;
        public bool InCombat = false;

        // Start is called before the first frame update
        void Start()
        {
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
            player = GameObject.Find("Player");
            rigidBody = gameObject.GetComponent<Rigidbody>();
            character = GameObject.Find("Character");
            characterSprite = character.GetComponent<SpriteRenderer>();
            combatMovement = gameObject.GetComponent<CombatMovement2D>();
            anim = character.gameObject.GetComponent<Animator>();
            GameObject card1 = GameObject.Find("Card1");
            playerCardGameObjectList.Add(GameObject.Find("Card1"));
            playerCardGameObjectList.Add(GameObject.Find("Card2"));
            playerCardGameObjectList.Add(GameObject.Find("Card3"));
            playerCardGameObjectList.Add(GameObject.Find("Card4"));
            foreach (GameObject nextGO in playerCardGameObjectList)
            {
                playerCardSkillManagerList.Add(nextGO.GetComponent<SkillManager>());
            }
            foreach (GameObject nextGO in playerCardGameObjectList)
            {
                playerCardManagerList.Add(nextGO.GetComponent<CardManager>());
            }
            LoadPlayerCards();
            spriteOriginalScale = character.transform.localScale;

        }
        // Update is called once per frame
        void Update()
        {
            if ((!combatManager.GamePaused) && (!ControlsDisabled))
            {
                updateOrientation();
            }
            SetEnvironmentSprites();
        }
        private void FixedUpdate()
        {
            checkDodgeRoll();
            if (TakingDamage)
            {
                if (Time.time > nextFlash)
                {
                    nextFlash = Time.time + (DamageTimerDuration / 4);
                    if (flashOn)
                    {
                        turnSpriteTransparent();
                        flashOn = false;
                    }
                    else
                    {
                        turnSpriteVisible();
                        flashOn = true;
                    }

                }
                if (Time.time > DamageTimerEnd)
                {
                    TakingDamage = false;
                    turnSpriteVisible();
                    flashOn = true;
                }
            }

        }

        private void turnSpriteTransparent()
        {
            characterSprite.color = new Color(255, 255, 255, 0);
        }

        private void turnSpriteVisible()
        {
            characterSprite.color = new Color(255, 255, 255, 1);
        }


        private void updateOrientation()
        {

            // Look at mouse
            Vector3 mouseRelativeToPlayer = FindMousePointRelativeToPlayerClean() - player.gameObject.transform.position;

            float mouseAngle = getForward360Angle(mouseRelativeToPlayer.normalized);
            if ((mouseAngle >= 0) && (mouseAngle < 22.5)) {
                lookUp();
            }
            else if ((mouseAngle >= 22.5) && (mouseAngle < 67.5))
            {
                lookRightUp();
            }
            else if ((mouseAngle >= 67.5) && (mouseAngle < 157.5))
            {
                lookRight();
            }
            else if ((mouseAngle >= 157.5) && (mouseAngle < 202.5))
            {
                lookDown();
            }
            else if ((mouseAngle >= 202.5) && (mouseAngle < 292.5))
            {
                lookLeft();
            }
            else if ((mouseAngle >= 292.5) && (mouseAngle < 337.5))
            {
                lookLeftUp();
            }
            else if (mouseAngle >= 337.5)
            {
                lookUp();
            }
        }

        private void lookUp()
        {

            if (current_orientation != "up")
            {
                //anim.parameters[1].defaultBool = true;
                //anim.SetBool("Up", true);
                //anim.SetBool("Right", false);
                //anim.SetBool("Left", false);
                anim.SetInteger("State", 1);
                current_orientation = "up";
            }
        }

        private void lookDown()
        {
            if (current_orientation != "down")
            {
                //anim.SetBool("Down", true);
                //anim.SetBool("Up", false);
                //anim.SetBool("Right", false);
                //anim.SetBool("Left", false);
                anim.SetInteger("State", 3);
                current_orientation = "down";
            }
        }

        private void lookRight()
        {
            if (current_orientation != "right")
            {
                anim.SetInteger("State", 2);
                current_orientation = "right";

                character.transform.localScale = spriteOriginalScale;
            }
        }

        private void lookLeft()
        {
            if (current_orientation != "left")
            {
                anim.SetInteger("State", 2);
                current_orientation = "left";
                Vector3 newScale = new Vector3(spriteOriginalScale.x * -1, spriteOriginalScale.y, spriteOriginalScale.z);
                character.transform.localScale = newScale;
            }
        }

        private void lookLeftUp()
        {
            if (current_orientation != "leftUp")
            {
                anim.SetInteger("State", 4);
                current_orientation = "leftUp";
                Vector3 newScale = new Vector3(spriteOriginalScale.x * -1, spriteOriginalScale.y, spriteOriginalScale.z);
                character.transform.localScale = newScale;
            }
        }

        private void lookRightUp()
        {
            if (current_orientation != "rightUp")
            {
                anim.SetInteger("State", 4);
                current_orientation = "rightUp";
                Vector3 newScale = spriteOriginalScale;
                character.transform.localScale = newScale;
            }
        }


        Vector3 FindMousePointRelativeToPlayer()
        {
            Vector3 mousePointOnFloor = FindMousePointOnFloor();
            mousePointOnFloor = new Vector3(mousePointOnFloor.x, mousePointOnFloor.y + player.transform.position.y, mousePointOnFloor.z);
            return mousePointOnFloor;

        }

        Vector3 FindMousePointRelativeToPlayerClean()
        {
            Vector3 mousePointOnFloor = FindMousePointOnFloor();
            mousePointOnFloor = new Vector3(mousePointOnFloor.x, mousePointOnFloor.y, mousePointOnFloor.z);
            return mousePointOnFloor;

        }

        Vector3 FindMousePointOnFloor()
        {
            Vector3 mousePointOnFloor = new Vector3(0, 0, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "ground")
                {
                    mousePointOnFloor = hit.point;
                }
            }
            return mousePointOnFloor;

        }

        private void checkDodgeRoll()
        {
            if (inDodgeRoll)
            {
                if (Time.time >= nextUpdateForDodgeRoll)
                {
                    EndDodgeRoll();
                    inDodgeRoll = false;
                }
            }
        }
        public void Damage(float amount, string type)
        {
            if (type == "blunt")
            {
                health = health + shields - amount;
                if (shields > amount)
                {
                    shields = shields - amount;
                }
                else
                {
                    shields = 0;
                }
                print("Player took " + amount.ToString() + " points of damage. Health is currently " + health.ToString());
            }
            flashPlayerSprite();
            UpdateHUDHealth(health);
            CheckForDeath();

        }

        private void CheckForDeath()
        {
            if (health <= 0) {
                combatManager.Death();
            }
        }


        private void flashPlayerSprite()
        {
            DamageTimerEnd = Time.time + DamageTimerDuration;
            TakingDamage = true;
            nextFlash = Time.time;
        }

        private void UpdateHUDHealth(float health)
        {
            int ihealth;
            if (int.TryParse(health.ToString(), out ihealth))
            {
                combatManager.UpdateHUDHealth(ihealth);
            }

        }
        private void OnCollisionEnter(Collision collision)
        {
            GameObject colliderGO = collision.collider.gameObject;
            if (colliderGO.tag == "enemy_projectile")
            {
                    AttackProjectile enemyAP = colliderGO.GetComponent<AttackProjectile>();

                    if (enemyAP)
                    {
                        Damage(enemyAP.damage, "blunt");
                    }
            }
            if (colliderGO.tag == "enemy")
            {
                Damage(1, "blunt");
            }
        }

        public void DodgeRoll()
        {

            dodgeRoll();
        }

        private void DodgeRollUp()
        {
            anim.SetInteger("State", 11);

        }
        private void DodgeRollRightUp()
        {
            character.transform.localScale = spriteOriginalScale;
            anim.SetInteger("State", 12);
        }
        private void DodgeRollRight()
        {
            character.transform.localScale = spriteOriginalScale;
            anim.SetInteger("State", 13);
        }
        private void DodgeRollRightDown()
        {
            character.transform.localScale = spriteOriginalScale;
            anim.SetInteger("State", 14);
        }
        private void DodgeRollDown()
        {
            anim.SetInteger("State", 15);
        }
        private void DodgeRollLeftUp()
        {
            Vector3 newScale = new Vector3(spriteOriginalScale.x * -1, spriteOriginalScale.y, spriteOriginalScale.z);
            character.transform.localScale = newScale;
            anim.SetInteger("State", 12);
        }

        private void DodgeRollLeft()
        {
            Vector3 newScale = new Vector3(spriteOriginalScale.x * -1, spriteOriginalScale.y, spriteOriginalScale.z);
            character.transform.localScale = newScale;
            anim.SetInteger("State", 13);
        }
        private void DodgeRollLeftDown()
        {
            Vector3 newScale = new Vector3(spriteOriginalScale.x * -1, spriteOriginalScale.y, spriteOriginalScale.z);
            character.transform.localScale = newScale;
            anim.SetInteger("State", 14);
        }


        IEnumerator DodgeRollCoroutine()
        {
            for (; ; )
            {
                //Vector3 force = rigidBody.velocity.normalized * dodgeRollForce;
                rigidBody.velocity = rigidBody.velocity * 0.93f;
                yield return new WaitForSeconds(0.05f);
            }
        }


        private void dodgeRoll()
        {

            if (!inDodgeRoll)
            {
                nextUpdateForDodgeRoll = Time.time + dodgeRollDuration;
                StartDodgeRoll();
                inDodgeRoll = true;

            }

            float direction = getForward360Angle(rigidBody.velocity.normalized);
            if ((direction >= 0) && (direction < 22.5))
            {
                DodgeRollUp();
            }
            else if ((direction >= 22.5) && (direction < 67.5))
            {
                DodgeRollRightUp();
            }
            else if ((direction >= 67.5) && (direction < 112.5))
            {
                DodgeRollRight();
            }
            else if ((direction >= 112.5) && (direction < 157.5))
            {
                DodgeRollRightDown();
            }

            else if ((direction >= 157.5) && (direction < 202.5))
            {
                DodgeRollDown();
            }
            else if ((direction >= 202.5) && (direction < 247.5))
            {
                DodgeRollLeftDown();
            }
            else if ((direction >= 247.5) && (direction < 292.5))
            {
                DodgeRollLeft();
            }
            else if ((direction >= 292.5) && (direction < 337.5))
            {
                DodgeRollLeftUp();
            }
            else if (direction >= 337.5)
            {
                DodgeRollUp();
            }


              float speedAngle = getForward360Angle(rigidBody.velocity.normalized);



        }

        private float getForward360Angle(Vector3 normalVector)
        {
            if (normalVector.x >= 0)
            {
                return Vector3.Angle(new Vector3(0, 0, 1), normalVector);
            }
            else if (normalVector.x < 0)
            {
                return 360f - Vector3.Angle(new Vector3(0, 0, 1), normalVector);
            }
            else return 0;
        }

      public void StartDodgeRoll()
        {
            gameObject.layer = 15;
            combatMovement.SetAllKeysAsUp();
            current_orientation = "DR";
             ControlsDisabled = true;
            rigidBody.velocity = rigidBody.velocity * 1.2f;
            StartCoroutine("DodgeRollCoroutine");

            //     character.transform.position = character.transform.position + new Vector3(0, 0f, 0.5f);
        }

        public void EndDodgeRoll()
        {
            gameObject.layer = 8;
            ControlsDisabled = false;
            combatMovement.SetAllKeysAsUp();
            combatMovement.characterStopped();
            current_orientation = "DR";
            //character.transform.position = character.transform.position + new Vector3(0, 0f, -0.5f);
            updateOrientation();
            StopCoroutine("DodgeRollCoroutine");
        }

        private void TakeDamage()
        {

        }

        public TextAsset playerCards;

        public void LoadPlayerCards()
        {

            string readerString = "";
            if (!PlayerPrefs.HasKey("Cards"))
            {
                readerString = playerCards.text;
                PlayerPrefs.SetString("Cards", readerString);
            }
            else {
                readerString = PlayerPrefs.GetString("Cards");
            }

            current_deck.Clear();
            PlayerState playerState = JsonUtility.FromJson<PlayerState>(readerString);
            current_deck = playerState.current_deck;
            draw_pile = current_deck.OrderBy(x => Random.value).ToList();
            StartCoroutine("slightlyAfterStart");
            discard_pile.Clear();
        }

        IEnumerator slightlyAfterStart()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            foreach (CardManager cm in playerCardManagerList)
            {
                cm.DiscardCard(true, false);
            }
            yield return null;
        }

        public void DrawCard(string cardIdentifier, bool AddToDiscardPile = true)
        {
             if (draw_pile.Count == 0)
            {
                ShuffleDiscardToDrawPile();
            }

            GameObject nextSlot = GameObject.Find(cardIdentifier);
            SkillManager oldSkillManager = new SkillManager();
            oldSkillManager = nextSlot.GetComponent<SkillManager>();
            SkillDetail oldSkillDetail = convertSkillManagerToSkillDetail(oldSkillManager);
            if (AddToDiscardPile)
            {
                discard_pile.Add(oldSkillDetail);
            }
            SkillManager skillManager = nextSlot.GetComponent<SkillManager>();
            SkillDetail nextCard = draw_pile[0];
            SpriteRenderer spriteActiveCardRenderer = nextSlot.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
            SpriteRenderer spriteInactiveCardRenderer = nextSlot.transform.Find("InactiveCard").gameObject.GetComponent<SpriteRenderer>();
            var ActiveSprite = Resources.Load<Sprite>("ActiveCard" + nextCard.skill_sprite_name);
            var InActiveSprite = Resources.Load<Sprite>("InactiveCard" + nextCard.skill_sprite_name);
            spriteActiveCardRenderer.sprite = ActiveSprite;
            spriteInactiveCardRenderer.sprite = InActiveSprite;
            skillManager.skillDetail = nextCard;
            skillManager.duration = nextCard.duration;
            skillManager.magnitude = nextCard.magnitude;
            skillManager.description = nextCard.description;
            skillManager.type = nextCard.type;
            skillManager.skill_sprite_name = nextCard.skill_sprite_name;
            skillManager.cooldown = nextCard.cooldown;
            skillManager.requiresTarget = nextCard.requiresTarget;

            draw_pile.Remove(nextCard);
        }

        private void ShuffleDiscardToDrawPile()
        {
            draw_pile.AddRange(discard_pile);
            discard_pile.Clear();
        }

        public void SavePlayerCards()
        {
            PlayerState playerState = new PlayerState();
            SkillDetail newSkillDetail = new SkillDetail();
            newSkillDetail.description = "Some description";
            playerState.current_deck = current_deck;
            PlayerPrefs.SetString("Cards", JsonUtility.ToJson(playerState));
        }


        public void SavePlayerCardsToFile()
        {
            PlayerState playerState = new PlayerState();
            SkillDetail newSkillDetail = new SkillDetail();
            newSkillDetail.description = "Some description";
            playerState.current_deck = current_deck;
            string path = "PlayerCards.json";
            StreamWriter writer = new StreamWriter(path, true);
            writer.Write(JsonUtility.ToJson(playerState));
            writer.Close();
        }

        public void DiscardHand()
        {
            foreach (GameObject card in playerCardGameObjectList)
            {

                card.GetComponent<CardManager>().DiscardCard();
            }
        }

        public SkillDetail convertSkillManagerToSkillDetail(SkillManager skillManager)
        {
            SkillDetail sd = new SkillDetail();
            sd.cooldown = skillManager.cooldown;
            sd.description = skillManager.description;
            sd.duration = skillManager.duration;
            sd.magnitude = skillManager.magnitude;
            sd.projectileSize = skillManager.projectileSize;
            sd.projectileSpeed = skillManager.projectileSpeed;
            sd.skill_name = skillManager.skill_name;
            sd.skill_sprite_name = skillManager.skill_sprite_name;
            sd.type = skillManager.type;
            return sd;
        }

        public SkillManager convertSkillDetailToSkillManager(SkillDetail skillDetail)
        {
            SkillManager skillManager = new SkillManager();
            skillManager.cooldown = skillDetail.cooldown;
            skillManager.description = skillDetail.description;
            skillManager.duration = skillDetail.duration;
            skillManager.magnitude = skillDetail.magnitude;
            skillManager.projectileSize = skillDetail.projectileSize;
            skillManager.projectileSpeed = skillDetail.projectileSpeed;
            skillManager.skill_name = skillDetail.skill_name;
            skillManager.skill_sprite_name = skillDetail.skill_sprite_name;
            skillManager.type = skillDetail.type;
            return skillManager;
        }

        public CardManager getCardByIndex(int i) {
            return playerCardManagerList[i];
        }


        public void AdvanceConversation()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3);
            foreach (Collider collider in hitColliders) {
                if (collider.gameObject.tag == "npc") {
                    collider.transform.GetComponent<NPCController>().advanceStep();
                }
            }
        }


        private void SetEnvironmentSprites()
        {
                                RaycastHit hit;
            var cameraPos = Camera.main.transform.position;
            if (Physics.Raycast(cameraPos, transform.position - cameraPos, out hit, Mathf.Infinity))
            {

                //if (hit.collider.gameObject.tag == "player")
                //{
                //    character.GetComponent<SpriteRenderer>().sortingLayerID = 2;
                //    //var renderer = transform.Find("Sphere").GetComponent<Renderer>();
                //    //if (seeThroughSize > 0) {
                //    //    seeThroughSize -= seeThroughSizeScaleRate * Time.deltaTime;
                //    //}
                //    //renderer.material.SetFloat("_ScaleX", seeThroughSize);
                //    //renderer.material.SetFloat("_ScaleY", seeThroughSize);
                //}
                //else {
                //    character.GetComponent<SpriteRenderer>().sortingLayerID = 0;
                //    //var renderer = transform.Find("Sphere").GetComponent<Renderer>();
                //    //if (seeThroughSize < 2)
                //    //{
                //    //    seeThroughSize += seeThroughSizeScaleRate * Time.deltaTime;
                //    //}
                //    //renderer.material.SetFloat("_ScaleX", seeThroughSize);
                //    //renderer.material.SetFloat("_ScaleY", seeThroughSize);
                //}
            }
        }

        private float seeThroughSizeScaleRate = 3;
        private float seeThroughSize = 0;
    }
}