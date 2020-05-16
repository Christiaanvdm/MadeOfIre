using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

public class EnemyAxeController : MonoBehaviour, IEnemyController
{
    private Transform colliderTransform;
    private Animator anim;
    private Transform enemySprite;
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private float bulletVelocity = 9f;
    private int numberOfBullets = 30;
    private float health = 20f;
    private Quaternion initialRotation;
    private bool finalDeath = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        enemySprite = transform.Find("Anim");
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        anim = transform.Find("Anim").GetComponent<Animator>();
        colliderTransform = transform.Find("Collider");
        initialRotation = transform.rotation;
        //SpawnAndFireBullets();
        StartCoroutine(AttackEnemy());
    }

    // Update is called once per frame
    void Update()
    {

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

    private void LookAtPlayer()
    {

    }
    IEnumerator AttackEnemy()
    {
        while (!finalDeath)
        {
            var waitDuration = UnityEngine.Random.Range(4f, 6f);
            yield return new WaitForSeconds(waitDuration);
            StartCoroutine(SwingAxe());
        }
    }

    IEnumerator SwingAxe()
    {
        bool swung = false;
        while (!swung && !finalDeath)
        {
            anim.SetInteger("Attack", 1);
            yield return new WaitForSeconds(0.5f);
            anim.SetInteger("Attack", 0);
            SpawnAndFireBulletsCircle();
            yield return new WaitForSeconds(1f);
            swung = true;
        }
        yield return null;
    }

    private void SpawnAndFireBulletsCircle()
    {
        var fireTrajectory = (colliderTransform.position - player.position).normalized;
        int intervals = numberOfBullets;
        float radius = 2;
        Vector3 currentPos = new Vector3(colliderTransform.position.x, 1.2f, colliderTransform.position.z);
        var projectilePrefab = Resources.Load<GameObject>("EnemyProjectile");
        float degrees = (360f / intervals) * (Mathf.PI / 180);
        float currentDegree = -45 * (Mathf.PI / 180);
        for (int i = 0; i < intervals; i++)
        {
            Vector3 offset = new Vector3(Mathf.Cos(currentDegree) * radius, 0, Mathf.Sin(currentDegree) * radius);
            var nextBullet = Instantiate(projectilePrefab, currentPos + offset, transform.rotation);
            fireTrajectory = offset.normalized;
            currentDegree += degrees;
            nextBullet.GetComponent<Rigidbody>().velocity += fireTrajectory * bulletVelocity;
        }
    }

    private void MoveTowardsEnemy()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    protected void Death()
    {
        transform.GetComponent<NavMeshAgent>().speed = 0f;
        finalDeath = true;
        anim.SetBool("Dead", true);
        colliderTransform.gameObject.SetActive(false);
        enemySprite.transform.position = new Vector3(transform.position.x, 0.405f, transform.position.z);
    }

    public void HitByProjectile(AttackProjectile projectile)
    {
        health -= projectile.damage;
        if (health < 0)
        {
            Death();
        }
    }

    public Transform parentTransform => transform;

    public void addDebuff(AttackModifier attackModifier)
    {
        throw new NotImplementedException();
    }
}
