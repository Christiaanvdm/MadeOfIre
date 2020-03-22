using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 originalOffset;
    private Vector3 offset;
    public GameObject player;
    private float offsetMagnitude = 2f;
    // Start is called before the first frame update
    void Start()
    {
        originalOffset = transform.position - player.transform.position;
        offset = originalOffset;
    }

    private void FixedUpdate()
    {
        var mousePos = Input.mousePosition;
        float x = mousePos.x / Screen.width;
        float y = mousePos.y / Screen.height;
        x = (x - 0.5f) * offsetMagnitude;
        y = (y - 0.5f) * offsetMagnitude;
        offset.x = originalOffset.x + x;
        offset.y = originalOffset.y + y;
       
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = player.transform.position + offset;
    }
}
