using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Boundary();

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
}
