using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gm;
    public GameObject laser;
    
    //Variables
    Vector2 movement;
    //used for PlayerMoveKeyboard()
    [Header("Speed controls")]
    public float playerSpeed = 10f;
    [Header("Laser position")]
    public float laserPosition = 1f;
    
    //used for PlayerMoveMouse()
    float horizontalSpeed = 2.0f;
    float verticalSpeed = 2.0f;
    //used for FireLaser()
    bool laserExists = false;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        //set position
        //set health to 1
        //check number of lives
        //game manager will control score, lives, enemy spawn, etc        
        
    }

    // Update is called once per frame
    void Update()
    {
        //Player movement
        PlayerMoveKeyboard();
        //PlayerMoveMouse();
        //fire
        //collisions for player hit by enemy
        if(Input.GetButton("Fire1")){
            FireLaser();
        }

    }

    //fire laser call
    void FireLaser()
    {
        //check if laser exists 
        //alows only a single laser
        if (FindObjectOfType<Laser>()){
            laserExists = true;
        }else{
            laserExists = false;
            //fire laser
            Vector3 position = transform.position; 
            position += new Vector3(0.5f, laserPosition);
            Instantiate(laser, position, Quaternion.identity);
        }   
    }

    //Player movement keyboard
    void PlayerMoveKeyboard()
    {
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
        
        float x = Mathf.Clamp(transform.position.x, 0, gm.movementBoundaryX - 1);
        float y = Mathf.Clamp(transform.position.y, 0, gm.movementBoundaryY - 1);
        transform.position = new Vector3(x, y);
        
    }

    //Player movement mouse
    void PlayerMoveMouse()
    {
        // Get the mouse delta. This is not in the range -1...1
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");
        transform.Translate(h, v, 0);
    }

}
