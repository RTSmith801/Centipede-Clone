using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mushroom : MonoBehaviour
{
    public int health = 4;
    public int pts = 1;
    public bool isPoisoned;

    GameManager gm;
    public SpriteRenderer sr;
    Sprite[] mushroomSpriteAtlas;
    Sprite[] MushroomSpriteAtlasPoisoned;

	int spriteNum;
    //float lifetimeAwake;
    //float lifetimeStart;
    //int awakeCounter = 0;

    private void Awake()
    {
        //awakeCounter++;
        //lifetimeAwake = Time.time;
        gm = FindObjectOfType<GameManager>();
        sr = GetComponent<SpriteRenderer>();
		mushroomSpriteAtlas = Resources.LoadAll<Sprite>("Sprites & Texts/Mushroom");
		MushroomSpriteAtlasPoisoned = Resources.LoadAll<Sprite>("Sprites & Texts/MushroomPoisoned");
	}

    // Start is called before the first frame update
    void Start()
    {
        //lifetimeStart = Time.time;
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

    public void Poison()
    {
        isPoisoned = true;
        SpriteGeneration();
        //sr.color = Color.blue;
        // change what it looks like
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
        else { spriteNum = 3; }

        Sprite[] currentAtlas = isPoisoned ? MushroomSpriteAtlasPoisoned : mushroomSpriteAtlas;
        if (sr.sprite)
        {
			//sr.sprite = mushroomSpriteAtlas.Single(s => s.name == "Mushroom_" + spriteNum);


			// This try catch was for troubleshohoting an issue where on edge case scenarios the hit() function was getting called
			// prior to Start() running, meaning the Atlases had not been set. Moving them to Awake() seems to have fixed it, but
            // I'm leaving this section here in case another rare edge case shows up.
			try
			{
				sr.sprite = currentAtlas[spriteNum];
			}
            catch (System.Exception)
            {
				//print("lifetimeAwake: " + lifetimeAwake + "; awakeCounter: " + awakeCounter);
				//float a = Time.time - lifetimeAwake;
				//float b = Time.time - lifetimeStart;
				print("You're hitting a weird edge case");
				//print("spriteNum: " + spriteNum + "; health: " + health + "; currentAtlas: " + currentAtlas + "; isPoisoned: " + isPoisoned + "; Times: " + a + "&" + b);
				//sr.color= Color.red;
				throw;
            }


		}
    }

    public void HealMushroom()
    {
		gm.scoreUpdate(5); 
        isPoisoned = false;
        sr.color = Color.white;
        health = 4;
        SpriteGeneration();
		gm.am.Play("boop");
	}
}
