using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject arena;
    [HideInInspector]
    public GameObject movementBoundary;
    [HideInInspector]
    public float movementBoundaryX;
    [HideInInspector]
    public float movementBoundaryY;
    [Header("Boundary size")]
    public float movementBoundaryScale = .33f;
    [HideInInspector]
    public float boundaryOffset;

    [Header ("Game Stats")]
    public int score = 0;
    public int highScore = 0;

    [HideInInspector]
    public TextMeshProUGUI scoreboard;

    private GameObject mushroom;
    public int maxRowDensity = 5;

    // Start is called before the first frame update
    void Start()
    {
        //Keeps track of lives
        //Starting lives 3
        //Keeps track of score
        //Add life for every X score reached
        //Keeps track of high-score
        //Controls enemy spawn
        //instantiate mushrooms
        arena = GameObject.FindWithTag("arena");
        movementBoundary = GameObject.FindWithTag("movement boundary");
        mushroom = Resources.Load("Prefabs/Mushroom") as GameObject;
        Boundary();
        BuildLevel();
        score = 0;
        scoreboard = GameObject.FindWithTag("scoreboard").GetComponent<TextMeshProUGUI>();
    }
    
    //Sets boundary for player movement
    void Boundary()
    {
        movementBoundaryX = arena.transform.localScale.x / 2;
        movementBoundaryY = arena.transform.localScale.y * movementBoundaryScale / 2;
        boundaryOffset = (arena.transform.localScale.y * (1 - movementBoundaryScale) / 2);
        movementBoundary.transform.localScale = new Vector3(1, movementBoundaryScale, 1);
        movementBoundary.transform.position = new Vector3(0, -boundaryOffset, 0);
    }

    void BuildLevel()
    {
        for(int i = 4; i <= arena.transform.localScale.y; i++)
        {   
            int rowDensity = Random.Range(1, maxRowDensity + 1);           
            List<int> density = new List<int>();            
            while(density.Count < rowDensity)
            {
                int rnd = Random.Range(1, (int)arena.transform.localScale.x + 1);
                if (!density.Contains(rnd))
                {
                    Instantiate(mushroom, new Vector3(rnd - movementBoundaryX - .5f, i - (arena.transform.localScale.y / 2) - .5f, 0), Quaternion.identity);
                    density.Add(rnd);
                }
            }
        }
    }

    public void scoreUpdate(int pts)
    {
        score += pts;
        scoreboard.text = "Score " + score; 
    }
}
