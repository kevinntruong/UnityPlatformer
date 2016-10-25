using UnityEngine;
using System.Collections;


[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    public float maxjumpheight = 5;
    public float minjumpheight = 1;
    public float timetojump = .4f;
    public float movespeed = 6;

    public float maxwallslidespeed = 3;
    public float timetostick = .05f;
    public float timetounstick;

    public Vector2 climbingwalljump;
    public Vector2 offwalljump;
    public Vector2 wallleap;

    float airacceltime = .2f;
    float groundacceltime = .1f;

    float gravity;
    float maxjumpvelocity;
    float minjumpvelocity;
    float velocityXsmoothing;

    Vector3 velocity;

    public Vector2 input;

    Controller2D controller;
	void Start () {
        controller = GetComponent<Controller2D>();

        gravity = -(2* maxjumpheight)/ Mathf.Pow(timetojump, 2);
        maxjumpvelocity = Mathf.Abs(gravity) * timetojump;
        minjumpvelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minjumpheight);
        print("Gravity: " + gravity + " Jump Velocity: " + maxjumpvelocity);
	}

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int walldirX = (controller.collisioninfo.left) ? -1 : 1;

        
        float targetVelocityX = input.x * movespeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXsmoothing, (controller.collisioninfo.below)?groundacceltime:airacceltime);

        bool wallsliding = false;
        if((controller.collisioninfo.left || controller.collisioninfo.right) && !controller.collisioninfo.below && velocity.y < 0){
            wallsliding = true;

            if(velocity.y < -maxwallslidespeed)
            {
                velocity.y = -maxwallslidespeed;
            }
        }
        if(timetounstick > 0)
        {
            if(input.x != walldirX && input.x != 0)
            {
                timetounstick -= Time.deltaTime;
            }
            else
            {
                timetounstick = timetostick;
            }
            
        }
            
        if(controller.collisioninfo.above || controller.collisioninfo.below)
        {
            velocity.y = 0;
        }

        if(Input.GetKeyDown (KeyCode.D))
        {
            if (wallsliding)
            {
                if(walldirX == input.x)
                {
                    velocity.x = -walldirX * climbingwalljump.x;
                    velocity.y = climbingwalljump.y;
                }
                else if(input.x == 0)
                {
                    velocity.x = -walldirX * offwalljump.x;
                    velocity.y = offwalljump.y;
                }
                else
                {
                    velocity.x = -walldirX * wallleap.x;
                    velocity.y = wallleap.y;
                }
            }
            if (controller.collisioninfo.below)
            {
                velocity.y = maxjumpvelocity;
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if(velocity.y > minjumpvelocity)
            {
                velocity.y = minjumpvelocity;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
