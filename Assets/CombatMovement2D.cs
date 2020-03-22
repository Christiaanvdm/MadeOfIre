using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
    public class CombatMovement2D : MonoBehaviour
    {
        private Rigidbody rigidBody;
        private Animator anim;
        private GameObject character;
        private PlayerManager playerManager;

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private const float diagonalAdjustmant = 0.7071067811865475244f;
        private float nextUpdate = 1f;

        public float moveSpeed = 5;
        private bool KeyA;
        private bool KeyD;
        private bool KeyS;
        private bool KeyW;
        private Animator animator;

        private Vector3 previousLocation;

        private Vector3 localVelocity;
        // Start is called before the first frame update
        void Start()
        {
            playerManager = gameObject.GetComponent<PlayerManager>();
            animator = gameObject.GetComponent<Animator>();
            character = GameObject.Find("Character");
            originalPosition = gameObject.transform.position;
            originalRotation = gameObject.transform.rotation;
            anim = character.gameObject.GetComponent<Animator>();
            rigidBody = gameObject.GetComponent<Rigidbody>();

        }

        // Update is called once per frame


        private void FixedUpdate()
        {
            if (!playerManager.ControlsDisabled)
            {
                checkKey();
                checkIfStopped();
            }

        }

        private void Update()
        {

        }


        void UpdateCharacterDirection()
        {
            //character.transform.rotation = Quaternion.LookRotation(rigidBody.velocity);
        }



        void checkKey()
        {
            float x = Input.GetAxis("Horizontal");

            float y = Input.GetAxis("Vertical");
            if ((x * x > 0) || (y * y > 0)) {
                anim.SetBool("Moving", true);
            }
            else
            {
                anim.SetBool("Moving", false);
            }
            if ((x == 0) && (y == 0))
            {
                characterStopped();
            }
            if ((x == 0) && (y > 0))
            {
                moveUp();
            }
            if ((x == 0) && (y < 0))
            {
                moveDown();
            }
            if ((x > 0) && (y == 0))
            {
                moveRight();
            }
            if ((x < 0) && (y ==0))
            {
                moveLeft();
            }
            if ((x < 0) && (y > 0))
            {
                moveUpLeft();
            }
            if ((x > 0) && (y > 0))
            {
                moveUpRight();
            }
            if ((x < 0) && (y < 0))
            {
                moveDownLeft();
            }
            if ((x > 0) && (y < 0))
            {
                moveDownRight();
            }
           

        }

        void KeyADown()
        {
            if (KeyW)
            {
                moveUpLeft();
            }
            else if (KeyS)
            {
                moveDownLeft();
            }
            else
            {
                moveLeft();
            }
            KeyA = true;
            anim.SetBool("Moving", true);
        }

        void KeyAUp()
        {
            KeyA = false;
            checkIfStopped();
        }

        void KeyWDown()
        {
            if (KeyA)
            {
                moveUpLeft();
            }
            else if (KeyD)
            {
                moveUpRight();
            }
            else
            {
                moveUp();
            }
            KeyW = true;

            anim.SetBool("Moving", true);
        }

        void KeyWUp()
        {
            KeyW = false;
            checkIfStopped();
        }

        void KeyDDown()
        {
            if (KeyW)
            {
                moveUpRight();
            }
            else if (KeyS)
            {
                moveDownRight();
            }
            else
            {
                moveRight();
            }
            KeyD = true;
            anim.SetBool("Moving", true);
        }

        void KeyDUp()
        {
            KeyD = false;
            checkIfStopped();
        }

        void KeySDown()
        {
            if (KeyA)
            {
                moveDownLeft();
            }
            else if (KeyD)
            {
                moveDownRight();
            }
            else
            {
                moveDown();
            }
            KeyS = true;
            anim.SetBool("Moving", true);
        }
        void KeySUp()
        {
            KeyS = false;
            checkIfStopped();
        }

        void checkIfStopped()
        {
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                characterStopped();
            }
        }

        public void characterStopped()
        {
            KeyA = false;
            KeyD = false;
            KeyW = false;
            KeyS = false;
            anim.SetBool("Moving", false);
            rigidBody.velocity = new Vector3(0, 0, 0);
        }


        private void moveUp()
        {

            rigidBody.velocity = transform.forward * moveSpeed;
            UpdateCharacterDirection();
        }

        private void moveDown()
        {
            rigidBody.velocity = transform.forward * moveSpeed * -1;
            UpdateCharacterDirection();
        }

        private void moveLeft()
        {
            rigidBody.velocity = transform.right * moveSpeed * -1;
            UpdateCharacterDirection();
        }

        private void moveRight()
        {
            rigidBody.velocity = transform.right * moveSpeed;
            UpdateCharacterDirection();
        }

        private void moveUpLeft()
        {
            rigidBody.velocity = ((transform.right * -1) + (transform.forward)).normalized * moveSpeed;
            UpdateCharacterDirection();
        }

        private void moveUpRight()
        {
            rigidBody.velocity = ((transform.right) + (transform.forward)).normalized * moveSpeed;
            UpdateCharacterDirection();
        }

        private void moveDownLeft()
        {
            rigidBody.velocity = ((transform.right * -1) + (transform.forward * -1)).normalized * moveSpeed;
            UpdateCharacterDirection();
        }

        private void moveDownRight()
        {
            rigidBody.velocity = ((transform.right) + (transform.forward * -1)).normalized * moveSpeed;
            UpdateCharacterDirection();
        }

        public void SetAllKeysAsUp()
        {
            KeyA = false;
            KeyD = false;
            KeyW = false;
            KeyS = false;
        }
    }

}