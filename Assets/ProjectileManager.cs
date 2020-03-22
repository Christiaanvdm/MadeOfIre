using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
    public class ProjectileManager : MonoBehaviour
    {
        public float amount;
        public string type;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "player")
            {
                PlayerManager playerManager = other.gameObject.GetComponent<PlayerManager>();
                playerManager.Damage(amount, "blunt");
            }
        }


    }
}