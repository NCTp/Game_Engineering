using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_a : MonoBehaviour
{
    public bool isAlive;

    void Awake()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        isAlive = true;
    }
}
