using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]

public class Controller2D : MonoBehaviour {



    public LayerMask collisionMask;

    const float skinWidth = .015f;
    public int horizontalRayCount = 20;
    public int verticalRayCount = 20;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;
    Player player;

	void Start () {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }


 
    public void Move(Vector3 velocity)
    {
        //handle speed when going to ram yourself in a wall (and not phasing, but that doesn't happen here)
        collisions.Reset();
        UpdateRaycastOrigins();

        if (velocity.x != 0) {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            //handle the where and how of raycast collision detection
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            //an extra colision detection to make sure your back end ('tail') has made it out of the wall with you. Nothing's as awkward as getting your tail stuck in a wall.
            //1 should be the player's x scale, but everything blows up if I try to access it. I quit!
            RaycastHit2D tailHit = Physics2D.Raycast(rayOrigin, Vector2.right * -directionX, 1, collisionMask);

            //some debugging niceties, showing the shape and location of raycasts
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            Debug.DrawRay(rayOrigin, Vector2.right * -directionX * 1, Color.green);

            if (hit)
            {
                if (Player.phase == false)
                {
                    //if a wall's detected and we're not phasing, memorize the distance to the wall
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
                //if a wall IS detected but we're phasing, we're successfully in a wall as far as the X axis is concerned. Pass that on for Player's ContinuePhase
                else { Player.continuePhaseCheckX = true;}
            }
            //if the hitting has stopped, and our tail's in the clear too, X no longer spots collisions
            else if (!tailHit){ Player.continuePhaseCheckX = false; }
            
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        
        float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                //handle the where and how of raycast collision detection
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            //no need for a tail check here: we're not phasing downwards ever, while standard colliders take care of upwards

            //some debugging niceties, showing the shape and location of raycasts
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

                if (hit)
                {
                    if (Player.phase == false || velocity.y < 0)
                    {
                        //if a wall's detected and we're not phasing, memorize the distance to the wall. We're not planning to phase downwards, so if you're moving downwards, act as if not phasing
                        velocity.y = (hit.distance - skinWidth) * directionY;
                        rayLength = hit.distance;

                        collisions.below = directionY == -1;
                        collisions.above = directionY == 1;
                    }
                    //if hit but phasing upwards, pass that on for Player's ContinuePhase. If we don't have any upward velocity, don't, lest we get stuck skidding along the floor (X will handle horizontal staright movement)
                    else if (velocity.y >=0.00001f) { Player.continuePhaseCheckY = true; }
                }
                //if not in/near a wall, we're done phasing
                else { Player.continuePhaseCheckY = false; }
        }
    }
  

    void UpdateRaycastOrigins()
    {
        //keep checking collisions where relevant, not some random location
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        //get the rays nice and spread out
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing  = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
       //handle the wheres of rayctasts
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
	
    public struct CollisionInfo
    {
        //handle the wheres of collisions
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = left = right = false;
        }
    }
}
