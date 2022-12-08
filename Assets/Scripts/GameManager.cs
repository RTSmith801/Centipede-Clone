using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public int highScore; // get and set using PlayerPrefs
    public bool highScoreReached = false;
    public int centipedeWave = 0;

    //Used for manually creating delays;
    public float centipedeDespawnTime = 0.1f;
    public float arenaGenerationTime = 0.0001f;
    public float gameOverTimer = 2f;
    public float playerExplosionTime = 0.025f;

    //Gameplay Variables
    //Used to start/stop gameplay
    public bool gameOver = false;
    public bool pauseGame = false;
    public int startingPlayerLives = 3;
    public int playerLives;
    //Used for initial mushroom generation
    public int maxRowDensity = 5;
    //Used for initial centipede generation call. 
    public int startingCentipedeGenerationCount = 10;
    //Incremented everytime all centipedes are destroyed
    int centipedeGenerationCount;
    [SerializeField]
    List<Centipede> centipedeLivingList; //total


    //used for FireLaser()
    public bool laserExists = false;

    //Prefabs loaded in from resources
    public GameObject laser;
    public GameObject mushroom;
    GameObject centipede;
    GameObject player;
    public GameObject playerExplosion;

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
        player = Resources.Load("Prefabs/Player") as GameObject;
        playerExplosion = Resources.Load("Prefabs/PlayerExplosion") as GameObject;
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
        gameOver = false;
        pauseGame = true;
        highScoreReached = false;
        score = 0;
        playerLives = startingPlayerLives;
        centipedeGenerationCount = startingCentipedeGenerationCount;
        Boundary();
        StartCoroutine(BuildArena());

        //Fetch the high score from the PlayerPrefs. If no Int of this name exists, the default is 0.
        highScore = PlayerPrefs.GetInt("highScore", 0);
    }

    //Sets boundary for player movement
    void Boundary()
    {
        movementBoundaryX = arena.transform.localScale.x;
        movementBoundaryY = arena.transform.localScale.y * movementBoundaryScale;
        movementBoundary.transform.localScale = new Vector3(1, movementBoundaryScale, 1);
        movementBoundary.transform.position = new Vector3(0, 0, 0);
    }

    //Builds arena then starts game
    private IEnumerator BuildArena()
    {
        //for (int i = 4; i <= arena.transform.localScale.y; i++)
        for (int i = (int)arena.transform.localScale.y; i > 3; i--)
        {
            int rowDensity = Random.Range(1, maxRowDensity + 1);
            List<int> density = new List<int>();
            while (density.Count < rowDensity)
            {
                int rnd = Random.Range(1, (int)arena.transform.localScale.x + 1);
                if (!density.Contains(rnd))
                {
                    Instantiate(mushroom, new Vector3(rnd - 1, i - 1, 0), Quaternion.identity);
                    density.Add(rnd);
                    yield return new WaitForSeconds(arenaGenerationTime);
                }
            }
        }
        NewCentipedeWave();
        am.FadeinBGM("BGM1");
        pauseGame = false;
    }

    private void Update()
    {
        if(gameOver && Input.GetButton("Fire1"))
        {   
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator RestartWave()
    {
        //Despawn enemies
        for (int i = centipedeLivingList.Count - 1; i >= 0; i--)
        {
            Destroy(centipedeLivingList[i].transform.gameObject);
            yield return new WaitForSeconds(centipedeDespawnTime);
        }
        centipedeLivingList.Clear();
        Vector2 instantionPoint = new Vector2(arena.transform.localPosition.x + 15, arena.transform.localPosition.y);
        Instantiate(player, instantionPoint, Quaternion.identity);
        scoreUpdate(0);
        pauseGame = false;
        SpawnCentipedes();
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
        for (int i = 0; i < centipedeGenerationCount; i++)
        {
            //Currently hardcoded - instation position (15, 39) - to randomize
            int rand = Random.Range(1, 31);
            Vector2 instantionPoint = new Vector2(rand, arena.transform.localPosition.y + 39);
            Centipede c = Instantiate(centipede, instantionPoint, Quaternion.identity).GetComponent<Centipede>();
            centipedeSpawnList.Add(c);
            centipedeLivingList.Add(c);
        }
        for (int i = 0; i < centipedeGenerationCount; i++)
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
        if(score > highScore)
        {
            highScore = score;
            highScoreReached = true;
        }
        scoreboard.text = "LIVES " + playerLives + "\nWAVE " + centipedeWave + "\nSCORE " + score + "\nHIGH SCORE " + highScore; 
    }

    public void DecrementCentipedeList(GameObject c)
    {
        //centipedeLivingList.RemoveAt(0);
        centipedeLivingList.Remove(c.GetComponent<Centipede>());
        if (centipedeLivingList.Count <= 0)
        {
            centipedeGenerationCount += 2;
            NewCentipedeWave();
        }
    }

    public void PlayerDeath()
    {
        //Bool used here to prevent multiple calls PlayerDeath()
        if (pauseGame == false)
        {   
            pauseGame = true;
            am.Play("boom1");
            am.Play("playerdeath");
            playerLives-= 1;
            if (playerLives >= 0)
            {
                StopAllCoroutines();
                StartCoroutine(RestartWave());
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(GameOver());
            }
        }
    }

    private IEnumerator GameOver()
    {
        if (highScoreReached)
        {
            //Set High Score
            //Flash High Score - To do
            scoreboard.text = "\nWAVE " + centipedeWave + "\nNEW HIGH SCORE " + highScore + "\nGAME OVER";
            PlayerPrefs.SetInt("highScore", highScore);
        }
        //Flash Game Over Text? 
        scoreboard.text = "\nWAVE " + centipedeWave + "\nSCORE " + score + "\nHIGH SCORE " + highScore + "\nGAME OVER";
        yield return new WaitForSeconds(gameOverTimer);

        //Press Fire To Restart. 
        gameOver = true;
    }
}
