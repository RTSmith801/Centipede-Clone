using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    //Sprites
    Sprite[] centipedeHeadSpriteAtalas;
    Sprite[] centipedeBodySpriteAtalas;
    string animatorTriggerName;
    Animator animator;

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

        SpriteGeneration();
    }

    //No longer needed? 
    override protected void GenerateSpriteAtlas()
    {
        centipedeHeadSpriteAtalas = Resources.LoadAll<Sprite>("Sprites & Texts/Centipede Head");
        centipedeBodySpriteAtalas = Resources.LoadAll<Sprite>("Sprites & Texts/Centipede Body");
        animator = GetComponent<Animator>();
        SpriteGeneration();
        animator.SetTrigger(animatorTriggerName);
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
                MoveStateSwitch(nextState);
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
                MoveStateSwitch(MoveState.ascend);
            }
            else
            {
                MoveStateSwitch(MoveState.lateral_descend);
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
                MoveStateSwitch(MoveState.lateral_descend);
            }
            else
            {
                MoveStateSwitch(MoveState.lateral_ascend);
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
                MoveStateSwitch(MoveState.descend);
            }
            else if (moveState == MoveState.lateral_ascend)
            {
                verticalTarget = transform.position.y + 1;
                MoveStateSwitch(MoveState.ascend);
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

    void MoveStateSwitch(MoveState ms)
    {
        moveState = ms;
        SpriteGeneration();
        animator.SetTrigger(animatorTriggerName);
    }

    void SpriteGeneration()
    {
        if (isHead)
        {
            //using centipedeHeadSpriteAtalas
            //check MoveState
            switch (moveState)
            {
                case MoveState.lateral_ascend:
                case MoveState.lateral_descend:
                    if (movingRight)
                    {
                        //use centipedeHeadSpriteAtalas 18 - 23
                        //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + 18);
                        animatorTriggerName = "CentipedeHeadMoveRight";
                    }
                    else
                    {
                        //use centipedeHeadSpriteAtalas 6 - 11
                        //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + 6);
                        animatorTriggerName = "CentipedeHeadMoveLeft";
                    }
                    break;
                case MoveState.descend:
                case MoveState.dive:
                    //use centipedeHeadSpriteAtalas 12 - 17
                    //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + 12);
                    animatorTriggerName = "CentipedeHeadMoveDown";
                    break;
                case MoveState.ascend:
                    //use centipedeHeadSpriteAtalas 0 - 5
                    //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + 0);
                    animatorTriggerName = "CentipedeHeadMoveUp";
                    break;                
                default:
                    break;
            }

        }
        else
        {
            //using centipedeBodySpriteAtalas
            //check MoveState
            switch (moveState)
            {
                case MoveState.lateral_ascend:
                case MoveState.lateral_descend:
                    if (movingRight)
                    {
                        //use centipedeHeadSpriteAtalas 18 - 23
                        //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + 18);
                        animatorTriggerName = "CentipedeBodyMoveRight";
                    }
                    else
                    {
                        //use centipedeHeadSpriteAtalas 6 - 11
                        //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + 6);
                        animatorTriggerName = "CentipedeBodyMoveLeft";
                    }
                    break;
                case MoveState.descend:
                case MoveState.dive:
                    //use centipedeHeadSpriteAtalas 12 - 17
                    //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + 12);
                    animatorTriggerName = "CentipedeBodyMoveDown";
                    break;
                case MoveState.ascend:
                    //use centipedeHeadSpriteAtalas 0 - 5
                    //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + 0);
                    animatorTriggerName = "CentipedeBodyMoveUp";
                    break;
                default:
                    break;
            }

        }
        
        //sr.sprite = centipedeHeadSpriteAtalas.Single(s => s.name == "Centipede Head_" + spriteNum);
    }
}
