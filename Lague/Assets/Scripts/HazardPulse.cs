using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardPulse : MonoBehaviour {

    private Vector2 startPosition;
    public float speed;
    public float distance;
    public float offset;

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
    }
	
	// A trivial offset of the position via Sine, to let the object 'pulse' up and down with relative ease.
	void Update () {
        transform.position = startPosition + new Vector2(0.0f, distance * Mathf.Sin(speed * Time.time + offset));
    }
}
