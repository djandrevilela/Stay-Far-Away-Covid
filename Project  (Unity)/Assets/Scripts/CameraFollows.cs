﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

public class CameraFollows : MonoBehaviour
{

    public GameObject targetObject;
    private float distanceToTarget;

    // Start is called before the first frame update
    void Start()
    {
        distanceToTarget = transform.position.x - targetObject.transform.position.x;
        
    }

    // Update is called once per frame
    void Update()
    {
        float targetObjectX = targetObject.transform.position.x;
        Vector3 newCameraPosition = transform.position;
        newCameraPosition.x = targetObjectX + distanceToTarget;
        transform.position = newCameraPosition;
    }
}
