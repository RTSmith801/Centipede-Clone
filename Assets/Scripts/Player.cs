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

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.pauseGame)
        {
            //Player movement
            PlayerMoveKeyboard();
            //PlayerMoveMouse();
            if (Input.GetButton("Fire1"))
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
        }   
    }

    //Player movement keyboard
    void PlayerMoveKeyboard()
    {
        if (Input.anyKeyDown)
        {
            //Cursor.lockState = CursorLockMode.None;
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
            print("locking cursor");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        mousePos = gm.mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
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
            Destroy(gameObject);
        }  
    }
}
