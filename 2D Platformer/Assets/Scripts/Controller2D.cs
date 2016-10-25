using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : Player {

    public LayerMask collisionmask;                     //Defines what layer of rigid bodies can be collided with.

    const float skinwidth = .015f;                      //Defines the thickness within the player for the raycast origin
    public int horizontalrays = 4;                      //Defines number of horizontal rays
    public int verticalrays = 4;                        //Defines number of veritcal rays

    float maxclimbingangle = 60;                        //Defines the max climbable slope
    float maxdescendangle = 60;

    float horizontalrayspace;                           //Defines how wide the space is between horizontal raycast origins
    float verticalrayspace;                             //Defines how wide the space is between vertical raycast origins

    public BoxCollider2D collider;                             //Defines instance of 2D box collider.

    struct RaycastOrigins                               //Struct used to define locations of raycast origins
    {
        public Vector2 topleft, topright;
        public Vector2 bottomleft, bottomright;
    }

    RaycastOrigins raycastorigins;

    public struct collision                             //Struct used to detect which collisions are being made
    {
        public bool above, below;
        public bool left, right;
        public int facedir;

        public bool climbingslope;
        public bool descendingslope;
        public float slopeangle, oldslopeangle;
        public Vector3 velocityold;

        public void reset()
        {
            above = below = false;
            left = right = false;
            climbingslope = false;
            descendingslope = false;

            oldslopeangle = slopeangle;
            slopeangle = 0;
        }
    }

    public collision collisioninfo;                     

    void Awake()                                        //Upon startup
    {
        collider = GetComponent<BoxCollider2D>();
        collisioninfo.facedir = 1;
    }

    public virtual void Start()
    {
        calcrayspacing();
    }

    public void Move(Vector3 velocity)
    {
        updateraycastorigins();
        collisioninfo.reset();
        collisioninfo.velocityold = velocity;

        if (velocity.x != 0)
        {
            collisioninfo.facedir = (int)Mathf.Sign(velocity.x);
        }
        if (velocity.y < 0)
        {
            descendslope(ref velocity);
        }

        horizontalcollision(ref velocity);

        if(velocity.y != 0)
        {
            verticalcollision(ref velocity);
        }
        transform.Translate(velocity);
    }

    void horizontalcollision(ref Vector3 velocity)   //Detects horizontal collisions through raycasting
    {
        float directionX = collisioninfo.facedir;
        float raylength = Mathf.Abs(velocity.x) + skinwidth;

        if(Mathf.Abs(velocity.x) < skinwidth)
        {
            raylength = 2 * skinwidth;
        }

        for (int i = 0; i < horizontalrays; i++)
        {
            Vector2 rayorigin = (directionX == -1) ? raycastorigins.bottomleft : raycastorigins.bottomright;
            rayorigin += Vector2.up * (horizontalrayspace * i);

            RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.right * directionX, raylength, collisionmask);

            Debug.DrawRay(rayorigin, Vector2.right * directionX * raylength, Color.red);

            if (hit)
            {
                float slopeangle = Vector2.Angle(hit.normal, Vector2.up);

                if(i == 0 && slopeangle <= maxclimbingangle)
                {
                    if (collisioninfo.descendingslope)
                    {
                        collisioninfo.descendingslope = false;
                        velocity = collisioninfo.velocityold;
                    }
                    climbslope(ref velocity, slopeangle);
                    float distancetoslope = 0;

                    if(slopeangle != collisioninfo.oldslopeangle)
                    {
                        distancetoslope = hit.distance - skinwidth;
                        velocity.x -= distancetoslope * directionX;
                    }

                    climbslope(ref velocity, slopeangle);
                    velocity.x += distancetoslope * directionX;
                }
                if(!collisioninfo.climbingslope || slopeangle > maxclimbingangle)
                {
                    velocity.x = (hit.distance - skinwidth) * directionX;
                    raylength = hit.distance;

                    if (collisioninfo.climbingslope)
                    {
                        velocity.y = Mathf.Tan(collisioninfo.slopeangle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x); 
                    }

                    collisioninfo.left = directionX == -1;
                    collisioninfo.right = directionX == 1;
                }
            }
        }
    }

    void verticalcollision(ref Vector3 velocity) //Detects vertical collisions through raycasting
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

                if (collisioninfo.climbingslope)
                {
                    velocity.x = velocity.y/Mathf.Tan(collisioninfo.slopeangle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisioninfo.below = directionY == -1;
                collisioninfo.above = directionY == 1;
            }
        }

        if (collisioninfo.climbingslope)
        {
            float directionX = Mathf.Sign(velocity.x);
            raylength = Mathf.Abs(velocity.x + skinwidth);
            Vector2 rayorigin = ((directionX == -1) ? raycastorigins.bottomleft : raycastorigins.bottomright) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.right * directionX, raylength, collisionmask);

            if (hit)
            {
                float slopeangle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeangle != collisioninfo.slopeangle)
                {
                    velocity.x = (hit.distance - skinwidth) * directionX;
                    collisioninfo.slopeangle = slopeangle;
                }
            }
        }
    }
    
    void climbslope(ref Vector3 velocity, float slopeangle)
    {
        float movedistance = Mathf.Abs(velocity.x);
        float climbvelocityY = Mathf.Sin(slopeangle * Mathf.Deg2Rad) * movedistance;
        if (velocity.y <= climbvelocityY)
        {
            velocity.y = climbvelocityY;
            velocity.x = Mathf.Cos(slopeangle * Mathf.Deg2Rad) * movedistance * Mathf.Sign(velocity.x);
            collisioninfo.below = true;
            collisioninfo.climbingslope = true;
            collisioninfo.slopeangle = slopeangle;

        }
    }

    void descendslope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastorigins.bottomright : raycastorigins.bottomleft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionmask);

        if (hit)
        {
            float slopeangle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeangle != 0 && slopeangle <= maxdescendangle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinwidth <= Mathf.Tan(slopeangle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float movedistance = Mathf.Abs(velocity.x);
                        float descendvelocityY = Mathf.Sin(slopeangle * Mathf.Deg2Rad) * movedistance;
                        velocity.x = Mathf.Cos(slopeangle * Mathf.Deg2Rad) * movedistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendvelocityY;

                        collisioninfo.slopeangle = slopeangle;
                        collisioninfo.descendingslope = true;
                        collisioninfo.below = true;
                    }
                }
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
}
