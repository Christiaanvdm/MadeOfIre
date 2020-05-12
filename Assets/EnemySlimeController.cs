using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public class EnemySlimeController : MonoBehaviour, IEnemyController
{
    private Transform colliderTransform;
    private Animator anim;
    private GameObject enemySprite;
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private float bulletVelocity = 7f;
    private int numberOfBullets = 6;
    private Transform circle;
    private List<GameObject> axeBullets = new List<GameObject>();
    private Vector3 circleOriginalScale;
    private float circleScale = 0;
    private SpriteRenderer renderer;
    private float explosionRadius = 2.4f;
    private int explosionDamage = 1;
    private bool finalDeath = false;
    private float health = 30f;
    private bool explosionStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        circle = transform.Find("CircleParent").Find("Circle");

        circleOriginalScale = circle.localScale;

        circle.localScale = circleOriginalScale * circleScale;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        anim = transform.Find("Anim").GetComponent<Animator>();
        renderer = transform.Find("Anim").GetComponent<SpriteRenderer>();
        colliderTransform = transform.Find("Collider");
        initialRotation = transform.rotation;

        //SpawnAndFireBullets();

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (!(finalDeath))
        {
            MoveTowardsEnemy();
            if (Vector3.Distance(player.position, parentTransform.position) < explosionRadius * 2)
            {

                StartCoroutine(SpinCircle());

            }

        }
        transform.rotation = initialRotation;
    }



    IEnumerator AttackEnemy()
    {
        while (!finalDeath)
        {

            yield return new WaitForSeconds(3f);
            StartCoroutine(SpinCircle());
            yield return new WaitForSeconds(3f);

        }
    }

    IEnumerator GrowCircle()
    {
        while (!finalDeath)
        {
            yield return new WaitForSeconds(0.1f);

        }
        yield return null;
    }

    IEnumerator SpinCircle()
    {
        while (circleScale < 1)
        {
            circleScale += 0.02f;
            circle.localScale = circleOriginalScale * circleScale;
            var newColor = new Color(0.8f, (0.8f - circleScale), (0.8f - circleScale));
            renderer.color = newColor;
            circle.GetComponent<SpriteRenderer>().color = newColor;
            circle.transform.RotateAroundLocal(Vector3.forward, 15 * (Mathf.PI / 180));
            yield return new WaitForSeconds(0.3f);


        }
        if (circleScale > 1 && circleScale < 1.5)
        {

            StartCoroutine(Explode());
            circleScale = 2;
        }
    }

    IEnumerator Explode()
    {
        var simpleBool = false;
        while (!simpleBool)
        {
            explosionStarted = true;
            anim.SetBool("Explode", true);

            yield return new WaitForSeconds(0.75f);
            if (Vector3.Distance(player.position, parentTransform.position) < explosionRadius)
            {
                var playerManager = player.GetComponent<PlayerManager>();
                playerManager.Damage(1, "blunt");
            }
            simpleBool = true;
            anim.gameObject.SetActive(false);
            circle.gameObject.SetActive(false);
            Death();
        }

        yield return null;
    }


    private Quaternion initialRotation;

    private void MoveTowardsEnemy()
    {

        navMeshAgent.SetDestination(player.transform.position);
    }

    protected void Death()
    {
        finalDeath = true;
        //anim.SetInteger("State", 5);
        //enemySprite.layer = 16;
        StartCoroutine(SlightlyAfterDeath());
    }

    IEnumerator SlightlyAfterDeath()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        yield return null;
    }

    public void HitByProjectile(AttackProjectile projectile)
    {
        if (!explosionStarted)
            health -= projectile.damage;
        if (health < 0) {
            anim.SetBool("Explode", true);
            Death();
        }
    }

    public Transform parentTransform => transform;

    public void addDebuff(AttackModifier attackModifier)
    {
        throw new NotImplementedException();
    }
}
