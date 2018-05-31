using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalFlee : MonoBehaviour {

    public Vector2 distanceRun;
    public float runSpeed;
    private Vector2 startPosition;


	// Use this for initialization
	void Start () {
        startPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (ChaseTrigger.chase == true)
        {
            //as soon as the chase starts, run away
            if (Mathf.Abs(startPosition.x - transform.position.x) < distanceRun.x) transform.Translate(Vector2.right * (runSpeed * Time.deltaTime));
            //once far enough, go far down and out of the way, and stop running
            else if (transform.position.y == startPosition.y) transform.Translate(Vector2.down * 50);
        }
        //if the chase stops a.k.a. the player loses, go back in position
        else if (transform.position.x != startPosition.x) transform.position = startPosition;

	}
}
