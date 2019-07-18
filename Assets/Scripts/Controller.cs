//-----------------------------------------------
// This script is used to control color buttons'
// visibility by activating and deactivating them 
// according to the user input.
//-----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {

	}

	// Update is called once per frame
	void Update () 
    {
		
	}
    // Below methods are activating and deactivating the game object which the script
    // is attached to.
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void DeActivate()
    {
        gameObject.SetActive(false);
    }
}
