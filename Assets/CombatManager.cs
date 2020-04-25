using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

namespace Complete
{

    public class CombatManager : MonoBehaviour
    {
        //private float nextUpdate = 1; //In seconds
        //private float nextUpdate5 = 1; //In seconds
        //private float updateSize = 0.1f;
        private bool Key1 = false;
        private bool Key2 = false;
        private bool Key3 = false;
        private bool Key4 = false;
        private bool Mouse1 = false;
        private bool Mouse2 = false;
        private float nextUpdate = 1f;
        private float duration = 0.5f;
        private bool inMenu = false;
        private float iconWidth = 64;

        public bool isDraggingACard = false;
        private int uniqueSpellID = 0;
        private int uniqueSkillModifierID = 0;
        private GameObject player;
        private PlayerManager playerManager;
        public List<AttackModifier> AttackModifierList;
        public List<AttackModifier> SkillModifierList;
        public List<AttackModifier> TerrainModifierList;
        private CardManager terrainCard;
        private int playerHealth;

        public GameObject canvasGameObject;

        public GameObject terrainPlacement;
        private Sprite currentTerrain;
        public Vector3 screenPoint;
        public Color currentColor;
        public bool isPlacingTerain = false;
        public Rigidbody terrainTemplate;
        GameObject healthSample;
        private List<GameObject> healthIcons;
        private GameObject DeckManagerGO;
        private DeckManager deckManager;
        private DeathScreen deathScreen;

        Quaternion initialOrientation;
        // Start is called before the first frame update
        void Start()
        {

            deathScreen = Resources.FindObjectsOfTypeAll<DeathScreen>()[0];
            deathScreen.gameObject.SetActive(false);
            DeckManagerGO = Resources.FindObjectsOfTypeAll<DeckManager>()[0].gameObject;
            DeckManagerGO.SetActive(false);
            isPlacingTerain = false;
            canvasGameObject = GameObject.Find("HUDUICanvas");
            AttackModifierList = new List<AttackModifier>();
            SkillModifierList = new List<AttackModifier>();
            player = GameObject.Find("Player");
            playerManager = player.gameObject.GetComponent<PlayerManager>();
            terrainPlacement = GameObject.Find("TerrainPlacement");
            terrainTemplate = GameObject.Find("TerrainTemplate").GetComponent<Rigidbody>();
            healthIcons = new List<GameObject>();
            healthSample = GameObject.Find("Health");
            deckManager = DeckManagerGO.GetComponent<DeckManager>();
            UpdateHUDHealth(6);


        }

        // Update is called once a frame
        void Update()
        {
            if (!GamePaused)
            {
                checkKey();
            }
            else
            {
                checkMenuKeys();
            }
            if (isPlacingTerain)
            {
                PlacingTerrain();
            }
        }


        public void PlacingTerrain()
        {
            screenPoint = Camera.main.WorldToScreenPoint(terrainPlacement.transform.position);
            Vector3 cursorScreenPoint = FindMousePointRelativeToPlayer();
            cursorScreenPoint.y = 0;
            //Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint);
            terrainPlacement.transform.position = cursorScreenPoint;

        }
        public bool GamePaused = false;
        public void PauseGame()
        {
            Time.timeScale = 0;
            GamePaused = true;

        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
            GamePaused = false;
        }

        public void ModifyProjectile(ref AttackProjectile projectile)
        {
            projectile.allAttackModifiers.AddRange(AttackModifierList);
            foreach (AttackModifier nextAM in AttackModifierList)
            {
                if (projectile.GetType() == typeof(AttackProjectile))
                {
                    if (nextAM.type == "double_size")
                    {
                        projectile.rigidBody.gameObject.transform.localScale = projectile.rigidBody.gameObject.transform.localScale * nextAM.magnitude;
                    }
                    if (nextAM.type == "double_damage")
                    {
                        //ToDo Make fireball!
                        UpdateProjectileColor(projectile, new Color(1, 0, 0));
                    }
                    if (nextAM.type == "half_speed")
                    {
                        ApplySlowToProjectile(ref projectile, nextAM);
                    }
                    if (nextAM.type == "split_shot")
                    {
                        ApplySplitToProjectile(ref projectile, nextAM);
                    }
                    if (nextAM.type == "chain_shot")
                    {
                        ApplyChainToProjectile(ref projectile, nextAM);
                    }

                }
            }
        }

        private void ApplyChainToProjectile(ref AttackProjectile projectile, AttackModifier nextAM)
        {
            nextAM.context = "Hit";
            projectile.AddModifier(nextAM);
        }
        public void UpdateHUDHealth(int newHP)
        {
            int healthCount = healthIcons.Count;
            if (newHP > healthCount)
            {
                while (newHP > healthIcons.Count)
                {
                    Vector3 offset = new Vector3(27 + (iconWidth * 0.5f * (healthIcons.Count + 1)), 0, 0);
                    GameObject newHealthIcon = Instantiate(healthSample, healthSample.transform.position + offset, healthSample.transform.rotation);
                    newHealthIcon.transform.SetParent(canvasGameObject.transform);
                    healthIcons.Add(newHealthIcon);
                }
            }
            else if (newHP < healthCount)
            {
                while (newHP < healthIcons.Count)
                {
                    GameObject GOToDestroy = healthIcons[healthIcons.Count - 1];
                    GOToDestroy.GetComponent<Animator>().SetBool("Empty", true);
                    healthIcons.Remove(GOToDestroy);
                    //Destroy(GOToDestroy);
                }
            }

        }

        private void ApplySplitToProjectile(ref AttackProjectile projectile, AttackModifier nextAM)
        {
            nextAM.context = "Birth";
            projectile.AddModifier(nextAM);
        }

        private void ApplySlowToProjectile(ref AttackProjectile projectile, AttackModifier nextAM)
        {
            nextAM.context = "Enemy";
            projectile.AddModifier(nextAM);
        }

        //private bool firstUpdate = false;
        private void FixedUpdate()
        {
            if (Time.time >= nextUpdate)
            {
                nextUpdate = Time.time + duration;
                UpdateEveryDuration();
            }
        }

        private void UpdateEveryDuration()
        {

        }

        void checkMenuKeys()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                EscDown();

            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                EscUp();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                SpaceDown();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                SpaceUp();
            }
            if (Input.GetKey(KeyCode.Return))
            {
                ReturnDown();
            }
            else if (Input.GetKeyUp(KeyCode.Return))
            {
                ReturnUp();
            }
        }

        void SpaceDown()
        {

        }

        void CDown() {
            playerManager.AdvanceConversation();
        }

        void CUp() { }

        void ReturnDown() {

        }

        void ReturnUp() {

        }

        void checkKey()
        {
            if (playerManager)
            //&& playerManager.InCombat)
            {
                if (Input.GetKey(KeyCode.Q))
                {
                    Key1Down();

                }
                else if (Input.GetKeyUp(KeyCode.Q))
                {
                    Key1Up();
                }
                if (Input.GetKey(KeyCode.E))
                {
                    Key2Down();

                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    Key2Up();
                }

                if (Input.GetKey(KeyCode.R))
                {
                    Key3Down();

                }
                else if (Input.GetKeyUp(KeyCode.R))
                {
                    Key3Up();
                }

                if (Input.GetKey(KeyCode.F))
                {
                    Key4Down();

                }
                else if (Input.GetKeyUp(KeyCode.F))
                {
                    Key4Up();
                }

            }
            if (Input.GetMouseButtonDown(0))
            {
                Mouse1Down();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Mouse1Up();
            }

            if (Input.GetMouseButtonDown(1))
            {
                Mouse2Down();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Mouse2Up();
            }
            if (Input.GetKey(KeyCode.Escape))
            {
                EscDown();

            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                EscUp();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                SpaceDown();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                SpaceUp();
            }
            if (Input.GetKey(KeyCode.F1))
            {
                F1Down();
            }
            else if (Input.GetKeyUp(KeyCode.F1))
            {
                F1Up();
            }
            if (Input.GetKey(KeyCode.C))
            {
                CDown();
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                CUp();
            }

        }

        void F1Down()
        {

        }

        void F1Up()
        {
            if (inMenu)
            {
                LeaveMenu();
            }
            else
            {
                deckManager.ShowKeyboard();
                EnterMenu();
            }
        }

        void EscDown()
        {
        }

        void SpaceUp()
        {
            if (inMenu)
            {
                LeaveMenu();
            }
            else
            {
                deckManager.ShowDrawPile();
                EnterMenu();
            }
        }

        public void LeaveMenu()
        {

            DeckManagerGO.SetActive(false);
            UnpauseGame();
            inMenu = false;
        }

        public void EnterMenu()
        {
            PauseGame();
            DeckManagerGO.SetActive(true);
            inMenu = true;
        }

        public void EnterMenu(SkillDetail newCard, PodiumManager podium)
        {
            PauseGame();
            DeckManagerGO.SetActive(true);
            inMenu = true;
            deckManager.AddCardMenu(newCard, podium);
        }

        public void Death()
        {
            //PauseGame();
            //inMenu = true;
            deathScreen.gameObject.SetActive(true);
        }

        void EscUp()
        {

            if (inMenu)
            {
                LeaveMenu();
            }
            else
            {
                deckManager.ShowCurrentDeck();
                EnterMenu();
            }
        }

        void Key1Down()
        {
            Key1 = true;
            CancelTerrainPlacement();
            GameObject card1 = GameObject.Find("Card1");
            CardManager cardManager = card1.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.SetOffSet();
            cardManager.isDraggingThisCard = true;

        }

        void Key1Up()
        {
            Key1 = false;
            GameObject card1 = GameObject.Find("Card1");
            CardManager cardManager = card1.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            cardManager.OnMouseUp();

        }

        void Key2Down()
        {
            Key2 = true;
            CancelTerrainPlacement();
            GameObject card2 = GameObject.Find("Card2");
            CardManager cardManager = card2.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.isDraggingThisCard = true;

        }

        void Key2Up()
        {
            Key2 = false;
            GameObject card2 = GameObject.Find("Card2");
            CardManager cardManager = card2.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            cardManager.OnMouseUp();

        }

        void Key3Down()
        {
            Key3 = true;
            CancelTerrainPlacement();
            GameObject card3 = GameObject.Find("Card3");
            CardManager cardManager = card3.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.isDraggingThisCard = true;

        }

        void Key3Up()
        {
            Key3 = false;
            GameObject card3 = GameObject.Find("Card3");
            CardManager cardManager = card3.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            cardManager.OnMouseUp();

        }

        void Key4Down()
        {
            Key4 = true;

            CancelTerrainPlacement();
            GameObject card4 = GameObject.Find("Card4");
            CardManager cardManager = card4.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.isDraggingThisCard = true;

        }

        void Key4Up()
        {
            Key4 = false;
            GameObject card4 = GameObject.Find("Card4");
            CardManager cardManager = card4.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            cardManager.OnMouseUp();

        }

        void Mouse1Down()
        {
            Mouse1 = false;
            GameObject cardB = GameObject.Find("CardB");
            CardManager cardManager = cardB.GetComponent<CardManager>();
            //isDraggingACard = true;
            cardManager.isDraggingThisCard = true;
            if (isPlacingTerain)
            {
                PlaceTerrain();
            }
        }

        void Mouse1Up()
        {
            Mouse1 = true;
            GameObject cardB = GameObject.Find("CardB");
            CardManager cardManager = cardB.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            cardManager.OnMouseUp();
        }

        void Mouse2Down()
        {
            Mouse2 = false;
            GameObject cardDR = GameObject.Find("CardDR");
            CardManager cardManager = cardDR.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.isDraggingThisCard = true;
            CancelTerrainPlacement();
        }

        void Mouse2Up()
        {
            Mouse2 = true;
            GameObject cardDR = GameObject.Find("CardDR");
            CardManager cardManager = cardDR.GetComponent<CardManager>();
            //cardManager.isDraggingThisCard = false;
            //isDraggingACard = false;
            cardManager.OnMouseUp();
        }


        public void StartPlacingTerrain(CardManager card)
        {
            terrainCard = card;
            terrainPlacement.gameObject.SetActive(true);
            isPlacingTerain = true;
        }

        private void CancelTerrainPlacement()
        {
            isPlacingTerain = false;
            terrainPlacement.gameObject.SetActive(false);
        }

        private void UpdateProjectileColor(AttackProjectile projectile, Color color)
        {
            //projectile.UpdateColor(color);

        }

        public void addAttackModifier(AttackModifier newAM, CardManager originCard)
        {
            uniqueSpellID++;

            newAM.id = uniqueSpellID;

            GameObject skillCooldownSample = new GameObject();
            skillCooldownSample = GameObject.Find("SkillCooldown");
            int thisAMCount = AttackModifierList.Count + 1;

            newAM.skillCooldownClone = Instantiate(skillCooldownSample, canvasGameObject.transform);
            //newAM.skillCooldownClone.gameObject.transform.SetPositionAndRotation(new Vector3(11, -277, 0),
            //newAM.skillCooldownClone.gameObject.transform.rotation);
            newAM.skillCooldownClone.gameObject.transform.SetPositionAndRotation(new Vector3(
                newAM.skillCooldownClone.gameObject.transform.position.x + (thisAMCount * iconWidth),
                newAM.skillCooldownClone.gameObject.transform.position.y + 135,
                newAM.skillCooldownClone.gameObject.transform.position.z),
                newAM.skillCooldownClone.gameObject.transform.rotation);

            if (SkillModifierList.Count > 0)
            {
                ModifyAttackModifier(newAM);
            }

            Image spriteIcon = newAM.skillCooldownClone.transform.Find("CooldownIcon").gameObject.GetComponent<Image>();
            var skillSprite = Resources.Load<Sprite>(newAM.icon_name);
            spriteIcon.sprite = skillSprite;

            AttackModifierList.Add(newAM);
            newAM.startTimer(newAM, null, this);
            originCard.DiscardCard();

        }

        public void ModifyAttackModifier(AttackModifier amToModify)
        {
            // Apply the amplification
            foreach (AttackModifier sm in SkillModifierList)
            {
                if ((sm.type == "double_duration") & (sm.is_enabled == true))
                {
                    amToModify.duration = amToModify.duration * sm.magnitude;
                    amToModify.modifier_count += 1;
                    sm.skillCooldownClone.transform.SetParent(amToModify.skillCooldownClone.transform);

                    sm.skillCooldownClone.transform.localPosition = new Vector3(0, (-0.3f - (0.5f * amToModify.modifier_count)) * iconWidth, 0);

                }

            }
            // Update all skillmodifier durations (this is done seperately so they all have the total duration)
            foreach (AttackModifier sm in SkillModifierList)
            {
                if ((sm.type == "double_duration") & (sm.is_enabled == true))
                {
                    sm.duration = amToModify.duration;
                    sm.startTimer(sm, null, this);
                    sm.is_enabled = false;
                }
            }
        }



        public void removeAttackModifier(AttackModifier am_to_remove)
        {
            GameObject skillCooldownSample = new GameObject();
            skillCooldownSample = GameObject.Find("SkillCooldown");


            am_to_remove.is_enabled = false;

            int am_to_remove_indx = AttackModifierList.IndexOf(am_to_remove);
            AttackModifierList.Remove(am_to_remove);
            int thisAMCount = AttackModifierList.Count;

            for (int j = am_to_remove_indx; j < thisAMCount; j++)
            {

                AttackModifier nextAM = AttackModifierList[j];
                if (nextAM.skillCooldownClone)
                {
                    updateGameObjectXPosition(nextAM.skillCooldownClone, iconWidth * -1);
                }
            }

            am_to_remove.skillCooldownClone.SetActive(false);
            Destroy(am_to_remove.skillCooldownClone);
            Destroy(am_to_remove);
        }

        private void updateGameObjectXPosition(GameObject gameObjectToTransform, float moveXAmmount)
        {
            gameObjectToTransform.transform.SetPositionAndRotation(new Vector3(
                gameObjectToTransform.transform.position.x + moveXAmmount,
                gameObjectToTransform.transform.position.y,
                gameObjectToTransform.transform.position.z),
                gameObjectToTransform.transform.rotation);
        }

        private int uniqueTerainModiferID = 0;
        public void AddTerainModifier(AttackModifier newTM, CardManager originCard)
        {

            uniqueTerainModiferID++;
            newTM.id = uniqueSkillModifierID;

        }


        public void addSkillModifier(AttackModifier newSM, CardManager originCard)
        {
            uniqueSkillModifierID++;
            newSM.id = uniqueSkillModifierID;
            GameObject skillCooldownSample = new GameObject();
            skillCooldownSample = GameObject.Find("AmplifyCooldown");
            int thisAMCount = SkillModifierList.Where(x => x.is_enabled).ToList().Count + 1;

            newSM.skillCooldownClone = Instantiate(skillCooldownSample, canvasGameObject.transform);
            //newSM.skillCooldownClone.gameObject.transform.SetPositionAndRotation(new Vector3(-53, -277, 0),
            //newSM.skillCooldownClone.gameObject.transform.rotation);
            newSM.skillCooldownClone.gameObject.transform.SetPositionAndRotation(
            new Vector3(
                newSM.skillCooldownClone.gameObject.transform.position.x + (iconWidth),
                newSM.skillCooldownClone.gameObject.transform.position.y + ((thisAMCount - 1) * iconWidth * -1 / 2) + 135,
                newSM.skillCooldownClone.gameObject.transform.position.z),
            newSM.skillCooldownClone.gameObject.transform.rotation);

            SkillModifierList.Add(newSM);
            //newSM.startTimer(this, newSM);
            originCard.DiscardCard();
        }

        public void removeSkillModifier(AttackModifier sm_to_remove)
        {
            if (sm_to_remove.skillCooldownClone)
            {
                GameObject skillCooldownSample = new GameObject();
                skillCooldownSample = GameObject.Find("AmplifyCooldown");

                sm_to_remove.is_enabled = false;

                int am_to_remove_indx = SkillModifierList.IndexOf(sm_to_remove);
                SkillModifierList.Remove(sm_to_remove);
                //int thisAMCount = SkillModifierList.Where(x => !x.is_enabled).ToList().Count;

                //for (int j = am_to_remove_indx; j < thisAMCount; j++)
                //{
                //    AttackModifier nextAM = SkillModifierList[j];
                //    updateGameObjectXPosition(nextAM.skillCooldownClone, iconWidth * -1);
                //}

                sm_to_remove.skillCooldownClone.SetActive(false);
                Destroy(sm_to_remove.skillCooldownClone);
                Destroy(sm_to_remove);
            }

        }


        Rigidbody exampleTerrain;
        private TerrainManager tm;
        public void PlaceTerrain()
        {
            update_count = 0;
            Vector3 spawnPoint = FindMousePointRelativeToPlayer();
            //spawnPoint.y = 0;
            Rigidbody terrainInstance = Instantiate(terrainTemplate, spawnPoint, terrainTemplate.rotation) as Rigidbody;
            tm = terrainInstance.gameObject.GetComponent<TerrainManager>();
            StartCoroutine("CheckForCollision");

        }

        public bool collided = false;

        private int update_count = 0;
        IEnumerator CheckForCollision()
        {
            for (; ; )
            {
                if (update_count == 0)
                {
                    update_count += 1;
                }
                else if ((update_count == 1) && (tm))
                {
                    if (tm.collided == true)
                    {
                        Destroy(tm.gameObject);
                        CancelTerrainPlacement();
                    }
                    else
                    {
                        terrainCard.DiscardCard();
                        CancelTerrainPlacement();
                    }
                    StopCoroutine("CheckForCollision");
                    update_count = 0;

                }

                yield return new WaitForFixedUpdate();
            }
        }

        private Vector3 FindMousePointOnFloor()
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

        Vector3 FindMousePointRelativeToPlayer()
        {
            Vector3 mousePointOnFloor = FindMousePointOnFloor();
            mousePointOnFloor = new Vector3(mousePointOnFloor.x,
                                            mousePointOnFloor.y,
                                            mousePointOnFloor.z);
            return mousePointOnFloor;
        }

    }



}
