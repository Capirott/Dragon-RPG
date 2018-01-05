﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMe : MonoBehaviour {

	[SerializeField] float xRotationsPerMinute = 1f;
	[SerializeField] float yRotationsPerMinute = 1f;
	[SerializeField] float zRotationsPerMinute = 1f;
	
	void Update () {
        float xDegreesPerFrame = Time.deltaTime * 6 * xRotationsPerMinute;
        transform.RotateAround (transform.position, transform.right, xDegreesPerFrame);

		float yDegreesPerFrame = Time.deltaTime * 6 * yRotationsPerMinute;
        transform.RotateAround (transform.position, transform.up, yDegreesPerFrame);

        float zDegreesPerFrame = Time.deltaTime * 6 * zRotationsPerMinute;
        transform.RotateAround (transform.position, transform.forward, zDegreesPerFrame);
	}
}
