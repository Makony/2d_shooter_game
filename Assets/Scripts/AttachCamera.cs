using UnityEngine;

public class AttachCamera : MonoBehaviour
{
    public Transform player; 
    public float smoothSpeed = 5f; //Higher values means faster rotation
    private Vector2 offset;


    //30.04 by A.: to zoom in/out while playing
    private Camera cam;
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        cam.orthographicSize = 10f;    //03.05, to set the camzoom to this value

        offset = (Vector2)(transform.position - player.position);
    }

    // Update is called once per frame
    void Update()
    {
        //30.04 by A.: use scroll to zoom out /in
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);



        Vector3 newPosition = player.position + (Vector3)offset;
        newPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
    }
}
