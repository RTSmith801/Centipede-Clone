using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public int health = 4;
    public int pts = 1;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            gm.scoreUpdate(pts);
            Destroy(gameObject);            
        }
    }

    public void Hit()
    {   
        health--;
    }
}
