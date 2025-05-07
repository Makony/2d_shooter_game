using UnityEngine;

public class AttachCamera : MonoBehaviour
{
    /*A: removed "offset" because we want the camera to follow the player anyway. So an offset isn't really needed
     * However it can be used later so I didn't remove it completly
     */


    public Transform player; 
    public float smoothSpeed = 5f; //Higher values means faster rotation
    //private Vector2 offset;


    //variables for zoom feature
    private Camera cam;
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 20f;

    void Start()
    {
        cam = Camera.main;
        cam.orthographicSize = 13f;    //03.05, to set the camzoom to this value
        
        //offset = (Vector2)(transform.position - player.position);
    }

    void Update()
    {
        //A.: use scroll to zoom out /in
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        if (player != null) {
            Vector3 newPosition = player.position; //+ (Vector3)offset; if we use offset later. Just remove //
            newPosition.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
