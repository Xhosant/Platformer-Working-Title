using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {


    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne= .2f;
    float accelerationTimeGrounded= .1f;
    float moveSpeed = 6;
    public static bool phase = false;
    public static bool continuePhaseCheckX = false;
    public static bool continuePhaseCheckY = false;
    public bool continuePhase = false;
    bool lastPhaseInput = false;
    public float startTimer;
    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    float phaseTimer = 0f;
    Vector2 lastVelocity;
    Vector3 velocity;
    private Vector2 spawn;

    

    Controller2D controller;

	void Start () {
        //save starting point for respawn purposes
        controller = GetComponent<Controller2D> ();
        spawn = transform.position;

        //calculate gravity and jump force from the much more intuitive 'time to apex' and 'jump heigh'
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity:" + gravity + " Jump velocity" + jumpVelocity);
	}
	

	void Update () {

        //on using the restart button, go back to spawn
        if (Input.GetButton("Restart")) transform.position = spawn;

        //don't store up velocity while resting on the ground
        if ((controller.collisions.above || controller.collisions.below))
        {
            velocity.y = 0;
        }
        //if you're on the ground, press jump, and are not currently phasing (which demands no controls), get jumpin'
        if ((Input.GetButtonDown("Jump") && controller.collisions.below) && (phase == false))
        {
            velocity.y = jumpVelocity;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //to avoid storing a deltatime-quantum of gravity, at the start of phasing, counteract it. The constant in the end makes sure that the innevitable innacuracies are minimal and upward, which is much less of a problem than a tiny downward error
        if (phase==false && Input.GetButtonDown("Phase")) lastVelocity.y = lastVelocity.y - gravity * Time.deltaTime * 0.99999999999f;
        //we don't want people flying via phasing, so unless you find a wall, you only get a little time of 'free' phasing. This line ensures all that's true.
        if (Input.GetButtonDown("Phase") && phaseTimer == 0f && !lastPhaseInput) { phaseTimer = startTimer; phase = true; }
        //make sure you're in a wall. More on that in Controller2
        continuePhase = (continuePhaseCheckX || continuePhaseCheckY);
        //count down till the grace period expires
        if (phaseTimer > 0) phaseTimer -= Time.deltaTime;
        //eradicate float inmperfections
        else if (phaseTimer < 0) phaseTimer = 0f;
        //once successfully in a wall, we don't need the grace period anymore
        if (continuePhase) phaseTimer=0f;
        //if neither in a wall nor during grace period, or if with minimal velocity (which would lead to getting stuck), stop phasing
        if ((!continuePhase && phaseTimer == 0) || (Mathf.Abs(velocity.x) < 1f && Mathf.Abs(velocity.y) < 1f)) phase=false;
        //memorize whether we just started phasing or not
        lastPhaseInput = Input.GetButtonDown("Phase");




        if (phase == false)
        {
            //standard horizontal input control. Only firing during not-phasing, obviously. More responsive on the ground.
            float targetVelocityX = input.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocity.y += gravity * Time.deltaTime;
        }
        //now, if you ARE phasing, just keep going
        else velocity = lastVelocity;
        //and feed the results to the controller
        controller.Move(velocity * Time.deltaTime);
        //memorize previous velocity in case of phasing
        lastVelocity = velocity;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //handle victories and defeats
        if (collision.transform.tag == "Goal") { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single); }
        else if (collision.transform.tag == "Hazard") { transform.position = spawn; }
    }
}
