using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveMessages : MonoBehaviour {
    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
       rb =  this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {


		
	}

    public void Push(Vector2 direction)
    {
        rb.AddForce(direction);
    }
}
