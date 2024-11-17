using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;

    [SerializeField] float resetPositionY = -600f;
    [SerializeField] float startPositionY = 600f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.localPosition.y <= resetPositionY)
        {
            Vector3 newPos = transform.localPosition;
            newPos.y = startPositionY;
            transform.localPosition = newPos;
        }
    }
}
