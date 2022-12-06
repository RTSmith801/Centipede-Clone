using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centipede : Enemy
{
    [SerializeField]
    bool isHead = true;
    //public int pts = 10;
    bool movingRight = true;
    public float moveSpeed = 5f;
    Vector2 castDirection = Vector2.right;
    [SerializeField]
    MoveState moveState = MoveState.lateral_descend;

    [SerializeField]
    float verticalTarget;

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
                Lateral(-1, MoveState.descend);
                break;
            case MoveState.descend:
                Descend();
                break;
            case MoveState.dive:
                break;
            case MoveState.ascend:
                Ascend();
                break;
            case MoveState.lateral_ascend:
                Lateral(1, MoveState.ascend);
                break;
            default:
                break;
        }


    }

    /// <summary>
    /// 1 is for ascend, -1 is for descend
    /// </summary>
    /// <param name="direction"></param>
    void Lateral(int direction, MoveState nextState)
    {
        Vector3 center = transform.localPosition + new Vector3(.5f, .5f);
        RaycastHit2D[] collide = Physics2D.BoxCastAll(center, new Vector2(.9f, .9f), 0f, castDirection, moveSpeed * Time.deltaTime);

        foreach (RaycastHit2D collision in collide)
        {
            if (collision.transform.gameObject.tag == "mushroom")
            {
                //clamp to a column          
                verticalTarget = transform.position.y + direction;
                float x = Mathf.Round(transform.position.x);
                transform.position = new Vector3(x, transform.position.y);

                UpdateCastDirection();
                moveState = nextState;
                return;
            }
        }

        // Do we love this?
        float xDir;
        if (movingRight)
            xDir = 1;
        else
            xDir = -1;

        float horizontal = xDir * Time.deltaTime * moveSpeed;
        transform.Translate(horizontal, 0, 0);

        CheckSideBoundaries();
    }

    
    void Descend()
    {
        if (transform.position.y > verticalTarget)
        {
            transform.Translate(0, Time.deltaTime * moveSpeed * -1, 0);
        }
        else
        {
            float y = verticalTarget;
            transform.position = new Vector3(transform.position.x, verticalTarget);

            // check for bottom of screen
            if (transform.position.y <= 0)
            {
                verticalTarget = 1;
                moveState = MoveState.ascend;
            }
            else
            {
                moveState = MoveState.lateral_descend;
            }
        }


    }

    void Ascend()
    {
        if (transform.position.y < verticalTarget)
        {
            transform.Translate(0, Time.deltaTime * moveSpeed * 1, 0);
        }
        else
        {
            // clamp
            float y = verticalTarget;
            transform.position = new Vector3(transform.position.x, verticalTarget);

            // check for top of player boundary
            if (transform.position.y >= gm.movementBoundaryY - 1)
            {
                verticalTarget = gm.movementBoundaryY - 2;
                moveState = MoveState.lateral_descend;
            }
            else
            {
                moveState = MoveState.lateral_ascend;
            }
        }
    }

    void CheckSideBoundaries()
    {
        if (transform.position.x <= 0 || transform.position.x >= gm.movementBoundaryX - 1f)
        {
            //move down
            UpdateCastDirection();
            float x = Mathf.Clamp(transform.position.x, 0, gm.movementBoundaryX - 1f);
            transform.position = new Vector3(x, transform.position.y);

            if (moveState == MoveState.lateral_descend)
            {
                verticalTarget = transform.position.y - 1;
                moveState = MoveState.descend;
            }
            else if (moveState == MoveState.lateral_ascend)
            {
                verticalTarget = transform.position.y + 1;
                moveState = MoveState.ascend;
            }
            else
                Debug.Log("you didn't expect this motherfucker");
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
    
    enum MoveState { lateral_descend, descend, dive, ascend, lateral_ascend, follow };
}
