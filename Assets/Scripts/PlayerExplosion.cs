using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosion : MonoBehaviour
{
    GameManager gm;
    SpriteRenderer sr;
    Sprite[] sprites;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("Sprites & Texts/Player Explosion Original");
        StartCoroutine(PlayerExplosionAnimation());
    }
    private IEnumerator PlayerExplosionAnimation()
    {
        for(int i = 0; i < sprites.Length; i++)
        {   
            sr.sprite = sprites[i];
            yield return new WaitForSeconds(gm.playerExplosionTime);
        }
        Destroy(gameObject);
    }
    
}

    

