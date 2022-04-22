using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private void Update()
    {
        var xInput = Input.GetAxis("Horizontal");
        var newRotation = Quaternion.Euler(0f, -xInput * rotationSpeed * Time.deltaTime, 0f);
        
        transform.rotation *= newRotation;
    }
}
