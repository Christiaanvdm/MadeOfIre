using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkAnimation : MonoBehaviour
{
    private float TimeOfDeath;
    private float Duration = 2f;
    private GameObject player;
    Animator thisAnimator;
    // Start is called before the first frame update
    void Start()
    {
        thisAnimator = gameObject.GetComponent<Animator>();
        player = GameObject.Find("Player");
        gameObject.SetActive(true);
        TimeOfDeath = Time.time + Duration;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.transform.position - new Vector3(0, 0, 1);
        if (Time.time > TimeOfDeath)
        {
            Death();
        }
    }

    void Death()
    {
        //gameObject.SetActive(false);

    }

    void StartAnimation()
    {
        thisAnimator.Play("Arive");
    }
}
