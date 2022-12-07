using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// All resource load handled by GameManager
/// Keeps track of player lives & centipedes alive
/// Spawns enemies
/// Controls arena size
/// Keeps track of score  
/// Keeps track of high-score - to do
/// Add life for every X score reached - to do
/// </summary>
public class GameManager : MonoBehaviour
{   
    [Header("Boundary size")]
    public float movementBoundaryScale = .25f; //Keep divisible by 4

    [Header("Game Stats")]
    public int score = 0;
    public int highScore = 0;
    public int centipedeWave = 0;

    //Used for initial mushroom generation
    public int maxRowDensity = 5;
    //Used for initial centipede generation call. 
    //Incremented everytime all centipedes are destroyed
    int centipedeGeneration = 10;

    [SerializeField]
    List<Centipede> centipedeLivingList; //total

    //Prefabs loaded in from resources
    public GameObject laser;
    GameObject mushroom;
    GameObject centipede;

    //Required in Unity Scene
    [Header("Audio Manager")]
    public AudioManager am; //Is public so AudioManager can be called by through GameManager.
    public GameObject arena; //Create accessor for this 
    public GameObject movementBoundary; //create accessor for this 
    public float movementBoundaryX; //create accessor for this 
    public float movementBoundaryY; //create accessor for this 
    TextMeshProUGUI scoreboard;
    public Camera mainCam;

    private void Awake()
    {
        LoadPrefabs();
        BuildReferences();
    }

    void LoadPrefabs()
    {
        laser = Resources.Load("Prefabs/Laser") as GameObject;
        mushroom = Resources.Load("Prefabs/Mushroom") as GameObject;
        centipede = Resources.Load("Prefabs/Centipede") as GameObject;
    }

    void BuildReferences()
    {
        am = FindObjectOfType<AudioManager>();
        arena = GameObject.FindWithTag("arena");
        movementBoundary = GameObject.FindWithTag("movement boundary");
        scoreboard = GameObject.FindWithTag("scoreboard").GetComponent<TextMeshProUGUI>();
        mainCam = GameObject.FindObjectOfType<Camera>();
    }
    
    void Start()
    {   
        Boundary();
        BuildLevel();
        NewCentipedeWave();
        score = 0;
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

    void NewCentipedeWave()
    {
        centipedeWave++;
        scoreUpdate(0);
        SpawnCentipedes();
    }

    //Instantiaion point needs to be randomized here
    void SpawnCentipedes()
    {   
        //set nodeAhead/nodeBehind for centipedes.
        List<Centipede> centipedeSpawnList = new List<Centipede>();
        for (int i = 0; i < centipedeGeneration; i++)
        {
            //Currently hardcoded - instation position (15, 39) - to randomize
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
    }

    public void scoreUpdate(int pts)
    {
        score += pts;
        scoreboard.text = "SCORE " + score + "\nWAVE " + centipedeWave; 
    }

    public void DecrementCentipedeList()
    {
        centipedeLivingList.RemoveAt(0);
        if(centipedeLivingList.Count <= 0)
        {
            centipedeGeneration += 2;
            NewCentipedeWave();
        }
    }

    public void PlayerDeath()
    {
        am.Play("boom1");
        am.Play("playerdeath");
    }
}
