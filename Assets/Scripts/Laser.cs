using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float laserSpeed = 100f;
    private Rigidbody2D rb;
    GameManager gm;
    //Be sure ProjectSettings >> Fixed Timestamp = 0.01

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        //instantiate at player position (slightly in front) 
        rb = this.transform.gameObject.GetComponent<Rigidbody2D>();
        Movement();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.y > gm.arena.transform.localScale.y - 1)
        {
            Destroy(this.gameObject);
        }
        Debug.Log("Laser speed = " + rb.velocity);
    }

    //Laser movement
    void Movement()
    {
        //move up along y axis
        //transform.position = transform.position + Vector3.up * laserSpeed * Time.deltaTime;
        //moving with physics
        rb.velocity = new Vector3(0, laserSpeed, 0);        
    }

    //Collision check
    //Be sure all tags are set on colliders
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.tag == "enemy")
        {
            collision.GetComponent<Enemy>().Hit();
            Destroy(this.gameObject);
        }

        if (collision.tag == "mushroom")
        {
            Debug.Log("laser collided with " + collision);
            Mushroom mushroom = collision.gameObject.GetComponent<Mushroom>();
            mushroom.Hit();

            //mushroom damagae +1
            //despawan
            Destroy(this.gameObject);
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    //*Note - laser has a frame where it is outside of the collider before is destroyed
    //    //can mask by putting a box around our arena? 
    //    if (collision.tag == "arena")
    //    {   
    //        Destroy(this.gameObject);
    //        //despawn
    //    }
    //}
}