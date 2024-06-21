using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class PlayerController : MonoBehaviour
{

    public bool TestMode;
    
    public static PlayerController PCI {  get; private set; }

    public int lives;
    
    [SerializeField] private int maxLives = 5;
    [SerializeField] public int maxHealth = 5;
    [SerializeField] public Vector3 respawnPoint;
    [SerializeField] public int health;
    [SerializeField] public bool dead = false;

    [SerializeField] private int speed;
    [SerializeField] private int jumpForce = 3;
    public int score = 0;

    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask isGroundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private bool isAttacking;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip[] audioClips;

    private bool isPaused = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    private Coroutine jumpForceChange = null;
    private Coroutine speedChange = null;
    private Coroutine healthChange = null;

    public Shoot shootScript;

    public void PowerupValueChange(Pickup.PickupType type)
    {
        if (type == Pickup.PickupType.PowerupSpeed)
        {
            FillSpecificCoroutineVar(ref speedChange, ref speed, type);
        }
        if (type == Pickup.PickupType.PowerupJump)
        {
            FillSpecificCoroutineVar(ref jumpForceChange, ref jumpForce, type);
        }
        if (type == Pickup.PickupType.Health)
        {
            FillSpecificCoroutineVar(ref healthChange, ref health, type);
        }
    }

    void FillSpecificCoroutineVar(ref Coroutine inVar, ref int varToChange, Pickup.PickupType type)
    {
        if (inVar != null)
        {
            StopCoroutine(inVar);
            inVar = null;
            varToChange -= varToChange / 2;
            inVar = StartCoroutine(ValueChangeCoroutine(type));
            return;
        }

        inVar = StartCoroutine(ValueChangeCoroutine(type));
    }


    IEnumerator ValueChangeCoroutine(Pickup.PickupType type)
    {
        if (type == Pickup.PickupType.PowerupSpeed)
            speed += speed / 2;
        if (type == Pickup.PickupType.PowerupJump)
            jumpForce += jumpForce / 2;
        if (type == Pickup.PickupType.Health)
        {
            ++health;
            healthUi.Instance.healthBar(2);
        }
            

        yield return new WaitForSeconds(2.0f);

        if (type == Pickup.PickupType.PowerupSpeed)
            speed -= speed / 3;
        if (type == Pickup.PickupType.PowerupJump)
            jumpForce -= jumpForce / 3;
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(1);
        transform.position = respawnPoint;
        health = maxHealth;
        healthUi.Instance.resetBar();
        dead = false;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (health < maxHealth) 
        { 
            health = maxHealth; 
        }
        if (speed <= 0)
        {
            speed = 5;
            if (TestMode) Debug.Log("Forgot to set speed");
        }

        if (jumpForce <= 0)
        {
            jumpForce = 5;
            if (TestMode) Debug.Log("Forgot to set jumpForce");
        }

        if (groundCheckRadius <= 0)
        {
            groundCheckRadius = 0.02f;
            if (TestMode) Debug.Log("Ground CheckSet To Default Value");
        }

        if (groundCheck == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("GroundCheck");
            if (obj != null)
            {
                groundCheck = obj.transform;
                return;
            }
            GameObject newObj = new GameObject();
            newObj.transform.SetParent(transform);
            newObj.transform.localPosition = Vector3.zero;
            newObj.name = "GroundCheck";
            newObj.tag = newObj.name;
            groundCheck = newObj.transform;
            if (TestMode) Debug.Log("Ground Check Transform Created Via Code - did you forget to assign it in inspector?");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (maxLives <= 0)
        {
            maxLives = 5;
        }
        lives = maxLives;
        healthUi.Instance.UpdateLivesDisplay(maxLives);
    }

    void Update()
    {
        AnimatorClipInfo[] curPlayingClips = anim.GetCurrentAnimatorClipInfo(0);
        float xInput = Input.GetAxis("Horizontal");
        audioSource = GetComponent<AudioSource>();

        healthUi.Instance.UpdateScoreDisplay(score);
        healthUi.Instance.UpdateLivesDisplay(lives);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, isGroundLayer);
        isAttacking = false;

        if (Input.GetKeyDown(KeyCode.P)) {
            if (isPaused)
            {
                isPaused = false;
            }
            else
            {
                isPaused = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            --health;
            healthUi.Instance.healthBar(1);
        }

        if (curPlayingClips.Length > 0)
        {
            if (curPlayingClips[0].clip.name == "GroundAttack")

                rb.velocity = Vector2.zero;
            else
            {
                Vector2 moveDirection = new Vector2(xInput * speed, rb.velocity.y);
                rb.velocity = moveDirection;
            }
            if (curPlayingClips[0].clip.name == "megamanDeath")
            {
                rb.velocity = Vector2.zero;
            }
        }

        /*
        if (health <= 0 && lives >= 1 && dead == false)
        {
            dead = true;
            --lives;
            healthUi.Instance.UpdateLivesDisplay(lives);
            StartCoroutine(WaitTime());
        }
        else if (health <= 0 && lives <= 0 && dead == false)
        {
            dead = true;
            GameOver();
        }
        */

        if (Input.GetButtonDown("Jump") && isGrounded && !isPaused)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            PlaySpecificClip(2);
        }

        if (Input.GetButtonDown("Fire1") && !isPaused)
        {
            isAttacking = true;
            shootScript.Fire();
            PlaySpecificClip(0);
        }



        if (xInput != 0) sr.flipX = (xInput < 0);

        anim.SetFloat("speed", Mathf.Abs(xInput));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("attackIsPressed", isAttacking);
        anim.SetBool("isDead", dead);

        if (xInput < 0 && sr.flipX || xInput > 0 && !sr.flipX)
        {
            sr.flipX = sr.flipX;
        }
    }

    private void GameOver()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(2);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && health > 1)
        {
            --health;
            healthUi.Instance.healthBar(1);
        }
        else if (collision.gameObject.CompareTag("Enemy") && health <= 1 && lives >= 1 && dead == false)
        {
            dead = true;
            --lives;
            StartCoroutine(WaitTime());
        }
        else if (collision.gameObject.CompareTag("Enemy") && health <= 1 && lives == 0 && dead == false)
        {
            dead = true;
            GameOver();
        }
    }
    public void PlaySpecificClip(int index)
    {
        if (index >= 0 && index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Invalid audio clip index.");
        }
    }

    public void gainScore()
    {
        ++score;
    }

}
