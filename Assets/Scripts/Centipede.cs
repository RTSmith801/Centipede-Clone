using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Centipede : Enemy
{
    [SerializeField]
    public bool isHead = false;    
    bool movingRight = true;
    public float moveSpeed = 5f;
    Vector2 castDirection = Vector2.right;
    [SerializeField]
    MoveState moveState = MoveState.lateral_descend;
    MoveState centipedeHeadMoveState = MoveState.lateral_descend;
    MoveStateDirection moveStateDirection = MoveStateDirection.down;

    [SerializeField]
    Centipede nodeAhead;
    [SerializeField]
    Centipede nodeBehind;
    public int followFrames = 20;
    [SerializeField]
    public List<Vector2> followQueue;

    [SerializeField]
    float verticalTarget;
    //used only by centipede followers
    //float horizontalTarget;

    //Animation
    string animatorTriggerName;
    Animator animator;

    public enum MoveState { lateral_descend, descend, dive, ascend, lateral_ascend, follow };
    public enum MoveStateDirection { up, left, down, right };

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //Called in Enemy.cs Start()
    override protected void LocalStart()
    {   
        isHead = false;
        SpriteGeneration();
    }

    // Update is called once per frame
    void Update()
    {
        if (nodeBehind != null)
        {
            nodeBehind.FollowQueueAdd(transform.position, movingRight, centipedeHeadMoveState);
        }
        if(gm.pauseGame == false)
        {
            Move();
        }
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
            case MoveState.follow:
                Follow();
                break;
            default:
                break;
        }

        SpriteGeneration();
    }

    void Follow()
    {
        //get node ahead's transform 
        if (followQueue.Count >= followFrames)
        {
            transform.position = followQueue[0];
            followQueue.RemoveAt(0);
        }
    }
   
    public void Initialized(Centipede a, Centipede b)
    {
        nodeAhead = a;
        nodeBehind = b;

        if(nodeAhead == null)
        {
            isHead = true;
            MoveStateSwitch(MoveState.lateral_descend);
        }
        else
        {
            followQueue = new List<Vector2>();
            MoveStateSwitch(MoveState.follow);
        }
    }

    public void LeaderUpdate()
    {
        isHead = true;
        MoveStateSwitch(centipedeHeadMoveState);
        movingRight = !movingRight;
    }

    public void FollowQueueAdd(Vector2 newTarget, bool _movingRight, MoveState _centipedeHeadMoveState)
    {
        
        followQueue.Add(newTarget);
        movingRight = _movingRight;
        centipedeHeadMoveState = _centipedeHeadMoveState;
        // Animation call here
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
            bool hitAnotherCentipedeHead = false;

            // check if enemy is a centipede head
            if (collision.transform.gameObject.tag == "enemy")
            {
                int other = collision.transform.gameObject.GetInstanceID();
                int self = transform.gameObject.GetInstanceID();

                if (collision.transform.GetComponent<Centipede>() != null && self != other)
                {
                    hitAnotherCentipedeHead = collision.transform.GetComponent<Centipede>().isHead;
                }
            }



            if (collision.transform.gameObject.tag == "mushroom" || hitAnotherCentipedeHead)
            {
                //clamp to a column          
                verticalTarget = transform.position.y + direction;
                float x = Mathf.Round(transform.position.x);
                transform.position = new Vector3(x, transform.position.y);

                UpdateCastDirection();
                MoveStateSwitch(nextState);
                //it's a feature  
                //print("fuckery happening here");
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
                //MoveStateSwitch(MoveState.ascend);
                //Now allows for movement along bottom row
                MoveStateSwitch(MoveState.lateral_ascend);
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

   
    void MoveStateSwitch(MoveState ms)
    {
        moveState = ms;
        centipedeHeadMoveState = ms;
        SpriteGeneration();

        //Set moveStateDirection here
        switch (ms)
        {
            case MoveState.ascend:
                moveStateDirection = MoveStateDirection.up;
                break;
            case MoveState.lateral_ascend:
                if (movingRight)
                {
                    moveStateDirection = MoveStateDirection.right;
                }
                else
                {
                    moveStateDirection = MoveStateDirection.left;
                }
                break;
            case MoveState.descend:
            case MoveState.dive:
                moveStateDirection = MoveStateDirection.down;
                break;
            case MoveState.lateral_descend:
                if (movingRight)
                {
                    moveStateDirection = MoveStateDirection.right;
                }
                else
                {
                    moveStateDirection = MoveStateDirection.left;
                }
                break;
        }
        animator.SetTrigger(animatorTriggerName);
    }

    void SpriteGeneration()
    {   
            switch (moveState)
            {
                case MoveState.lateral_ascend:
                case MoveState.lateral_descend:
                    if (movingRight)
                    {   
                        animatorTriggerName = "CentipedeHeadMoveRight";
                    }
                    else
                    {
                        animatorTriggerName = "CentipedeHeadMoveLeft";
                    }
                    break;
                case MoveState.descend:
                case MoveState.dive:
                    animatorTriggerName = "CentipedeHeadMoveDown";
                    break;
                case MoveState.ascend:
                    animatorTriggerName = "CentipedeHeadMoveUp";
                    break;
                case MoveState.follow:
                    animatorTriggerName = "CentipedeBodyMoveUp";
                    break;
                default:
                    break;
            }
    }

    public override void Hit()
    {
        if (isHead)
        {
            pts = 100;
        }
        else
        {
            pts = 10;
        }

        if (nodeBehind)
        {
            nodeBehind.LeaderUpdate();
        }
              
        Instantiate(gm.mushroom, new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0), Quaternion.identity);

        gm.am.Play("boom2");
        gm.DecrementCentipedeList(this.gameObject);
        Die(pts);
    }
}
