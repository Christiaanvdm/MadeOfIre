using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using System.Threading.Tasks;

namespace Complete
{

    public class CombatManager : MonoBehaviour
    {
        //private float nextUpdate = 1; //In seconds
        //private float nextUpdate5 = 1; //In seconds
        //private float updateSize = 0.1f;

        AttackProjectile playerAttack;

        private float nextUpdate = 1f;
        private float duration = 0.5f;
        private bool inMenu = false;
        private float iconWidth = 64;

        public bool isDraggingACard = false;
        private int uniqueSpellID = 0;
        private int uniqueSkillModifierID = 0;
        private GameObject player;
        private PlayerManager playerManager;
        public List<ModifierObject> AttackModifierList;
        public List<ModifierObject> SkillModifierList;
        public List<ModifierObject> TerrainModifierList;
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

        //private SkillDetail playerAttack = new SkillDetail();

        public string currentRoom = "StartRoom";
        // Start is called before the first frame update
        void Start()
        {

            if (!PlayerPrefs.HasKey("CurrentRoom"))
                PlayerPrefs.SetString("CurrentRoom", "StartRoom");
            deathScreen = Resources.FindObjectsOfTypeAll<DeathScreen>()[0];
            deathScreen.gameObject.SetActive(false);
            DeckManagerGO = Resources.FindObjectsOfTypeAll<DeckManager>()[0].gameObject;
            DeckManagerGO.SetActive(false);
            isPlacingTerain = false;
            canvasGameObject = GameObject.Find("HUDUICanvas");
            AttackModifierList = new List<ModifierObject>();
            SkillModifierList = new List<ModifierObject>();
            player = GameObject.Find("Player");
            playerManager = player.gameObject.GetComponent<PlayerManager>();
            terrainPlacement = GameObject.Find("TerrainPlacement");
            terrainTemplate = GameObject.Find("TerrainTemplate").GetComponent<Rigidbody>();
            healthIcons = new List<GameObject>();
            healthSample = GameObject.Find("Health");
            deckManager = DeckManagerGO.GetComponent<DeckManager>();
            SetupPlayerAttack();
            UpdateHUDHealth(6);
            SpawnRoom();
        }

        void SetupPlayerAttack() {
            playerAttack = Resources.Load<AttackProjectile>("AttackProjectile");
            playerAttack.speed = 10f;
            playerAttack.scale = 1f;
            playerAttack.cooldown = 0.5f;
        }

        public void SpawnRoom()
        {
            var currentRoom = PlayerPrefs.GetString("CurrentRoom");
            var room = Resources.Load<GameObject>($"Rooms/{currentRoom}");

            GameObject newRoom = Instantiate(room) as GameObject;
            var entrance = transform.Find("Exit");

            newRoom.transform.position = GameObject.Find("RoomOrigin").transform.position;
        }

        // Update is called once a frame
        void Update()
        {

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
            foreach (ModifierObject nextAM in AttackModifierList)
            {
                if (projectile.GetType() == typeof(AttackProjectile))
                {
                    if (nextAM.info.type == "double_size")
                    {
                        projectile.rigidBody.gameObject.transform.localScale = projectile.rigidBody.gameObject.transform.localScale * nextAM.info.magnitude;
                    }
                    if (nextAM.info.type == "double_damage")
                    {
                        //ToDo Make fireball!
                        UpdateProjectileColor(projectile, new Color(1, 0, 0));
                    }
                    if (nextAM.info.type == "half_speed")
                    {
                        ApplySlowToProjectile(ref projectile, nextAM);
                    }
                    if (nextAM.info.type == "split_shot")
                    {
                        ApplySplitToProjectile(ref projectile, nextAM);
                    }
                    if (nextAM.info.type == "chain_shot")
                    {
                        ApplyChainToProjectile(ref projectile, nextAM);
                    }
                    if (nextAM.info.type == "bounce") {
                        AddBounceToProject(ref projectile, nextAM);
                    }
                }
            }

        }

        private void ApplyChainToProjectile(ref AttackProjectile projectile, ModifierObject nextAM)
        {
            nextAM.info.context = "Hit";
            projectile.AddModifier(nextAM);
        }


        private void AddBounceToProject(ref AttackProjectile projectile, ModifierObject nextAM) {
            projectile.AddBounces(Mathf.RoundToInt(nextAM.info.magnitude));
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

        private void ApplySplitToProjectile(ref AttackProjectile projectile, ModifierObject nextAM)
        {
            nextAM.info.context = "Birth";
            projectile.AddModifier(nextAM);
        }

        private void ApplySlowToProjectile(ref AttackProjectile projectile, ModifierObject nextAM)
        {
            nextAM.info.context = "Enemy";
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



        public void SpaceDown()
        {

        }

        public void CDown() {
            playerManager.AdvanceConversation();
        }

        public void CUp() { }

        public void ReturnDown() {

        }

        public void ReturnUp() {

        }

        public void F1Down()
        {

        }

        public void F1Up()
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

        public void EscDown()
        {
        }

        public void SpaceUp()
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

        public void EnterMenu(SkillDetail newCard)
        {
            PauseGame();
            DeckManagerGO.SetActive(true);
            inMenu = true;
            deckManager.AddCardMenu(newCard);
        }

        public void Death()
        {
            //PauseGame();
            //inMenu = true;
            deathScreen.gameObject.SetActive(true);
        }

        public void EscUp()
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

        public void Key1Down()
        {

            CancelTerrainPlacement();
            GameObject card1 = GameObject.Find("Card1");
            CardManager cardManager = card1.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.SetOffSet();
            cardManager.isDraggingThisCard = true;

        }

        public void Key1Up()
        {

            GameObject card1 = GameObject.Find("Card1");
            CardManager cardManager = card1.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            executeCard(cardManager);
        }

        private void executeCard(CardManager cardManager) {
            if (cardManager.isActive)
            {
                if (!cardManager.skillManager)
                {
                    cardManager.skillManager = cardManager.gameObject.GetComponent<SkillManager>();
                }
                cardManager.skillManager.ExecuteSkill(this);
                cardManager.SnapToAttention();
                cardManager.DiscardCard();
            }
        }

        public void Key2Down()
        {

            CancelTerrainPlacement();
            GameObject card2 = GameObject.Find("Card2");
            CardManager cardManager = card2.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.isDraggingThisCard = true;

        }

        public void Key2Up()
        {
            GameObject card2 = GameObject.Find("Card2");
            CardManager cardManager = card2.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            executeCard(cardManager);

        }

        public void Key3Down()
        {

            CancelTerrainPlacement();
            GameObject card3 = GameObject.Find("Card3");
            CardManager cardManager = card3.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.isDraggingThisCard = true;

        }

        public void Key3Up()
        {

            GameObject card3 = GameObject.Find("Card3");
            CardManager cardManager = card3.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            executeCard(cardManager);

        }

        public void Key4Down()
        {


            CancelTerrainPlacement();
            GameObject card4 = GameObject.Find("Card4");
            CardManager cardManager = card4.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.isDraggingThisCard = true;

        }

        public void Key4Up()
        {

            GameObject card4 = GameObject.Find("Card4");
            CardManager cardManager = card4.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            executeCard(cardManager);

        }

        public void Mouse1Down()
        {

            GameObject cardB = GameObject.Find("CardB");
            CardManager cardManager = cardB.GetComponent<CardManager>();
            //isDraggingACard = true;
            cardManager.isDraggingThisCard = true;
            if (isPlacingTerain)
            {
                PlaceTerrain();
                return;
            }

        }

        public void Mouse1Up()
        {

            GameObject cardB = GameObject.Find("CardB");
            CardManager cardManager = cardB.GetComponent<CardManager>();
            cardManager.isDraggingThisCard = false;
            isDraggingACard = false;
            Attack();
        }

        public void Mouse2Down()
        {
            GameObject cardDR = GameObject.Find("CardDR");
            CardManager cardManager = cardDR.GetComponent<CardManager>();
            isDraggingACard = true;
            cardManager.isDraggingThisCard = true;
            CancelTerrainPlacement();
        }

        public void Mouse2Up()
        {
            GameObject cardDR = GameObject.Find("CardDR");
            CardManager cardManager = cardDR.GetComponent<CardManager>();
            executeCard(cardManager);
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

        public void addAttackModifier(ModifierObject newAM)
        {
            uniqueSpellID++;

            newAM.info.id = uniqueSpellID;

            GameObject skillCooldownSample = new GameObject();
            skillCooldownSample = Resources.Load<GameObject>("skillCooldown");
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

            var skillSprite = Resources.Load<Sprite>(newAM.info.icon_name);
            spriteIcon.sprite = skillSprite;

            AttackModifierList.Add(newAM);
            startAMCooldown(newAM);


        }

        public void startAMCooldown(ModifierObject attackModifier)
        {
            if (attackModifier.start)
                throw new Exception("Start Timer called twice on one attack modifier");

            Image skillCooldown = attackModifier.skillCooldownClone.GetComponent<Image>();
            cooldownIcon(attackModifier, skillCooldown);
        }

        private async void cooldownIcon(ModifierObject attackModifier, Image skillCooldownImage) {
            var duration = (attackModifier.info.duration);
            while (skillCooldownImage.fillAmount > 0) {
                await Task.Delay(Mathf.RoundToInt(Time.fixedDeltaTime * 1000));
                skillCooldownImage.fillAmount -= (1 / duration) * Time.fixedDeltaTime;
            }

            removeAttackModifier(attackModifier);
        }

        public void ModifyAttackModifier(ModifierObject amToModify)
        {
            // Apply the amplification
            foreach (ModifierObject sm in SkillModifierList)
            {
                if ((sm.info.type == "double_duration") & (sm.info.enabled == true))
                {
                    amToModify.info.duration = amToModify.info.duration * sm.info.magnitude;
                    amToModify.info.modifier_count += 1;
                    sm.skillCooldownClone.transform.SetParent(amToModify.skillCooldownClone.transform);

                    sm.skillCooldownClone.transform.localPosition = new Vector3(0, (-0.3f - (0.5f * amToModify.info.modifier_count)) * iconWidth, 0);
                }

            }
            // Update all skillmodifier durations (this is done seperately so they all have the total duration)
            foreach (ModifierObject sm in SkillModifierList)
            {
                if ((sm.info.type == "double_duration") & (sm.info.enabled == true))
                {
                    sm.info.duration = amToModify.info.duration;
                    this.startAMCooldown(sm);
                    sm.info.enabled = false;
                }
            }
        }

        public void removeAttackModifier(ModifierObject am_to_remove)
        {
            GameObject skillCooldownSample = new GameObject();
            skillCooldownSample = Resources.Load<GameObject>("SkillCooldown");


            am_to_remove.info.enabled = false;

            int am_to_remove_indx = AttackModifierList.IndexOf(am_to_remove);

            int thisAMCount = AttackModifierList.Count;

            for (int j = am_to_remove_indx; j < thisAMCount - 1; j++)
            {
                ModifierObject nextAM = AttackModifierList[j];
                if (nextAM.skillCooldownClone)
                {
                    updateGameObjectXPosition(nextAM.skillCooldownClone, iconWidth * -1);
                }
            }
            AttackModifierList.Remove(am_to_remove);
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
        public void AddTerainModifier(ModifierObject newTM, CardManager originCard)
        {
            uniqueTerainModiferID++;
            newTM.info.id = uniqueSkillModifierID;
        }


        public void addSkillModifier(ModifierObject newSM)
        {
            uniqueSkillModifierID++;
            newSM.info.id = uniqueSkillModifierID;
            GameObject skillCooldownSample = new GameObject();
            skillCooldownSample = GameObject.Find("AmplifyCooldown");
            int thisAMCount = SkillModifierList.Where(x => x.info.enabled).ToList().Count + 1;

            newSM.skillCooldownClone = Instantiate(skillCooldownSample, canvasGameObject.transform);
            newSM.skillCooldownClone.gameObject.transform.SetPositionAndRotation(
            new Vector3(
                newSM.skillCooldownClone.gameObject.transform.position.x + (iconWidth),
                newSM.skillCooldownClone.gameObject.transform.position.y + ((thisAMCount - 1) * iconWidth * -1 / 2) + 135,
                newSM.skillCooldownClone.gameObject.transform.position.z),
            newSM.skillCooldownClone.gameObject.transform.rotation);

            SkillModifierList.Add(newSM);
        }

        public void removeSkillModifier(ModifierObject sm_to_remove)
        {
            if (sm_to_remove.skillCooldownClone)
            {
                GameObject skillCooldownSample = new GameObject();
                skillCooldownSample = GameObject.Find("AmplifyCooldown");

                sm_to_remove.info.enabled = false;

                int am_to_remove_indx = SkillModifierList.IndexOf(sm_to_remove);
                SkillModifierList.Remove(sm_to_remove);

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

        private bool isAttacking = false;
        public async void Attack()
        {
            if (!isAttacking)
            {
                isAttacking = true;
                await asyncAttack();
                isAttacking = false;
            }

        }

        public async Task asyncAttack() {
            Vector3 shotDirection = (FindMousePointRelativeToPlayer() - player.transform.position).normalized;
            var projectileRigidbody = Resources.Load<Rigidbody>("AttackProjectile");

            Rigidbody projectileInstance = Instantiate(projectileRigidbody, player.transform.position + new Vector3(0, 0.1f, -0.2f), player.transform.rotation) as Rigidbody;
            projectileInstance.velocity = shotDirection * playerAttack.speed;
            //projectileInstance.transform.up = new Vector3(0, 1, 0);
            AttackProjectile nextAP = projectileInstance.GetComponent<AttackProjectile>();
            nextAP.speed = playerAttack.speed;
            ModifyProjectile(ref nextAP);
            nextAP.StartUp();
            await Task.Delay(playerAttack.cooldown.ToMilliSeconds());
        }

    }
}
