using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
    public class DeathScreen : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RestartScene()
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
