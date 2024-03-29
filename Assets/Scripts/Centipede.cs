using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Centipede : Enemy
{
    [SerializeField]
    public bool isHead = false;    
    bool movingRight = true;
    float moveSpeed;
    float diveXPosition;
    Vector2 castDirection = Vector2.right;
    [SerializeField]
    MoveState moveState = MoveState.lateral_descend;
    MoveState centipedeHeadMoveState = MoveState.lateral_descend;
    MoveStateDirection moveStateDirection = MoveStateDirection.down;

    [SerializeField]
    Centipede nodeAhead;
    [SerializeField]
    Centipede nodeBehind;
    int centipedeFollowFrames; //get from GameManager
    [SerializeField]
    //public List<Vector2> followQueue;
    public List<Tuple<Vector2, MoveStateDirection>> followQueue;



    [SerializeField]
    float verticalTarget;
    //used only by centipede followers
    //float horizontalTarget;

    //Animation
    string animatorTriggerName = "";
    Animator animator;

    float movementSoundTimer = .25f;

    public enum MoveState { lateral_descend, descend, dive, ascend, lateral_ascend, follow };
    public enum MoveStateDirection { up, left, down, right };

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //Called in Enemy.cs Start()
    override protected void LocalStart()
    {   
        centipedeFollowFrames = gm.centipedeFollowFrames;
        moveSpeed = gm.centipedeMoveSpeed;
        //isHead = false;
        //SpriteGeneration();
        StartCoroutine("MovementSound");
    }

    //Fixed Update to check if this changes build behavior. 
    private void FixedUpdate()
    {

        if (gm.pauseGame == false)
        {
            Move();
        }
		if (nodeBehind != null)
		{
            if (isHead) // This allows the head to pass it's state down the chain
                centipedeHeadMoveState = moveState;

			nodeBehind.FollowQueueAdd(transform.position, movingRight, centipedeHeadMoveState, moveStateDirection);
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
                Dive();
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

    public List<Centipede> GetNodesBehind(List<Centipede> nodeList)
    {
        nodeList.Add(this);

        if (nodeBehind != null)
            return nodeBehind.GetNodesBehind(nodeList);
        else
            return nodeList;

    }

    void Follow()
    {
        //get node ahead's transform 
        if (followQueue.Count >= centipedeFollowFrames)
        {
            transform.position = followQueue[0].Item1;
            moveStateDirection = followQueue[0].Item2;
            followQueue.RemoveAt(0);
        }
    }

    //Called from Game Manger when centipede is spawned.   
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
            isHead = false;
            followQueue = new List<Tuple<Vector2, MoveStateDirection>>();
            MoveStateSwitch(MoveState.follow);
        }
        SpriteGeneration();
    }

    public void LeaderUpdate()
    {
        // snap to the closest row
        transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), transform.position.z);

        isHead = true;

        // This makes it so shooting a diving centipede makes it exit it's dive
        if (centipedeHeadMoveState == MoveState.dive)
            MoveStateSwitch(MoveState.lateral_descend);
        else if (centipedeHeadMoveState == MoveState.lateral_descend) // This ensures it doesn't go back through itself
        {
            MoveStateSwitch(MoveState.descend);
        }
        else
            MoveStateSwitch(centipedeHeadMoveState);

        movingRight = !movingRight;
    }

    public void FollowQueueAdd(Vector2 newTarget, bool _movingRight, MoveState _centipedeHeadMoveState, MoveStateDirection _moveStateDirection)
    {

        
        followQueue.Add(new Tuple<Vector2, MoveStateDirection>(newTarget, _moveStateDirection));
        movingRight = _movingRight;
        centipedeHeadMoveState = _centipedeHeadMoveState;
        // Animation call here
    }

    void Dive()
    {
        if (transform.position.x > diveXPosition + 1 || transform.position.x < diveXPosition)
            movingRight = !movingRight;

        Vector3 movementDirection = movingRight? (Vector3.right + Vector3.down).normalized : (Vector3.left + Vector3.down).normalized;

        transform.position = transform.position + movementDirection * .15f;

        // If below zero, snap to zero and change state
        if (transform.position.y < 0)
        {
            verticalTarget = 1;
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            MoveStateSwitch(MoveState.ascend);
        }
    }

    /// <summary>
    /// 1 is for ascend, -1 is for descend
    /// </summary>
    /// <param name="direction"></param>
    void Lateral(int direction, MoveState nextState)
    {
        Vector3 center = transform.localPosition + new Vector3(.5f, .5f);
        RaycastHit2D[] collide = Physics2D.BoxCastAll(center, new Vector2(.8f, .8f), 0f, castDirection, moveSpeed);

        foreach (RaycastHit2D collision in collide)
        {
            bool hitAnotherCentipede = false;

            // check if enemy is a centipede head
            if (collision.transform.gameObject.tag == "enemy")
            {




                Centipede centipedeHit = collision.transform.GetComponent<Centipede>();
                // Makes sure that the collision is a centipede and not another type of enemy
                if (centipedeHit != null)
                {
                    List<Centipede> nodesBehind = GetNodesBehind(new List<Centipede>());
                    // check if the sunnovabitch is inside me
                    hitAnotherCentipede = !nodesBehind.Contains(centipedeHit);
                }
                


                //if (collision.transform.GetComponent<Centipede>() != null && self != other)
                //{
                //    hitAnotherCentipedeHead = collision.transform.GetComponent<Centipede>().isHead;
                //}
            }


            // Bounce logic
            if (collision.transform.gameObject.tag == "mushroom")
            {
				//clamp to a column          
				float x = Mathf.Round(transform.position.x);
				transform.position = new Vector3(x, transform.position.y);

                // If mushroom is poisoned, dive
				if (moveState != MoveState.dive && collision.transform.GetComponent<Mushroom>().isPoisoned)
                {
					MoveStateSwitch(MoveState.dive);
				}
                else
                {
					verticalTarget = transform.position.y + direction;

					UpdateCastDirection();
					MoveStateSwitch(nextState);
					return;
				}

            }
            else if (hitAnotherCentipede)
            {
				//clamp to a column          
				float x = Mathf.Round(transform.position.x);
				transform.position = new Vector3(x, transform.position.y);

				verticalTarget = transform.position.y + direction;

				UpdateCastDirection();
				MoveStateSwitch(nextState);
				return;
			}
		}

        float xDir = movingRight? 1 : -1;
        float horizontal = xDir * moveSpeed;
        transform.Translate(horizontal, 0, 0);

        CheckSideBoundaries();
    }

    
    void Descend()
    {
        if (transform.position.y > verticalTarget)
        {

            // This section with the boxcast ensures that heads won't stack
			//Vector3 center = transform.localPosition + new Vector3(.5f, .5f);
			//RaycastHit2D[] collide = Physics2D.BoxCastAll(center, new Vector2(.8f, .8f), 0f, Vector2.down, moveSpeed);

   //         foreach (RaycastHit2D hit in collide)
   //         {
   //             Centipede c = null;
   //             if (hit.transform.tag == "enemy" && hit.transform != transform)
   //                 c = hit.transform.GetComponent<Centipede>();

   //             if (c != null && c.isHead)
   //             {
   //                 MoveStateSwitch(MoveState.lateral_descend);
   //                 return;
   //             }

   //         }

			transform.Translate(0, moveSpeed * -1, 0);
        }
        if (transform.position.y < verticalTarget)
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
            transform.Translate(0, moveSpeed * 1, 0);
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
                MoveStateSwitch(MoveState.descend);
            }
            else if (moveState == MoveState.lateral_ascend)
            {
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
        //centipedeHeadMoveState = ms;
        //SpriteGeneration();

        //Set moveStateDirection here
        switch (ms)
        {
            case MoveState.ascend:
				gm.NotifyGMCentipedeReachedBottom();
				moveStateDirection = MoveStateDirection.up;
				verticalTarget = transform.position.y + 1;
				break;
            case MoveState.lateral_ascend:
				gm.NotifyGMCentipedeReachedBottom(); 
				if (movingRight)  // make this a ternary to reduce by 5 lines
                {
                    moveStateDirection = MoveStateDirection.right;
                }
                else
                {
                    moveStateDirection = MoveStateDirection.left;
                }
                break;
            case MoveState.descend:
				moveStateDirection = MoveStateDirection.down;
				verticalTarget = transform.position.y - 1;
                break;
			case MoveState.dive:
                diveXPosition = transform.position.x;

                // edge case if on the right edge of the screen
                if (diveXPosition >= gm.movementBoundaryX - 1)
                    diveXPosition -= 1;

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
        //animator.SetTrigger(animatorTriggerName);
        SpriteGeneration();
    }

    void SpriteGeneration()
    {
        if (isHead)
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
                    print("This is called as the centipede is spawned before it begins moving.");
                    //Placeholder animation before it starts moving
                    animatorTriggerName = "CentipedeBodyMoveDown";
                    break;
                default:
                    print("This should never be called");
                    break;
            }
        }
        else if (!isHead)
        {
            switch (moveStateDirection)
            {
                case MoveStateDirection.up:
                    animatorTriggerName = "CentipedeBodyMoveUp";
                    break;
                case MoveStateDirection.left:
                    animatorTriggerName = "CentipedeBodyMoveLeft";
                    break;
                case MoveStateDirection.down:
                    animatorTriggerName = "CentipedeBodyMoveDown";
                    break;
                case MoveStateDirection.right:
                    animatorTriggerName = "CentipedeBodyMoveRight";
                    break;
                default:
                    print("This should never be called");
                    break;
            }
        }
        animator.SetTrigger(animatorTriggerName);
    }

    public override void Hit()
    {

        if (nodeBehind)
        {
            nodeBehind.LeaderUpdate();
        }
              
        Instantiate(gm.mushroom, new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0), Quaternion.identity, gm.mushroomContainer.transform);

        gm.am.Play("boom2");
        gm.DecrementCentipedeList(this.gameObject);
        Die();
    }

	protected override void LocalDeath()
	{
		pts = isHead ? 100 : 10;
	}


    private IEnumerator MovementSound()
    {
        while (gameObject && !gm.pauseGame)
        {
            gm.am.Play("centipedemove");
            yield return new WaitForSeconds(movementSoundTimer);
        }
    }
}
