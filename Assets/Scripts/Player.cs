using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Variables
    Rigidbody2D rb;
    Vector2 movement;
    //used for PlayerMoveKeyboard()
    public float playerSpeed = 10f;
    //used for PlayerMoveMouse()
    float horizontalSpeed = 2.0f;
    float verticalSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        //set position
        //set health to 1
        //check number of lives
        //game manager will control score, lives, enemy spawn, etc
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Player movement
        PlayerMoveKeyboard();
        //PlayerMoveMouse();
        //fire
        //collisions for player hit by enemy

    }

    //Player movement keyboard
    void PlayerMoveKeyboard()
    {
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;
        transform.Translate(horizontal, vertical, 0);
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
