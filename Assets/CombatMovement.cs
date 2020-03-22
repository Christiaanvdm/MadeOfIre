using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMovement : MonoBehaviour
{
    private Rigidbody rigidBody;
    private Animator anim;
    private GameObject character;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private const float diagonalAdjustmant = 0.7071067811865475244f;
    private float nextUpdate = 1f;

    public float moveSpeed = 5;
    private bool KeyA;
    private bool KeyD;
    private bool KeyS;
    private bool KeyW;

    private Vector3 previousLocation;

    private Vector3 localVelocity;
    // Start is called before the first frame update
    void Start()
    {
        character = GameObject.Find("Character");
        originalPosition = gameObject.transform.position;
        originalRotation = gameObject.transform.rotation;
        anim = gameObject.GetComponent<Animator>();
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
   

    private void FixedUpdate()
    {

    }

    private void Update()
    {
        checkKey();
        resetRotation();
    }


    void resetRotation()
    {
        //gameObject.GetComponent<Transform>().position = originalPosition;
        gameObject.GetComponent<Transform>().rotation = originalRotation;
    }

    void UpdateCharacterDirection()
    {
        character.transform.rotation = Quaternion.LookRotation(rigidBody.velocity);
    }

    void checkKey()
    {
        if (Input.GetKey(KeyCode.A))
        {
            KeyADown();

        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            KeyAUp();
        }
        if (Input.GetKey(KeyCode.D))
        {
            KeyDDown();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            KeyDUp();
        }
        if (Input.GetKey(KeyCode.W))
        {
            KeyWDown();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            KeyWUp();
        }
        if (Input.GetKey(KeyCode.S))
        {
            KeySDown();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            KeySUp();
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
        anim.SetInteger("State", 1);
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
       
        anim.SetInteger("State", 1);
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
        anim.SetInteger("State", 1);
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
        anim.SetInteger("State", 1);
    }
    void KeySUp()
    {
        KeyS = false;
        checkIfStopped();
    }

    void checkIfStopped()
    {
        if (!(KeyA | KeyD | KeyW | KeyS)) 
        {
            characterStopped();
        }
    }

    void characterStopped()
    {
        anim.SetInteger("State", 0);
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
        rigidBody.velocity = ((transform.right * - 1) + (transform.forward)).normalized * moveSpeed;
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
        rigidBody.velocity = ((transform.right) + (transform.forward * - 1)).normalized * moveSpeed;
        UpdateCharacterDirection();
    }


}
