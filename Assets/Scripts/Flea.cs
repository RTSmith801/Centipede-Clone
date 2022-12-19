using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flea : Enemy
{
    public float moveSpeed = .15f;
    [SerializeField]
    private float mushroomSpawnChance = .15f;

    float yTarget;

	// Update is called once per frame
	void Update()
    {
        
    }
    override protected void Move()
    {
        transform.position = transform.position + Vector3.down * moveSpeed;

        if (transform.position.y < -1)
            Die(0);

        if (transform.position.y < yTarget && transform.position.y > 1)
        {
            yTarget -= 1;
			float rnd = Random.Range(0f, 1f);
			if (rnd <= mushroomSpawnChance)
			{
				// spawn mushroom
				Instantiate(gm.mushroom, new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0), Quaternion.identity);
			}
		}
    }
    override protected void LocalStart()
    {
        pts = 200;
        health = 2;
        yTarget = transform.position.y - 1;
    }

}
