using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mushroom : MonoBehaviour
{
    public int health = 4;
    public int pts = 1;

    GameManager gm;
    SpriteRenderer sr;
    Sprite[] mushroomSpriteAtalas;
    int spriteNum;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mushroomSpriteAtalas = Resources.LoadAll<Sprite>("Sprites & Texts/Mushroom");
        spriteNum = health;
        SpriteGeneration();
        //print(sr.sprite);
        //print(sr);
        gm.am.Play("boop");
    }

    private void Update()
    {
        if (health <= 0)
        {
            gm.scoreUpdate(pts);
            gm.am.Play("boop");
            Destroy(gameObject);            
        }
    }

    public void Hit()
    {   
        health--;
        SpriteGeneration();
    }

    void SpriteGeneration()
    {
        if (health == 4) { spriteNum = 0; }
        else if (health == 3) { spriteNum = 1; }
        else if (health == 2) { spriteNum = 2; }
        else if (health == 1) { spriteNum = 3; }
        if (sr.sprite)
        {
            sr.sprite = mushroomSpriteAtalas.Single(s => s.name == "Mushroom_" + spriteNum);
        }
    }
}
