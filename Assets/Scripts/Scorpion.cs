using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : Enemy
{
    bool isLeftScorpion;
    float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        
    }
    override protected void Move()
    {
        Vector3 moveDir = isLeftScorpion ? Vector3.right : Vector3.left;

        transform.position = transform.position + (moveDir * moveSpeed);

        // Check if needs to despawn
        if ((isLeftScorpion && transform.position.x > gm.movementBoundaryX + 2) || (!isLeftScorpion && transform.position.x < -2)) 
        {
			gm.SpawnScorpion();
			Destroy(gameObject);
		}
    }
    override protected void LocalStart()
    {
        moveSpeed = gm.scorpionMoveSpeed;
		isLeftScorpion = transform.position.x <= 0 ? true : false;

        if (isLeftScorpion)
            transform.localScale = new Vector3 (-1,1,1);
	}

	protected override void LocalDeath()
	{
		throw new System.NotImplementedException();
	}
}
