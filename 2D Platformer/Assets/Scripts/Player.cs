using UnityEngine;
using System.Collections;


[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    public float jumpheight = 4;
    public float timetojump = .4f;
    public float movespeed = 6;
    float airacceltime = .2f;
    float groundacceltime = .03f;

    float gravity;
    float jumpvelocity;
    float velocityXsmoothing;

    Vector3 velocity;

    Controller2D controller;
	void Start () {
        controller = GetComponent<Controller2D>();

        gravity = -(2* jumpheight)/ Mathf.Pow(timetojump, 2);
        jumpvelocity = Mathf.Abs(gravity) * timetojump;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpvelocity);
	}

    void Update()
    {
        if(controller.collisioninfo.above || controller.collisioninfo.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetKeyDown (KeyCode.D) && controller.collisioninfo.below)
        {
            velocity.y = jumpvelocity;
        }

        float targetVelocityX = input.x * movespeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXsmoothing, (controller.collisioninfo.below)?groundacceltime:airacceltime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
