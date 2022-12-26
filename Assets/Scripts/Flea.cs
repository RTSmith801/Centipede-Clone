using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flea : Enemy
{
    public float moveSpeed = .15f;
    private float mushroomSpawnChance = .4f;

    float yTarget;

	// Update is called once per frame
	void Update()
    {
        
    }
    override protected void Move()
    {
        transform.position = transform.position + Vector3.down * moveSpeed;

        if (transform.position.y < -1)
        {
			gm.SpawnFlea();
			Destroy(gameObject); //removes flee without calling die *prevents instantiate points
		}


        if (transform.position.y < yTarget && transform.position.y > 1)
        {
            yTarget -= 1;
			float rnd = Random.Range(0f, 1f);
			if (rnd <= mushroomSpawnChance)
			{
				// spawn mushroom
				Instantiate(gm.mushroom, new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0), Quaternion.identity, gm.mushroomContainer.transform);
			}
		}
    }
    override protected void LocalStart()
    {

        pts = 200;
        health = 2;
        yTarget = transform.position.y - 1;
        gm.am.Play("falling");
    }

	protected override void LocalDeath()
	{
        gm.SpawnFlea();
        gm.am.Play("boom2");
        Instantiate(gm.points300, new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0), Quaternion.identity);
    }
}

