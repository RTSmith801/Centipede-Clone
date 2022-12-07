using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instatiation called from Player.cs
/// Requires GameManger to check arena size and to play audio
/// Be sure ProjectSettings >> Fixed Timestamp = 0.01
/// laserSpeed at 100 will move laser up 1 unit (8 pixels) every frame
/// </summary>
public class Laser : MonoBehaviour
{
    GameManager gm;
    Rigidbody2D rb;
    float laserSpeed = 100f;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        rb = this.transform.gameObject.GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        //Movement called once
        Movement();
        //Laser sound called here
        gm.laserExists = true;
        gm.am.Play("laser");
    }
    
    void Update()
    {
        //Destroy laser if laser has left arena
        if (transform.localPosition.y >= gm.arena.transform.localScale.y - 1)
        {
            LaserDespawn();
        }
    }

    //Laser movement with physics
    void Movement()
    {   
        rb.velocity = new Vector3(0, laserSpeed, 0);        
    }

    //Collision check - be sure all tags are set on colliders
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.tag == "enemy")
        {
            collision.GetComponent<Enemy>().Hit();
            LaserDespawn();
        }

        if (collision.tag == "mushroom")
        {   
            Mushroom mushroom = collision.gameObject.GetComponent<Mushroom>();
            mushroom.Hit();
            LaserDespawn();
        }
    }

    void LaserDespawn()
    {
        gm.laserExists = false;
        Destroy(this.gameObject);
    }
}
