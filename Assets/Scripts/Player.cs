using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If player is not in scene, should be instantiated by GameManager
/// 
/// </summary>
public class Player : MonoBehaviour
{
    GameManager gm;
    
    //Variables
    Vector2 movement;
    Vector3 mousePos;
    //used for PlayerMoveKeyboard()
    [Header("Speed controls")]
    public float playerSpeed = 10f;
    [Header("Laser position")]
    public float laserPosition = 1f;
    
    //used for PlayerMoveMouse()
    float horizontalSpeed = 2.0f;
    float verticalSpeed = 2.0f;

    // laser throttling variables
    float laserDelay = .1f;
    float laserTimer = 0f;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (!gm.pauseGame)
    //    {
    //        //Player movement
    //        //PlayerMoveKeyboard();
    //        PlayerMoveMouse();
    //        if (Input.GetButton("Fire1"))
    //        {
    //            FireLaser();
    //        }
    //    }
    //}
    
    void FixedUpdate()
    {
        if (!gm.pauseGame)
        {
            //Player movement
            //PlayerMoveKeyboard();
            PlayerMoveMouse();
            if (Input.GetButton("Fire1") && Time.time > laserTimer)
            {
                FireLaser();
            }
        }
    }

    //fire laser call
    void FireLaser()
    {
        //check if laser exists - currently, allows only a single laser
        if(!gm.laserExists)
        {   
            //fire laser
            Vector3 position = transform.position; 
            position += new Vector3(0.4375f, laserPosition);
            Instantiate(gm.laser, position, Quaternion.identity);

            laserTimer = Time.time + laserDelay;
			gm.am.Play("laser");

            
		}   
    }

    //Player movement keyboard
    void PlayerMoveKeyboard()
    {
        if (Input.anyKeyDown)
        {   
            Cursor.visible = false;
        }

        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;
        float distance = new Vector3(horizontal, vertical, 0f).magnitude;

        Vector3 center = transform.localPosition + new Vector3(.5f, .5f);
        RaycastHit2D[] collide = Physics2D.BoxCastAll(center, new Vector2(.9f, .9f), 0f, new Vector2(horizontal, vertical), distance);

        foreach(RaycastHit2D collision in collide)
        {
            if (collision.transform.gameObject.tag == "mushroom")
            {   
                return;
            }
        }        
        transform.Translate(horizontal, vertical, 0);
        ClampPlayerMovement();
    }

    //Player movement mouse
    void PlayerMoveMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        mousePos = gm.mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Raycast logic to slide the ship around mushrooms. If the mouse is colliding with the mushroom
        // it places the player outside the mushroom 1 unit in the direction of the cursor
		Vector3 center = mousePos + new Vector3(.4375f, .5f); // the .4375f is because the sprite is 7 pixels wide, not 8. function = (7 / 8) / 2
		RaycastHit2D[] collisions = Physics2D.CircleCastAll(center, .5f, Vector2.zero);
        foreach (RaycastHit2D hit in collisions)
        {
            if (hit.transform.tag == "mushroom")
            {
                print("hitting mushroom");
                Vector3 vectorBetweenMushroomAndCursor = center - hit.transform.position;
                transform.position = hit.transform.position + vectorBetweenMushroomAndCursor.normalized;
                ClampPlayerMovement();
                return;
            }
                
        }

		transform.position = mousePos;
        ClampPlayerMovement();
    }

    void ClampPlayerMovement()
    {
        float x = Mathf.Clamp(transform.position.x, 0, gm.movementBoundaryX - 1);
        float y = Mathf.Clamp(transform.position.y, 0, gm.movementBoundaryY - 1);
        transform.position = new Vector3(x, y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Player death called when colliding with enemy
        if (collision.tag == "enemy")
        {
            gm.PlayerDeath();
            Vector3 position = transform.position;
            //position += new Vector3(0.4375f, laserPosition);
            Instantiate(gm.playerExplosion, position, Quaternion.identity);
            Destroy(gameObject);
        }  
    }
}
