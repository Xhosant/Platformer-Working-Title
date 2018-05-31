using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardChase : MonoBehaviour {

    public Vector2 distanceRun;
    public float runSpeed;
    private Vector2 startPosition;
    public Vector2 readyPosition;
    Collider2D target;
    public Vector2 maxDistance;
    public GameObject player;


    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
        player = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update()
    {

        //Had to do a little dance to get it to 'spawn' behind the player.
        if (ChaseTrigger.chase == false )
        {
            //if any amount of chasing has happened at the time of respawn, go to the hidden start position (directly below the Ready position)
            if (transform.position.x != startPosition.x || transform.position.y != startPosition.y) transform.position = startPosition;
        }
        else if (transform.position.y < readyPosition.y) { transform.position = readyPosition; } //if the chase has begun but you are below the ready position (therefore, at the start position), get to the Ready position
        else
        {
            //If the chase is on and you're in position, chase the player. If the player is too far ahead, chase at double speed, just to keep things interesting
            if (transform.position.x < (player.transform.position.x - maxDistance.x)) transform.Translate(Vector2.down * 2 * runSpeed * Time.deltaTime);
            else transform.Translate(Vector2.down * runSpeed * Time.deltaTime);
        }


    }
}
