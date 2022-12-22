using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spider : Enemy
{
    float moveSpeed;
	float topBoundary;
	float bottomBoundary;
	float moveTimer;
	float moveTimerMin;
	float moveTimerMax;
	bool isLeftSpider;

	public enum MoveStateDirection { up, upright, right, downright, down, downleft, left, upleft };
	// public enum MoveStateDirection { up=0, upright=1, right=2, downright=3, down=4, downleft=5, left=6, upleft=7 };
	MoveStateDirection moveState;

	// Update is called once per frame
	void Update()
    {
        moveTimer -= Time.deltaTime;
    }
    override protected void Move()
    {
		// if timer expires, or boundary hit, change direction
		if (moveTimer <= 0 || transform.position.y <= bottomBoundary || transform.position.y >= topBoundary)
		{
			ChangeDirections();
		}

		// Actually moves
        switch (moveState)
        {
            case MoveStateDirection.up:
                transform.position = transform.position + (Vector3.up * moveSpeed);
                break;
			case MoveStateDirection.upright:
				transform.position = transform.position + (new Vector3(1, 1).normalized * moveSpeed);
				break;
			case MoveStateDirection.right:
				transform.position = transform.position + (Vector3.right * moveSpeed);
				break;
            case MoveStateDirection.downright:
				transform.position = transform.position + (new Vector3(1, -1).normalized * moveSpeed);
				break;
            case MoveStateDirection.down:
				transform.position = transform.position + (Vector3.down * moveSpeed);
				break;
            case MoveStateDirection.downleft:
				transform.position = transform.position + (new Vector3(-1, -1).normalized * moveSpeed);
				break;
			case MoveStateDirection.left:
				transform.position = transform.position + (Vector3.left * moveSpeed);
				break;
            case MoveStateDirection.upleft:
                transform.position = transform.position + (new Vector3(-1, 1).normalized * moveSpeed);
                break;
            default:
                break;
        }

		// Check if off screen, if so despawn
		if ((isLeftSpider && transform.position.x >= gm.movementBoundaryX) || (!isLeftSpider && transform.position.x <= -2))
		{
			Destroy(gameObject);
		}

    }


	void ChangeDirections()
	{
		moveTimer = Random.Range(moveTimerMin, moveTimerMax);

		List<MoveStateDirection> possibleDirections = new List<MoveStateDirection>();

		possibleDirections.Add(MoveStateDirection.up);
		possibleDirections.Add(MoveStateDirection.down);

		if (isLeftSpider)
		{
			possibleDirections.Add(MoveStateDirection.right);
			possibleDirections.Add(MoveStateDirection.upright);
			possibleDirections.Add(MoveStateDirection.downright);
		}
		else
		{
			possibleDirections.Add(MoveStateDirection.left);
			possibleDirections.Add(MoveStateDirection.upleft);
			possibleDirections.Add(MoveStateDirection.downleft);
		}

		if (transform.position.y <= bottomBoundary)
			possibleDirections.Remove(MoveStateDirection.down);
		if (transform.position.y >= topBoundary)
			possibleDirections.Remove(MoveStateDirection.up);

		moveState = possibleDirections[Random.Range(0, possibleDirections.Count)];

	}

	override protected void LocalStart()
    {
        moveSpeed = gm.spiderMoveSpeed;
		topBoundary= gm.spiderTopBoundary;
		bottomBoundary= gm.spiderBottomBoundary;
		moveTimerMin = gm.spiderMoveTimerMin;
		moveTimerMax = gm.spiderMoveTimerMax;

		moveTimer = Random.Range(moveTimerMin, moveTimerMax);


		// Set the spider direction and moveState based on spawn location
		isLeftSpider = transform.position.x <= 0 ? true : false;
		moveState = isLeftSpider ? (MoveStateDirection)Random.Range(1, 4) : (MoveStateDirection)Random.Range(5, 8);



	}

	protected override void LocalDeath()
	{

		Player player = FindObjectOfType<Player>();
		float verticalDistanceFromPlayer = transform.position.y - player.transform.position.y;

		GameObject pointsText = null;
		switch (verticalDistanceFromPlayer)
		{
			case < 3:
				pts = 900;
				pointsText = gm.points900;
				break;
			case < 7:
				pts = 600;
				pointsText = gm.points600;
				break;
			default:
				pts = 300;
				pointsText = gm.points300;
				break;
		}

		gm.am.Play("boom2");
		gm.SpawnSpider();
		
		Instantiate(pointsText, new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0), Quaternion.identity);
	}
}
