using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {
    public float speed;
	public Vector2 nextPosition;
	public string color;

	void Start () {
        speed = 0;
		nextPosition = Vector2.zero;
		color = null;
	}
	
	void Update () {
	}
}
