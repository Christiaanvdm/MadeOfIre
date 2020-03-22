using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
    public class DoorManager : MonoBehaviour
    {
        private Animator animatorFront;
        private Animator animatorTop;
        private bool isOpen;
        private int doorState = 0; //0 = Never opened, 1 = Opened, 2 = CLosed, 3 = Opened...
                                   // Start is called before the first frame update
        void Start()
        {
            animatorFront = transform.Find("SpriteFront").gameObject.GetComponent<Animator>();
            animatorTop = transform.Find("SpriteTop").gameObject.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((collision.gameObject.tag == "player") && !isOpen && (doorState == 0))
            {
                OpenDoor();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            //if ((collision.gameObject.tag == "player") && isOpen)
            //{
            //    CloseDoor();
            //}
        }

        public void OpenDoor()
        {
            doorState += 1;
            gameObject.layer = 16; // No collide
            animatorFront.SetBool("Open", true);

            animatorTop.SetBool("Open", true);
            isOpen = true;
        }

        public void CloseDoor()
        {
            doorState += 1;
            gameObject.layer = 12; //World
            animatorFront.SetBool("Open", false);
            animatorTop.SetBool("Open", false);
            isOpen = false;
        }
    }
}