using UnityEngine;

public class Rotater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]private float rotationSpeed = 1f;
    private void LateUpdate()
    {
        transform.Rotate(Vector3.up, rotationSpeed*Time.deltaTime, Space.World);
    }
}
