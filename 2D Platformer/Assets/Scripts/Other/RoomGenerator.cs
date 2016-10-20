using UnityEngine;
using System.Collections;

public class RoomGenerator : MonoBehaviour {

    public Transform[] doorpoints;
    public GameObject room;
    public int numofrooms;


    private float roomwidth;
    private float roomheight;

	// Use this for initialization
	void Start () {
        roomwidth = room.GetComponent<BoxCollider2D>().size.x;
        roomheight = room.GetComponent<BoxCollider2D>().size.y;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnDrawGizmos() //Visual Markers for Collisions
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < doorpoints.Length; i++)
        {
            Gizmos.DrawWireCube(doorpoints[i].position, new Vector3(1,1,1));
        }
    }
}
