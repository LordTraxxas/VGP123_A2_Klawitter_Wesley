using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    public string playerTag = "Player"; 
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    public float projectileSpeed = 5f;
    public float detectionRange = 5f;

    public Transform spawnPoint;
    public GameObject[] collectable;

    private Transform playerTransform; 
    private float nextFireTime;
    public LayerMask projectileLayer;

    public float walkSpeed = 2f; 
    public float walkDistance = 5f; 
    public bool startWalkingRight;

    public int eHealth = 5;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Vector2 originalPosition;
    private Vector2 leftBoundary;
    private Vector2 rightBoundary;
    private bool walkingRight = true;

    private bool inRange = false;
    public bool tookDamage = false;
    public bool isTurret = false;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalPosition = rb.position;

        leftBoundary = originalPosition - new Vector2(walkDistance, 0f);
        rightBoundary = originalPosition + new Vector2(walkDistance, 0f);

        walkingRight = startWalkingRight;

    }

    void Update()
    {
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {

            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the tag specified in the enemy controller.");
        }
        if (walkingRight)
        {
            rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
            if (rb.position.x >= rightBoundary.x)
            {
                walkingRight = false;
                Flip();
            }
        }
        else
        {
            rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);
            if (rb.position.x <= leftBoundary.x)
            {
                walkingRight = true;
                Flip();
            }
        }
        if (playerTransform == null)
            return;

        if (Vector2.Distance(spawnPoint.position, playerTransform.position) <= detectionRange)
        {
            inRange = true;
            if (Time.time >= nextFireTime)
            {
                FireProjectile(playerTransform.position);

                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        else
        {
            inRange = false;
        }
        if (inRange == true && isTurret == true)
        {
            GetComponent<BoxCollider2D>().offset = new Vector2(0.2f, 0.2f);
            GetComponent<BoxCollider2D>().size = new Vector2(1.6f, 2.4f);
        }
        else if (inRange == false && isTurret == true)
        {
            GetComponent<BoxCollider2D>().offset = new Vector2(0.2f, 0);
            GetComponent<BoxCollider2D>().size = new Vector2(1.6f, 2);
        }

        anim.SetBool("inRange", inRange);
        anim.SetBool("tookDamage", tookDamage);
    }
    void FireProjectile(Vector3 targetPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        SetProjectileLayer(projectile);

        Vector3 targetPositionAbove = targetPosition + Vector3.up * 0.5f;

        Vector3 direction = (targetPositionAbove - spawnPoint.position).normalized;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        rb.velocity = direction * projectileSpeed;
    }

    void SetProjectileLayer(GameObject projectile)
    {
        
        int enemyLayer = gameObject.layer;

        projectile.layer = projectileLayer; 
    }

    void Flip()
    {
        
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void endDamAnim()
    {
        tookDamage = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            tookDamage = true;
            int randomSpawn = Random.Range(0, collectable.Length);
            if (eHealth > 1)
            {
                --eHealth;
            }
            else if (eHealth <= 1)
            {
                Destroy(gameObject);
                Instantiate(collectable[randomSpawn], transform.position, transform.rotation);
                PlayerController.PCI.score++;
            }
        }
    }
}