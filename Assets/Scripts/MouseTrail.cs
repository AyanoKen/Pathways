using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrail : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Distance from the camera
        transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
