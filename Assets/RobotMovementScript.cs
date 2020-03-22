using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete {

    public class RobotMovementScript : MonoBehaviour

    {
        private Rigidbody rigidBody;
        private Animator anim;
        private Transform originalTransform;
        // Start is called before the first frame update
        void Start()
        {
            originalTransform = gameObject.transform;
            anim = gameObject.GetComponent<Animator>();
            rigidBody = gameObject.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            checkKey();
        }

        void checkKey()
        {
            if (Input.GetKey(KeyCode.A))
            {
                moveLeft();
                anim.SetBool("Walk_Anim", true);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                anim.SetBool("Walk_Anim", false);
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveRight();
                anim.SetBool("Walk_Anim", true);
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                anim.SetBool("Walk_Anim", false);
            }
            if (Input.GetKey(KeyCode.W))
            {
                moveForward();
                anim.SetBool("Walk_Anim", true);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                anim.SetBool("Walk_Anim", false);
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveBackward();
                anim.SetBool("Walk_Anim", true);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                anim.SetBool("Walk_Anim", false);
            }


        }

        private float turnSpeed = 1f;
        private float moveSpeed = 2f;
        private float reverseSpeed = 1f;
        private void turnLeft()
        {

            transform.Rotate(Vector3.Lerp(transform.rotation.eulerAngles, transform.eulerAngles + new Vector3(0, -90, 0), Time.deltaTime));

        }

        private void turnRight()
        {

            transform.Rotate(Vector3.Lerp(transform.rotation.eulerAngles, transform.eulerAngles + new Vector3(0, 90, 0), Time.deltaTime));
        }

        private void moveForward()
        {
            transform.forward = originalTransform.forward;
            transform.position = Vector3.Lerp(transform.position, transform.position + (transform.forward * moveSpeed), Time.deltaTime);
        }

        private void moveBackward()
        {
            transform.forward = originalTransform.forward * -1;
            transform.position = Vector3.Lerp(transform.position, transform.position + (transform.forward * -1 * moveSpeed), Time.deltaTime);
        }

        private void moveLeft()
        {
            transform.forward = originalTransform.right * -1;
            transform.position = Vector3.Lerp(transform.position, transform.position + (transform.right * -1 * moveSpeed), Time.deltaTime);

        }

        private void moveRight()
        {
            transform.eulerAngles = originalTransform.right;
            transform.position = Vector3.Lerp(transform.position, transform.position + (transform.right * moveSpeed), Time.deltaTime);

        }
    }
}