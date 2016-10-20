using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    public LayerMask collisionmask;

    const float skinwidth = .015f;
    public int horizontalrays = 4;
    public int verticalrays = 4;

    float horizontalrayspace;
    float verticalrayspace;

    BoxCollider2D collider;
    RaycastOrigins raycastorigins;
    public collision collisioninfo;

    public struct collision
    {
        public bool above, below;
        public bool left, right;

        public void reset()
        {
            above = below = false;
            left = right = false;
        }
    }

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        calcrayspacing();
    }

    public void Move(Vector3 velocity)
    {
        updateraycastorigins();
        collisioninfo.reset();

        if(velocity.x != 0)
        {
            horizontalcollision(ref velocity);
        }
        if(velocity.y != 0)
        {
            verticalcollision(ref velocity);
        }

        transform.Translate(velocity);
    }
    void horizontalcollision(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float raylength = Mathf.Abs(velocity.x) + skinwidth;

        for (int i = 0; i < horizontalrays; i++)
        {
            Vector2 rayorigin = (directionX == -1) ? raycastorigins.bottomleft : raycastorigins.bottomright;
            rayorigin += Vector2.up * (horizontalrayspace * i);

            RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.right * directionX, raylength, collisionmask);

            Debug.DrawRay(rayorigin, Vector2.right * directionX * raylength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinwidth) * directionX;
                raylength = hit.distance;

                collisioninfo.left = directionX == -1;
                collisioninfo.right = directionX == 1;
            }
        }
    }
    void verticalcollision(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float raylength = Mathf.Abs(velocity.y) + skinwidth;

        for (int i = 0; i < verticalrays; i++)
        {
            Vector2 rayorigin = (directionY == -1) ? raycastorigins.bottomleft : raycastorigins.topleft;
            rayorigin += Vector2.right *(verticalrayspace * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.up * directionY, raylength, collisionmask);

            Debug.DrawRay(rayorigin, Vector2.up * directionY * raylength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinwidth) * directionY;
                raylength = hit.distance;

                collisioninfo.below = directionY == -1;
                collisioninfo.above = directionY == 1;
            }
        }
    }

    void updateraycastorigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinwidth * -2);

        raycastorigins.bottomleft = new Vector2(bounds.min.x, bounds.min.y);
        raycastorigins.bottomright = new Vector2(bounds.max.x, bounds.min.y);
        raycastorigins.topleft = new Vector2(bounds.min.x, bounds.max.y);
        raycastorigins.topright = new Vector2(bounds.max.x, bounds.max.y);
    }

    void calcrayspacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinwidth * -2);

        horizontalrays = Mathf.Clamp(horizontalrays, 2,int.MaxValue);
        verticalrays = Mathf.Clamp(verticalrays, 2, int.MaxValue);

        horizontalrayspace = bounds.size.y / (horizontalrays - 1);
        verticalrayspace = bounds.size.x / (verticalrays - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topleft, topright;
        public Vector2 bottomleft, bottomright;
    }
}
