using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemies : MonoBehaviour
{
    private GameObject scriptManager;
    private GameObject player;
    private Rigidbody rb;

    public float health;
    public float maxHealth;
    public Slider healthslider;

    private AudioSource aSource;
    public AudioClip death;

    private float hitKnockback;
    private float knockBackTime;


    private bool dead;

    public int TypeID;

    private void Start()
    {
        health = maxHealth;
        healthslider.value = CalculateHealth();
        player = GameObject.FindGameObjectWithTag("Player");
        scriptManager = GameObject.FindGameObjectWithTag("ScriptManager");
        rb = gameObject.GetComponent<Rigidbody>();
        hitKnockback = 350f;
        aSource = GetComponent<AudioSource>();        
        dead = false;
    }

    private void FixedUpdate()
    {
        //Leep on the plane
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);

        knockBackTime = knockBackTime + Time.deltaTime;
        if (knockBackTime > 0.3f && knockBackTime < 0.4f)
        {
            rb.velocity = new Vector3(0, 0, 0); //Stay dazed for a brief moment
        }

        //Stop fall and die
        if(health < 0 && !dead) //Become dead and dont do again
        {
            dead = true;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            
            aSource.PlayOneShot(death);
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            rb.useGravity = true;
            player.GetComponent<Player>().killCount += 1;
            
            Destroy(gameObject, 3);
        }
    }

    private void Update()
    {
        healthslider.value = CalculateHealth();

        //Check if the player is nearby and approach them
        float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);
        if (dist < 15 && dist > 2)
        {
            gameObject.GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
        }

        //Check if player is attacking and is in range
        if (player.GetComponent<Player>().isAttacking &&
            Vector3.Distance(gameObject.transform.position, player.transform.position) < player.GetComponent<Player>().attackRange)
        {
            //Take Damage
            health = health - 31; //Dies in 3 hits  for now

            //is on the right of the player
            if (player.GetComponent<Player>().facingRight &&
                transform.position.x > player.transform.position.x)
            {
                //Enemy is slightly above player
                if (transform.position.y > player.transform.position.y + 0.4f)
                {
                    rb.AddForce(Vector3.up * hitKnockback);
                    rb.AddForce(Vector3.right * hitKnockback);
                    knockBackTime = 0;
                }
                else if (transform.position.y < player.transform.position.y - 0.4f)
                {
                    //Is slightly below player
                    rb.AddForce(Vector3.down * hitKnockback);
                    rb.AddForce(Vector3.right * hitKnockback);
                    knockBackTime = 0;
                }
                else
                {
                    //Is ahead of player
                    rb.AddForce(Vector3.right * hitKnockback);
                    knockBackTime = 0;
                }

            }

            //is on the left of the player
            if (!player.GetComponent<Player>().facingRight &&
                transform.position.x < player.transform.position.x)
            {
                //Enemy is slightly above player
                if (transform.position.y > player.transform.position.y + 0.4f)
                {
                    rb.AddForce(Vector3.up * hitKnockback);
                    rb.AddForce(Vector3.left * hitKnockback);
                    knockBackTime = 0;
                }
                else if (transform.position.y < player.transform.position.y - 0.4f)
                {
                    //Is slightly below player
                    rb.AddForce(Vector3.down * hitKnockback);
                    rb.AddForce(Vector3.left * hitKnockback);
                    knockBackTime = 0;
                }
                else
                {
                    //Is ahead of player
                    rb.AddForce(Vector3.left * hitKnockback);
                    knockBackTime = 0;
                }
            }


            //Sending information for NCA
            float intensity = Vector3.Distance(transform.position, player.transform.position); //Intensity is the value of distance between the player and enemy

            if (gameObject.GetComponent<EnemyType>().ID == 1)
            {                
                scriptManager.GetComponent<GenerateCave>().enemies[0].GetComponent<EnemyType>().intensities.Add(intensity);
            }
            else if (gameObject.GetComponent<EnemyType>().ID == 2)
                {
                scriptManager.GetComponent<GenerateCave>().enemies[1].GetComponent<EnemyType>().intensities.Add(intensity);
            }
            else if (gameObject.GetComponent<EnemyType>().ID == 3)
            {
                scriptManager.GetComponent<GenerateCave>().enemies[2].GetComponent<EnemyType>().intensities.Add(intensity);
            }
            else if (gameObject.GetComponent<EnemyType>().ID == 4)
            {
                scriptManager.GetComponent<GenerateCave>().enemies[3].GetComponent<EnemyType>().intensities.Add(intensity);
            }
            else if (gameObject.GetComponent<EnemyType>().ID == 5)
            {
                scriptManager.GetComponent<GenerateCave>().enemies[4].GetComponent<EnemyType>().intensities.Add(intensity);
            }
        }

    }
    float CalculateHealth()
    {
        return health / maxHealth;
    }

    IEnumerator StopForce()
    {
        yield return new WaitForSeconds(1);
        rb.velocity = new Vector3(0, 0, 0);
    }
}
