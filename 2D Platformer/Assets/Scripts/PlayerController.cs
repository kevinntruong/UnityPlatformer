using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float movespeed;
    public int jumpheight;
    public float slidespeed;

    public Transform groundpoint;
    public Transform wallpoint1;
    public Transform wallpoint2;
    public float radius;

    public LayerMask groundmask;
    public LayerMask wallmask;

    bool grounded;
    bool wallslide = false;
    bool rwalled;
    bool lwalled;
    Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {

        //Booleans for Collision Detection
        grounded = Physics2D.OverlapCircle(groundpoint.position, radius, groundmask);
        rwalled = Physics2D.OverlapCircle(wallpoint1.position, radius, wallmask);
        lwalled = Physics2D.OverlapCircle(wallpoint2.position, radius, wallmask);

        //Player Controls
        if (Input.GetKey(KeyCode.LeftArrow)) //Left
        {
            rb2d.velocity = new Vector2(-movespeed, rb2d.velocity.y);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetKey(KeyCode.RightArrow)) //Right
        {
            rb2d.velocity = new Vector2(movespeed, rb2d.velocity.y);
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (Input.GetKeyDown(KeyCode.D) && grounded) //Jumping
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpheight);
        }
        /*if() //Wall Slide
        {
            rb2d.velocity = new Vector2(-2, rb2d.velocity.y);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && lwalled && !grounded)
        {
            rb2d.velocity = new Vector2(-2, rb2d.velocity.y);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {

        }*/
	}

    void OnDrawGizmos() //Visual Markers for Collisions
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundpoint.position, radius);
        Gizmos.DrawWireSphere(wallpoint1.position, radius);
        Gizmos.DrawWireSphere(wallpoint2.position, radius);
    }
}
