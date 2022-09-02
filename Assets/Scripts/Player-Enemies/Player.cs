using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public Animator m_Animator;

    public GameObject scriptManager; //Used to get information from the map
    public GameObject pauseMenu;
    public GameObject Deathscreen;

    public AudioSource aSource;

    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip jump;
    public AudioClip walk;
    public AudioClip death;
    public AudioClip Hurt;

    public TextMeshProUGUI score;

    // public GameObject healthBar;

    public int healthy;
    public int maxHealth = 60;
    public int killCount;
    public int artCount;
    public HealthBar healthbar;

    private float moveSpeed = 5.0f;
    private float sprintSpeed = 15.0f;

    public float jumpForce = 270.0f;

    private RaycastHit hit;

    private bool isPaused;
    
    public bool facingRight;

    public bool isAttacking;

    public float attackRange;
    public float attackDamage;

    private float time;

    //Animation bools
    public bool isAttackingRight;
    public bool isAttackingLeft;
    
    public bool isWalkingRight;
    public bool isWalkingLeft;

    public bool isIdleRight;
    public bool isIdleLeft;

    public bool isJumpingRight;
    public bool isJumpingLeft;

    public bool isTurnLeft;
    public bool isTurnRight;
    public bool isDeath;

        

    void Start()
    {
        pauseMenu.SetActive(false);

        aSource = GetComponent<AudioSource>();
        aSource.volume = 1f;
        isPaused = false;
        facingRight = true;
        healthy = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }
    private void FixedUpdate()
    {        
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);

        time = time + Time.deltaTime;

        if (healthy == 0)
        {
            isDeath = true;
            m_Animator.SetBool("isDeath", true);
            aSource.clip = death;
            aSource.Play();
            moveSpeed = 0f;
            sprintSpeed = 0f;
            aSource.volume = 0f;
            Deathscreen.SetActive(true);
        }
        else
        {
             moveSpeed = 5.0f;
                sprintSpeed = 15.0f;
}
        score.SetText("KILL SCORE = " + (killCount * 3) + " \nART SCORE = " + (artCount * 50));
      //  healthBar.GetComponent<Text>().text = health.ToString();
    }

    void Update()
    {
        //If nothing is perssed, then is idle
        if (facingRight)
        {
            m_Animator.SetBool("isIdleRight", true);
            isIdleRight = true;
        }
        else
        {
            m_Animator.SetBool("isIdleLeft", true);
            isIdleLeft = true;
        }
        if (healthbar.slider.value == 0)
        {
            isDeath = true;
            m_Animator.SetBool("isDeath", true);
          //  aSource.clip = death;
          //  aSource.Play();
        }
        else
        {
            isDeath = false;
            m_Animator.SetBool("isDeath", false);
        }
        //bool flipped = moveSpeed < 0;
       // this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));

        //Get keys and move
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            //Jump (cannot jump more than every 0.2 seconds (no need for cooldown if on the ground)
            if(time > 0.4f || (Physics.Raycast(transform.position + new Vector3(0.499f, 0, 0), -transform.up, out hit, 0.6f, LayerMask.GetMask("Default"))))
                {

                time = 0; //Reset jump cooldown

                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);

                if (facingRight)
                {
                    m_Animator.SetBool("isJumpingRight", true);
                    isJumpingRight = true;
                }
                else
                {
                    m_Animator.SetBool("isJumpingLeft", true);
                    isJumpingLeft = true;
                }
                m_Animator.SetBool("isIdleLeft", false);
                m_Animator.SetBool("isIdleRight", false);
                isIdleLeft = false;
                isIdleRight = false;


                //aSource.clip = jump;
                aSource.PlayOneShot(jump);
            }
        }
        else
        {
            m_Animator.SetBool("isJumpingRight", false);
            m_Animator.SetBool("isJumpingLeft", false);
            isJumpingRight = false;
            isJumpingLeft = false;
        }

        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            //Prevent from jittering into wall
            if (!Physics.Raycast(transform.position + new Vector3(0, 0.45f, 0), transform.right, out hit, 0.501f, LayerMask.GetMask("Default")) &&
                !Physics.Raycast(transform.position - new Vector3(0, 0.45f, 0), transform.right, out hit, 0.501f, LayerMask.GetMask("Default")))
            {
                //Right move
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //Sprinting
                    gameObject.transform.Translate(Vector3.right * Time.deltaTime * sprintSpeed);
                }
                else
                {
                    gameObject.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
                }
                if (!facingRight)
                {
                    m_Animator.SetBool("isTurnRight", true);
                    isTurnRight = true;
                }
                else
                {
                    m_Animator.SetBool("isTurnRight", false);
                    isTurnRight = false;
                }

                //Touching the floor to make foot sound
                if (Physics.Raycast(transform.position + new Vector3(0.499f,0, 0), -transform.up, out hit, 0.6f, LayerMask.GetMask("Default")))
                {
                    if (!(aSource.isPlaying && aSource.clip == walk && isDeath == false))
                        {
                        aSource.clip = walk;
                        aSource.Play();
                    }
                }

                facingRight = true;

                m_Animator.SetBool("isWalkingRight", true);
                isWalkingRight = true;

                m_Animator.SetBool("isIdleLeft", false);
                m_Animator.SetBool("isIdleRight", false);
                isIdleLeft = false;
                isIdleRight = false;
            }
        }
        else
        {
            m_Animator.SetBool("isWalkingRight", false);
            isWalkingRight = false;
        }


        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            if (!Physics.Raycast(transform.position + new Vector3(0, 0.45f, 0), -transform.right, out hit, 0.501f, LayerMask.GetMask("Default")) &&
                !Physics.Raycast(transform.position - new Vector3(0, 0.45f, 0), -transform.right, out hit, 0.501f, LayerMask.GetMask("Default")))
            {

                //Left
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //Sprinting
                    gameObject.transform.Translate(Vector3.left * Time.deltaTime * sprintSpeed);
                }
                else
                {
                    gameObject.transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
                }

                if (facingRight)
                {
                    m_Animator.SetBool("isTurnLeft", true);
                    isTurnLeft = true;
                }
                else
                {
                    m_Animator.SetBool("isTurnLeft", false);
                    isTurnLeft = false;
                }

                if (Physics.Raycast(transform.position + new Vector3(0.499f, 0, 0), -transform.up, out hit, 0.6f, LayerMask.GetMask("Default")))
                {
                    if (!(aSource.isPlaying && aSource.clip == walk && isDeath == false))
                    {
                        aSource.clip = walk;
                        aSource.Play();
                    }
                }

                facingRight = false;

                m_Animator.SetBool("isWalkingLeft", true);
                isWalkingLeft = true;

                m_Animator.SetBool("isIdleLeft", false);
                m_Animator.SetBool("isIdleRight", false);
                isIdleLeft = false;
                isIdleRight = false;
            }
        }
        else
        {
            m_Animator.SetBool("isWalkingLeft", false);
            isWalkingLeft = false;
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) //Resuming game (is currently paused)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else //Pausing game
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }

        if (Input.GetMouseButtonDown(0) && time > 0.7f)
        {
            isAttacking = true;
            time = 0f;
            
            if (facingRight)
            {
                m_Animator.SetBool("isAttackingRight", true);
                isAttackingRight = true;
            }
            else
            {
                m_Animator.SetBool("isAttackingLeft", true);
                isAttackingLeft = true;
            }

            
            if(UnityEngine.Random.RandomRange(0, 100) > 50){       
                aSource.PlayOneShot(attack1);
            }
            else
            {
                aSource.PlayOneShot(attack2);
                    
            }            

            m_Animator.SetBool("isIdleLeft", false);
            m_Animator.SetBool("isIdleRight", false);
            isIdleLeft = false;
            isIdleRight = false;
        }
        else
        {
            isAttacking = false;

            m_Animator.SetBool("isAttackingRight", false);
            m_Animator.SetBool("isAttackingLeft", false);
            isAttackingRight = false;
            isAttackingLeft = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (other.gameObject.tag == "Goal")
        {
            scriptManager.GetComponent<GenerateCave>().NextLevel(); //Start the next level
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if(healthy > 0)
            {
                aSource.clip = Hurt;
                aSource.Play();
                healthy = healthy - 5;
                healthbar.SetHealth(healthy);
            }                        
        }
    }
}
