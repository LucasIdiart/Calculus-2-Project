using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

	Vector3 wanted_position;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    	wanted_position = Camera.main.ScreenToWorldPoint(CustomPointer.pointerPosition);
    	wanted_position.z = 0; // Manténlo en el mismo plano que el juego
    	transform.position = wanted_position;
	}

}
