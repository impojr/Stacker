using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraRotator : MonoBehaviour
{
    public float RotationSpeed = 0.01f;

    void Start()
    {
        var rotX = Random.Range(0f, 360f);
        var rotY = Random.Range(0f, 360f);
        var rotZ = Random.Range(0f, 360f);

        transform.rotation = new Quaternion(rotX, rotY, rotZ, 0);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * (RotationSpeed * Time.deltaTime));
    }

}
