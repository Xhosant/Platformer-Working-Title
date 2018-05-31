using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTrigger : MonoBehaviour {

    public static bool chase = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            chase = true;
        }
    }
}
