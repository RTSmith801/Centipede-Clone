using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public GameManager gm;
    public SpriteRenderer sr;
    protected int health = 1; //Leaving hit function in case there are enemies with health > 1
    //protected int pts;
    protected int pts;


    

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        sr = GetComponent<SpriteRenderer>();
        LocalStart();
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!gm.pauseGame)
    //    {
    //        Move();
    //    }
    //}

    private void FixedUpdate()
    {
         if (!gm.pauseGame)
         {
             Move();
         }
    }

    protected abstract void Move();
    protected abstract void LocalStart();
	protected abstract void LocalDeath();
	public virtual void Hit()
    {
        health--;

        if (health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        LocalDeath();
        gm.scoreUpdate(pts);
        Destroy(gameObject);
    }

}
