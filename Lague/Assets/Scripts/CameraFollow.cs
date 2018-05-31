using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Controller2D target;
    public Vector2 focusAreaSize;

    public float verticalOffset;
    public float lookAheadDistanceX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirectionX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    FocusArea focusArea;

    void Start()
    {
        //set the 'follow box' of the player. Movement in it doesn't move the camera
        focusArea = new FocusArea(target.GetComponent<Collider2D>().bounds, focusAreaSize);
    }

    private void LateUpdate()
    {
        focusArea.Update(target.GetComponent<Collider2D>().bounds);

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        //foxus in front of the player
        if (focusArea.velocity.x != 0)
        {
            lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);
        }

        //be gentle on the camera movements
        targetLookAheadX = lookAheadDistanceX * lookAheadDirectionX;
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);


        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -18;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    //defining the focus area
    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        //the meat and potatoes of the camera following
        public void Update (Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
