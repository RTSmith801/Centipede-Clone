using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public GameManager gm;
    public SpriteRenderer sr;
    int health = 1; //Leaving hit function in case there are enemies with health > 1
    //protected int pts;
    protected int pts;


    

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        sr = GetComponent<SpriteRenderer>();
        LocalStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.pauseGame)
        {
            Move();
        }
    }

    protected abstract void Move();
    protected abstract void LocalStart();
    public virtual void Hit()
    {
        health--;

        if (health <= 0)
        {
            Die(pts);
        }
    }

    protected void Die(int pts)
    {
        gm.scoreUpdate(pts);
        Destroy(gameObject);
    }

}
