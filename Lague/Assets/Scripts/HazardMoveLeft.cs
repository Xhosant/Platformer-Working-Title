using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardMoveLeft : MonoBehaviour {

    public bool triggered = false;
    private bool done = false;
    private Vector2 startPosition;
    public float speed;
    public float distance;
    public BoxCollider2D trigger;


	// Use this for initialization
	void Start () {
        startPosition = transform.position;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //When the player enters the trapped area, and assuming the hazard hasn't finished its movement, start it.
        if (done==false && collision.tag == "Player") { triggered = true; }
        //The trigger colliders were causing trouble for the player's collision detection
        //Since they were meant to be single-use, it was much simpler to despawn them than build them as separate objects on the IgnoreRayCast layer, so I did.
        Destroy(trigger);
    }

    // Update is called once per frame
    void Update () {
        //Unless you've covered your predetermines distance...
        if (Mathf.Abs(startPosition.x - transform.position.x) >= distance) { triggered = false; done = true; }
        //...move backwards.
        if (triggered == true) { transform.Translate(Vector2.down * speed * Time.deltaTime); }
	}
}
