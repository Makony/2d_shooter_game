using UnityEngine;

public class AttachCamera : MonoBehaviour
{
    public Transform player; 
    public float smoothSpeed = 5f;      // How fast the camera moves to follow
    private Camera cam;
    
    // Zoom params
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 20f;

    void Start()
    {
        cam = Camera.main;
        cam.orthographicSize = 13f;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Immediately snap camera onto the player
        if (player != null)
        {
            Vector3 snapPos = player.position;
            snapPos.z = transform.position.z;
            transform.position = snapPos;
        }
    }

    void LateUpdate()
    {
        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);

        // Follow
        if (player != null)
        {
            Vector3 targetPos = player.position;
            targetPos.z = transform.position.z; 
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        }
    }
}
