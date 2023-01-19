using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// All resource load handled by GameManager
/// Keeps track of player lives & centipedes alive
/// Spawns enemies
/// Controls arena size
/// Keeps track of score  
/// Keeps track of high-score
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
    public int centipedeWave = -1;

    //Used for manually creating delays;
    public float centipedeDespawnTime = 0.1f;
    public float mushroomGenerationTime = 0.05f;
    public float gameOverTimer = 2f;
    public float playerExplosionTime = 0.05f;

    //Gameplay Variables
    //Used to start/stop gameplay
    public bool gameOver = false;
    public bool pauseGame = false;
    public int startingPlayerLives = 3;
    public int playerLives;
    public float laserSpeed = 80f;
    public int centipedeFollowFrames = 10;
    public float centipedeMoveSpeed = .1f;
    //Used for initial mushroom generation
    public int maxRowDensity = 3;
    //Used for initial centipede generation call. 
    public int startingCentipedeGenerationCount = 12;
    //Incremented everytime all centipedes are destroyed
    int centipedeGenerationCount;
    [SerializeField]
    List<Centipede> centipedeLivingList; //total



    //used for FireLaser()
    public bool laserExists = false;

    //Prefabs loaded in from resources
    public GameObject laser;
    public GameObject mushroom;
    public GameObject mushroomContainer;
    GameObject mushroomContainerPrefab;
    GameObject centipede;
    GameObject flea;
    GameObject spider;
    GameObject scorpion;
    GameObject player;
    public GameObject playerExplosion;
    public GameObject points300;
	public GameObject points600;
	public GameObject points900;
    public GameObject points1000;


	//Required in Unity Scene
	[Header("Audio Manager")]
    public AudioManager am; //Is public so AudioManager can be called by through GameManager.
    public GameObject arena; //Create accessor for this 
    public GameObject movementBoundary; //create accessor for this 
    public float movementBoundaryX; //create accessor for this 
    public float movementBoundaryY; //create accessor for this 
    TextMeshProUGUI scoreboard;
    TextMeshProUGUI highscoreUI;
    GameObject[] playerLivesUI;
    public Camera mainCam;

	// Flea / other enemy related variables
	float fleaTimeMin = 0f;
	float fleaTimeMax = 8f;
	float fleaSpawnTimer;
    float spiderSpawnTimeMin = 2f;
    float spiderSpawnTimeMax = 8f;
    float spiderSpawnTimer;
    float scorpionSpawnTimeMin = 2f;
    float scorpionSpawnTimeMax = 8f;
    float scorpionSpawnTimer;
    public float scorpionMoveSpeed = .15f;
    public float scorpionTopBoundary;
    public float scorpionBottomBoundary;
	public float spiderMoveSpeed = .15f;
    public float spiderTopBoundary;
	public float spiderBottomBoundary;
	public float spiderMoveTimerMin = .25f;
	public float spiderMoveTimerMax = .75f;

	private void Awake()
    {
        LoadPrefabs();
        BuildReferences();
    }

    void LoadPrefabs()
    {
        laser = Resources.Load("Prefabs/Laser") as GameObject;
        mushroom = Resources.Load("Prefabs/Mushroom") as GameObject;
        mushroomContainerPrefab = Resources.Load("Prefabs/MushroomContainer") as GameObject;
		centipede = Resources.Load("Prefabs/Centipede") as GameObject;
        flea = Resources.Load("Prefabs/Flea") as GameObject;
        spider = Resources.Load("Prefabs/Spider") as GameObject;
        scorpion = Resources.Load("Prefabs/Scorpion") as GameObject;
		player = Resources.Load("Prefabs/Player") as GameObject;
        playerExplosion = Resources.Load("Prefabs/PlayerExplosion") as GameObject;
        points300 = Resources.Load("Prefabs/Points300") as GameObject;
		points600 = Resources.Load("Prefabs/Points600") as GameObject;
		points900 = Resources.Load("Prefabs/Points900") as GameObject;
        points1000 = Resources.Load("Prefabs/Points1000") as GameObject;

        SwapPalette(1); // sets the palette to the default colors.

	}

    void BuildReferences()
    {
        am = FindObjectOfType<AudioManager>();
        arena = GameObject.FindWithTag("arena");
        movementBoundary = GameObject.FindWithTag("movement boundary");
        scoreboard = GameObject.FindWithTag("scoreboard").GetComponent<TextMeshProUGUI>();
        highscoreUI = GameObject.FindWithTag("highscore").GetComponent<TextMeshProUGUI>();
        playerLivesUI = new GameObject[5];
        playerLivesUI = GameObject.FindGameObjectsWithTag("lives");
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

    void SwapPalette(int _paletteNumber)
    {
        if (_paletteNumber < 1 || _paletteNumber > 14)
            _paletteNumber = Random.Range(1, 15);
        
        Material mat = Resources.Load("Materials/Palette " + _paletteNumber) as Material;

		mushroom.GetComponent<SpriteRenderer>().material = mat;
        centipede.GetComponent<SpriteRenderer>().material = mat;
        flea.GetComponent<SpriteRenderer>().material = mat;
        spider.GetComponent<SpriteRenderer>().material = mat;
        scorpion.GetComponent<SpriteRenderer>().material = mat;

        foreach (Mushroom mush in FindObjectsOfType<Mushroom>())
        {
            mush.sr.material = mat;
        }

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.sr.material = mat;
        }

        FindObjectOfType<Player>().GetComponent<SpriteRenderer>().material = mat;

    }

    //Sets boundary for player movement
    void Boundary()
    {
        movementBoundaryX = arena.transform.localScale.x;
        movementBoundaryY = arena.transform.localScale.y * movementBoundaryScale;
        movementBoundary.transform.localScale = new Vector3(1, movementBoundaryScale, 1);
        movementBoundary.transform.position = new Vector3(0, 0, 0);

        // Spider Boundaries
        spiderBottomBoundary = 0;
        spiderTopBoundary = arena.transform.localScale.y * .4f;

        // Scorpion Boundaries
        scorpionBottomBoundary = 20;
        scorpionTopBoundary = arena.transform.localScale.y - 1;
    }

    //Builds arena then starts game
    private IEnumerator BuildArena()
    {
		scoreUpdate(0);
		mushroomContainer = Instantiate(mushroomContainerPrefab);

        //for (int i = 4; i <= arena.transform.localScale.y; i++)
        for (int i = (int)arena.transform.localScale.y; i > 3; i--)
        {
            int rowDensity = Random.Range(0, maxRowDensity + 1);
            List<int> density = new List<int>();
            while (density.Count < rowDensity)
            {
                int rnd = Random.Range(1, (int)arena.transform.localScale.x + 1);
                if (!density.Contains(rnd))
                {
                    Instantiate(mushroom, new Vector3(rnd - 1, i - 1, 0), Quaternion.identity, mushroomContainer.transform);
                    density.Add(rnd);
                    yield return new WaitForSeconds(mushroomGenerationTime);
                }
            }
        }
        //am.FadeinBGM("BGM1"); // fade in having trouble on mobile launch.. 
        am.Play("BGM1");
        NewCentipedeWave();        
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
		scoreUpdate(0);

		//Despawn enemies (centipedes)
		for (int i = centipedeLivingList.Count - 1; i >= 0; i--)
        {
            //Prevents crash if centipede dies before being removed from list.
            if (centipedeLivingList[i])
            {
                Destroy(centipedeLivingList[i].transform.gameObject);
                yield return new WaitForSeconds(centipedeDespawnTime);
            }
        }
		centipedeLivingList.Clear();

		// Despawn enemies (everything else)
		Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
		foreach (Enemy enemy in remainingEnemies)
		{
            Destroy(enemy.gameObject);
		}

		// Heal Mushroom Logic
		Mushroom[] munchrooms = FindObjectsOfType<Mushroom>();
        for (int i = 0; i < munchrooms.Length; i++)
        {
            if (munchrooms[i].health < 4 || munchrooms[i].isPoisoned)
            {
				munchrooms[i].HealMushroom();
				yield return new WaitForSeconds(mushroomGenerationTime);
			}
		}


        // Spawn Player Again
        Vector2 instantionPoint = new Vector2(arena.transform.localPosition.x + 15, arena.transform.localPosition.y);
        Instantiate(player, instantionPoint, Quaternion.identity);

        pauseGame = false;
        SpawnEnemies();
    }
    

    void NewCentipedeWave()
    {
		scoreUpdate(0);
		centipedeWave++;
        SpawnEnemies();
    }

	Vector2 GetSpiderInstantiationPoint()
	{
		// Fifty. Fifty. Left or right. 
		float xPos = Random.Range(0, 100) >= 50 ? -2 : movementBoundaryX;
        float yPos = Random.Range(spiderBottomBoundary, spiderTopBoundary);

		return new Vector2(xPos, yPos);
	}

	Vector2 GetScorpionInstantiationPoint()
	{
		// Fifty. Fifty. Left or right. 
		float xPos = Random.Range(0, 100) >= 50 ? -2 : movementBoundaryX;
		float yPos = Random.Range(scorpionBottomBoundary, scorpionTopBoundary);

		return new Vector2(xPos, yPos);
	}
	Vector2 GetEnemyInstantiationPointTop()
    {
		int rand = Random.Range(1, (int)movementBoundaryX);

		return new Vector2(rand, arena.transform.localPosition.y + 39);
	}

    //Instantiaion point needs to be randomized here
    void SpawnEnemies()
    {
        // Sets the centipede length and how many heads based on wave number
        centipedeGenerationCount = startingCentipedeGenerationCount - (centipedeWave / 2);
        int headCount = (centipedeWave / 2);

        // Generates a list of unique spawn points the length of how many centipede heads you will have total (including the main centipede)
		List<Vector2> uniqueSpawnPoints = new List<Vector2>();
		while (uniqueSpawnPoints.Count < headCount + 1)
		{
            Vector2 point = GetEnemyInstantiationPointTop();
			if (!uniqueSpawnPoints.Contains(point))
			{
				uniqueSpawnPoints.Add(point);
			}
		}



        Vector2 instantionPoint = uniqueSpawnPoints[0];
        uniqueSpawnPoints.RemoveAt(0);
		//set nodeAhead/nodeBehind for full length Centipede.
		List<Centipede> centipedeSpawnList = new List<Centipede>();
        for (int i = 0; i < centipedeGenerationCount; i++)
        {
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

        // Add individual head only centipedes
        for (int i = 0; i < headCount; i++)
        {
			instantionPoint = uniqueSpawnPoints[0];
			uniqueSpawnPoints.RemoveAt(0);
			Centipede c = Instantiate(centipede, instantionPoint, Quaternion.identity).GetComponent<Centipede>();
			centipedeLivingList.Add(c);
		}


		// Spawn Fleas
		if (centipedeWave >= 2)
			SpawnFlea();

		SpawnSpider();
        SpawnScorpion();

	}

    public void scoreUpdate(int pts)
    {
        score += pts;
        if(score > highScore)
        {
            highScore = score;
            highScoreReached = true;
        }
        scoreboard.text = "" + score;
        highscoreUI.text = "" + highScore;
        //UI placement is currently hardcoded.
        for(int i = 0; i < 5; i++)
        {
            if (playerLivesUI[i])
            {
                if (int.Parse(playerLivesUI[i].name) <= playerLives)
                {
                    playerLivesUI[i].SetActive(true);
                }
                else
                {
                    playerLivesUI[i].SetActive(false);
                }
            }
        }
    }

    public void DecrementCentipedeList(GameObject c)
    {
        //centipedeLivingList.RemoveAt(0);
        centipedeLivingList.Remove(c.GetComponent<Centipede>());
        if (centipedeLivingList.Count <= 0)
        {
			SwapPalette(0);
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
            scoreUpdate(0);
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
        //Flash Game Over Text? 
        scoreUpdate(0);
        if (highScoreReached)
        {
            //Set High Score
            //Flash High Score - To do
            PlayerPrefs.SetInt("highScore", highScore);
        }

        yield return new WaitForSeconds(gameOverTimer);
        
        //Press Fire To Restart. 
        gameOver = true;
    }

    public void SpawnFlea()
    {
     
        fleaSpawnTimer = Random.Range(fleaTimeMin, fleaTimeMax);
        StopCoroutine("SpawnFleaCoroutine");
        StartCoroutine("SpawnFleaCoroutine");
    }

    private IEnumerator SpawnFleaCoroutine()
    {
		yield return new WaitForSeconds(fleaSpawnTimer);

		Vector2 instantionPoint = GetEnemyInstantiationPointTop();
        Instantiate(flea, instantionPoint, Quaternion.identity);
	}

	public void SpawnSpider()
	{
		spiderSpawnTimer = Random.Range(spiderSpawnTimeMin, spiderSpawnTimeMax);
		StopCoroutine("SpawnSpiderCoroutine");
		StartCoroutine("SpawnSpiderCoroutine");
	}

	private IEnumerator SpawnSpiderCoroutine()
	{
		yield return new WaitForSeconds(spiderSpawnTimer);

		Vector2 instantionPoint = GetSpiderInstantiationPoint();
		Instantiate(spider, instantionPoint, Quaternion.identity);
	}

	public void SpawnScorpion()
	{
		spiderSpawnTimer = Random.Range(scorpionSpawnTimeMin, scorpionSpawnTimeMax);
		StopCoroutine("SpawnScorpionCoroutine");
		StartCoroutine("SpawnScorpionCoroutine");
	}

	private IEnumerator SpawnScorpionCoroutine()
	{
		yield return new WaitForSeconds(spiderSpawnTimer);

		Vector2 instantionPoint = GetScorpionInstantiationPoint();
		Instantiate(scorpion, instantionPoint, Quaternion.identity);
	}
}
