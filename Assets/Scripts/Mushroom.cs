using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mushroom : MonoBehaviour
{
    public int health = 4;
    public int pts = 1;

    GameManager gm;
    AudioManager am;
    SpriteRenderer sr;
    Sprite[] mushroomSpriteAtalas;
    int spriteNum;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        am = FindObjectOfType<AudioManager>();
        sr = GetComponent<SpriteRenderer>();
        mushroomSpriteAtalas = Resources.LoadAll<Sprite>("Sprites & Texts/Mushroom");
        spriteNum = health;
        SpriteGeneration();
        print(sr.sprite);
        print(sr);
    }

    private void Update()
    {
        if (health <= 0)
        {
            gm.scoreUpdate(pts);
            am.Play("boop");
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
        sr.sprite = mushroomSpriteAtalas.Single(s => s.name == "Mushroom_" + spriteNum);
    }
}
