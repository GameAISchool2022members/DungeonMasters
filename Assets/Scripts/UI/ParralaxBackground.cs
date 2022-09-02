using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{

	private float length, startPos;
	public GameObject cam;
	public float parallexEffect;
	// sets the default position of the background and original x coord of the sprites
	void Start()
	{
		startPos = transform.position.x;
		length = GetComponent<SpriteRenderer>().bounds.size.x;
	}
	//modifies speed at wish the background images move allowing for independant layers to move at different speeds
	void Update()
	{
		float temp = (cam.transform.position.x * (1 - parallexEffect));
		float dist = (cam.transform.position.x * parallexEffect);

		transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

		if (temp > startPos + length) startPos += length;
		else if (temp < startPos - length) startPos -= length;
	}

}
