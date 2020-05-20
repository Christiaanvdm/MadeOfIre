using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

public class EnemyZombieController : EnemyController
{

    private List<GameObject> axeBullets = new List<GameObject>();

    public override void onStart() {
        bulletVelocity = 9f;
        health = 200f;
        StartCoroutine(AttackEnemy());
        anim.SetBool("Moving", true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void EnableBullets()
    {
        foreach (var bullet in axeBullets)
            bullet.SetActive(true);
    }

    private void DisableBullets()
    {
        foreach (var bullet in axeBullets)
            bullet.SetActive(false);
    }

    virtual public void FireAtEnemy()
    {
        Vector3 shotDirection = (transform.position - player.transform.position).normalized;
        var projectileRigidbody = Resources.Load<Rigidbody>("EnemyProjectile");
        Rigidbody projectileInstance = Instantiate(projectileRigidbody, transform.position, transform.rotation) as Rigidbody;
        projectileInstance.velocity = -1 * shotDirection * bulletVelocity;
    }

    private void FixedUpdate()
    {
        if (!(finalDeath))
        {
            LookAtPlayer();
            MoveTowardsEnemy();
        }
        transform.rotation = initialRotation;
    }


    IEnumerator AttackEnemy()
    {
        while (!finalDeath)
        {
            if (EnemyLineOfSight())
            {
               FireAtEnemy();
            }
            var waitDuration = UnityEngine.Random.Range(1.5f, 2f);
            yield return new WaitForSeconds(waitDuration);
        }
    }

    private bool EnemyLineOfSight()
    {
        RaycastHit[] raycastHits;
        Vector3 rayDirection = player.transform.position - transform.position;
        raycastHits = Physics.RaycastAll(transform.position, rayDirection, Vector3.Distance(transform.position, player.transform.position));
        bool canSeeEnemy = true;
        foreach (RaycastHit nextHit in raycastHits)
        {
            if (nextHit.collider.gameObject.layer == 12)  // 12 = World
            {
                canSeeEnemy = false;
            }
        }
        return canSeeEnemy;
    }


    private void MoveTowardsEnemy()
    {
        KeepDistance();
        //navMeshAgent.SetDestination(player.transform.position);
    }

    private void KeepDistance() {
        var playerDirection = (transform.position - player.transform.position).normalized;

        navMeshAgent.SetDestination(player.position + playerDirection * distanceToKeep);
    }

    private float distanceToKeep = 4f;
    public override void Death()
    {
        transform.GetComponent<NavMeshAgent>().speed = 0f;
        colliderTransform.gameObject.SetActive(false);
        finalDeath = true;
        anim.SetInteger("State", 5);
        enemySprite.transform.position = new Vector3(transform.position.x, 0.405f, transform.position.z);
        //StartCoroutine(SlightlyAfterDeath());
    }

    IEnumerator SlightlyAfterDeath()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        yield return null;
    }

    void LookAtPlayer()
    {
        transform.LookAt(player.transform);
        updateOrientation((player.transform.position - transform.position).normalized);

    }

    private void updateOrientation(Vector3 direction)
    {
        // Look at mouse
        if ((direction.x > 0) && (direction.z > 0))
        {
            if (direction.x < direction.z)
            {
                lookUp();
            }
            else
            {
                lookRight();
            }
        }
        if ((direction.x < 0) && (direction.z > 0))
        {
            if (direction.x > direction.z * -1)
            {
                lookUp();
            }
            else
            {
                lookLeft();
            }
        }
        if ((direction.x < 0) && (direction.z < 0))
        {
            if (direction.x > direction.z)
            {
                lookDown();
                //print("Down");
            }
            else
            {
                lookLeft();
                //print("Left");
            }
        }
        if ((direction.x > 0) && (direction.z < 0))
        {
            if (direction.x < direction.z * -1)
            {
                lookDown();
                //print("Down");
            }
            else
            {
                lookRight();
                //print("Right");
            }
        }
    }
    private string current_orientation;
    private void lookUp()
    {

        if (current_orientation != "up")
        {
            anim.SetInteger("State", 1);

            current_orientation = "up";
        }
    }

    private void lookDown()
    {
        if (current_orientation != "down")
        {
            anim.SetInteger("State", 3);
            current_orientation = "down";
        }
    }

    private void lookRight()
    {
        if (current_orientation != "right")
        {
            anim.SetInteger("State", 2);
            current_orientation = "right";
        }
    }

    private void lookLeft()
    {
        if (current_orientation != "left")
        {
            anim.SetInteger("State", 4);
            current_orientation = "left";
        }
    }
}
