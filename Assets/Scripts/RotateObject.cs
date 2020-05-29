using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    /// <summary>
    /// Скорость вращения объекта.
    /// </summary>
    [SerializeField]
    private float rotationSpeed;

    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");

        Quaternion newRotation = Quaternion.Euler(0f, -xInput * rotationSpeed, 0f);

        transform.rotation *= newRotation;
    }
}
