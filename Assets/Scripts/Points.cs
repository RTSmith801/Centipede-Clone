using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    //Points will destroy itself in 2 seconds
    void Start()
    {
        Destroy(gameObject, 1f);
    }
}
