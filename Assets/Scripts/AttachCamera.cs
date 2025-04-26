using UnityEngine;

public class AttachCamera : MonoBehaviour
{
    public Transform player; 
    public float smoothSpeed = 5f;
    private Vector2 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = (Vector2)(transform.position - player.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = player.position + (Vector3)offset;
        newPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
    }
}
