using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public GameObject Player;
    public Vector3 Down = new Vector3(0,5,0);
    public Vector3 Up = new Vector3(0,10,0);
    public float Speed = 1f;
    private bool flip = false;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(Down, Up, (Mathf.Sin(Speed * Time.time) + 1f)/2f);
    }
}
