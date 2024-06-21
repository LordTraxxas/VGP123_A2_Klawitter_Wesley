using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Shoot : MonoBehaviour
{

    SpriteRenderer sr;

    [Range(0, 10)]
    public float xVel;
    [Range(0, 10)]
    public float yVel;

    public Transform spawnPointLeft;
    public Transform spawnPointRight;

    public Projectile projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (xVel == 0 && yVel == 0) xVel = 7.0f;

        if (!spawnPointRight || !spawnPointLeft || !projectilePrefab)
            Debug.Log("Please set default values on the shoot script" + gameObject.name);
    }

    // Update is called once per frame
    public void Fire()
    {
        if (!sr.flipX)
        {
            Projectile curProjectile = Instantiate(projectilePrefab, spawnPointRight.position, spawnPointRight.rotation);
            curProjectile.xVel = xVel;
            curProjectile.yVel = yVel;
        }
        else
        {
            Projectile curProjectile = Instantiate(projectilePrefab, spawnPointLeft.position, spawnPointLeft.rotation);
            curProjectile.xVel = -xVel;
            curProjectile.yVel = yVel;
        }

    }
}