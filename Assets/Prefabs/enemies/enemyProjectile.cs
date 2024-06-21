using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour
{
    public float speed = 10f; 
    public int damage = 1;
    public float lifetime;
    private Transform target; 

    void Start()
    {
        
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (target == null)
        {
            
            Debug.LogError("Player not found!");
            Destroy(gameObject); 

        }
        if (lifetime <= 0) lifetime = 2.0f;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Destroy(gameObject);

        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            
            Destroy(gameObject);

        }
    }
}
