using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centipede : Enemy
{
    bool isHead = true;
    public int pts = 10;
    bool movingRight = true;
    public float moveSpeed = 5f;
    Vector2 castDirection = Vector2.right;
    MoveState moveState = MoveState.lateral_descend;

    // Update is called once per frame
    void Update()
    {
        if (isHead){
            pts = 100;
        }else{
            pts = 10;
        }

        Move();
    }

    override protected void Move()
    {
        //move logic

        switch (moveState)
        {
            case MoveState.lateral_descend:
                LateralDescend();
                break;
            case MoveState.descend:
                break;
            case MoveState.dive:
                break;
            case MoveState.ascend:
                break;
            case MoveState.lateral_ascend:
                break;
            default:
                break;
        }


    }

    void LateralDescend()
    {
        Vector3 center = transform.localPosition + new Vector3(.5f, .5f);
        RaycastHit2D[] collide = Physics2D.BoxCastAll(center, new Vector2(.9f, .9f), 0f, castDirection, moveSpeed * Time.deltaTime);

        foreach (RaycastHit2D collision in collide)
        {
            if (collision.transform.gameObject.tag == "mushroom")
            {
                //move down
                //change direction
                UpdateCastDirection();
                return;
            }
        }
        float xDir;
        if (movingRight)
        {
            xDir = 1;
        }
        else
        {
            xDir = -1;
        }


        //float vertical = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;
        float horizontal = xDir * Time.deltaTime * moveSpeed;
        transform.Translate(horizontal, 0, 0);

        if (transform.position.x <= 0 || transform.position.x >= gm.movementBoundaryX - 1f)
        {
            //move down
            UpdateCastDirection();
            float x = Mathf.Clamp(transform.position.x, 0, gm.movementBoundaryX - 1f);
            transform.position = new Vector3(x, transform.position.y);
        }
    }

    void UpdateCastDirection()
    {
        movingRight = !movingRight;

        if (movingRight)
        {
            //move right
            castDirection = Vector2.right;
        }
        else
        {
            //move left
            castDirection = Vector2.left;
        }
    }
    
    enum MoveState { lateral_descend, descend, dive, ascend, lateral_ascend };
}
