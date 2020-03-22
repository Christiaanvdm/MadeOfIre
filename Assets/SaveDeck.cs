using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class SaveDeck : MonoBehaviour
    {

        private PlayerManager playerManager;
        private CombatManager combatManager;
        // Start is called before the first frame update
        void Start()
        {
            playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Save()
        {
            print("Save");
            playerManager.SavePlayerCards();
        }
        public void Load()
        {
            print("Load");
            playerManager.LoadPlayerCards();
        }

        public void Test()
        {
            print("Test");
        
        }
    }
}