using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Controller2D target;
    public Vector2 focusareasize;
    public float lookaheaddistX;
    public float looksmoothtimeX;
    public float vertsmoothtime;
    public float verticaloffset;

    focusarea camfocusarea;

    float currentlookaheadX;
    float targetlookaheadX;
    float lookaheaddirX;
    float smoothlookvelocityX;
    float smoothvelocityY;

    bool lookaheadstopped;

    void Start()
    {
        camfocusarea = new focusarea(target.collider.bounds, focusareasize);
    }

    void LateUpdate()
    {
        camfocusarea.Update(target.collider.bounds);

        Vector2 focusposition = camfocusarea.center + Vector2.up * verticaloffset;

        if(camfocusarea.velocity.x != 0)
        {
            lookaheaddirX = Mathf.Sign(camfocusarea.velocity.x);
            if(Mathf.Sign(target.input.x) == Mathf.Sign(camfocusarea.velocity.x) && target.input.x != 0)
            {
                lookaheadstopped = false;
                targetlookaheadX = lookaheaddirX * lookaheaddistX;
            }
            else
            {
                if (!lookaheadstopped)
                {
                    lookaheadstopped = true;
                    targetlookaheadX = currentlookaheadX + (lookaheaddirX * lookaheaddistX - currentlookaheadX) / 4f;
                }
            }
        }

        targetlookaheadX = lookaheaddirX * lookaheaddistX;
        currentlookaheadX = Mathf.SmoothDamp(currentlookaheadX, targetlookaheadX, ref smoothlookvelocityX, looksmoothtimeX);

        focusposition.y = Mathf.SmoothDamp(transform.position.y, focusposition.y, ref smoothvelocityY, vertsmoothtime);
        focusposition += Vector2.right * currentlookaheadX;

        transform.position = (Vector3)focusposition + Vector3.forward * -10;
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(camfocusarea.center, focusareasize);
    }

    struct focusarea {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;
        
        public focusarea(Bounds targetbounds, Vector2 size) : this(){ 

            left = targetbounds.center.x - size.x / 2;
            right = targetbounds.center.x + size.x / 2;
            bottom = targetbounds.min.y;
            top = targetbounds.min.y + size.y;

            velocity = Vector2.zero;

            center = new Vector2 (((left + right)/2), ((top + bottom) / 2));
        }

        public void Update (Bounds targetbounds)
        {
            float shiftX = 0;
            if (targetbounds.min.x < left)
            {
                shiftX = targetbounds.min.x - left;
            }
            else if (targetbounds.max.x > right)
            {
                shiftX = targetbounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetbounds.min.y < bottom)
            {
                shiftY = targetbounds.min.y - bottom;
            }
            else if (targetbounds.max.y > top)
            {
                shiftY = targetbounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
