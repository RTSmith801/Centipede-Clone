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
    public float movementBoundaryScale = .25f; //divisible by 4

    [Header("Game Stats")]
    public int score = 0;
    public int highScore = 0;

    [HideInInspector]
    public TextMeshProUGUI scoreboard;

    private GameObject mushroom;
    public int maxRowDensity = 5;

    [SerializeField]
    List<Centipede> centipedeLivingList; //total
    GameObject centipede;



    [Header("Audio Manager")]
    AudioManager am;

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
        centipede = Resources.Load("Prefabs/Centipede") as GameObject;
        Boundary();
        BuildLevel();
        SpawnCentipedes();
        score = 0;
        scoreboard = GameObject.FindWithTag("scoreboard").GetComponent<TextMeshProUGUI>();
        am = FindObjectOfType<AudioManager>();
        am.FadeinBGM("BGM1");
    }


    
    //Sets boundary for player movement
    void Boundary()
    {
        movementBoundaryX = arena.transform.localScale.x;
        movementBoundaryY = arena.transform.localScale.y * movementBoundaryScale;
        movementBoundary.transform.localScale = new Vector3(1, movementBoundaryScale, 1);
        movementBoundary.transform.position = new Vector3(0, 0, 0);
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
                    Instantiate(mushroom, new Vector3(rnd - 1, i - 1, 0), Quaternion.identity);
                    density.Add(rnd);
                }
            }
        }
    }

    void SpawnCentipedes()
    {
        //define how many cetipedes
        //set nodeAhead/nodeBehind for centipedes.
        List<Centipede> centipedeSpawnList = new List<Centipede>();
        int centipedeGeneration = 7;
        for (int i = 0; i < centipedeGeneration; i++)
        {
            //instation position (15, 39)
            //currently hardcoded
            Vector2 instantionPoint = new Vector2(arena.transform.localPosition.x + 15, arena.transform.localPosition.y + 39);
            Centipede c = Instantiate(centipede, instantionPoint, Quaternion.identity).GetComponent<Centipede>();
            centipedeSpawnList.Add(c);
            centipedeLivingList.Add(c);
        }
        for (int i = 0; i < centipedeGeneration; i++)
        {
            Centipede a;
            Centipede b;
            try
            {
                a = centipedeSpawnList[i - 1];                
            }

            catch
            {
                a = null;
            }

            try
            {   
                b = centipedeSpawnList[i + 1];
            }
            catch
            {
                b = null;            
            }
                centipedeSpawnList[i].Initialized(a, b);
        }
        //set instantion point
        //Random instantion point as wave progresses 
    }

    public void scoreUpdate(int pts)
    {
        score += pts;
        scoreboard.text = "Score " + score; 
    }
}
