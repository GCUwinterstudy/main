using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class movingStep_14 : MonoBehaviour
{
    public float speed = 3.0f;
    public float distance = 13.0f;

    private float startPositionX;
    private float rightPositionX;
    private float leftPositionX;

    private bool movingLeft = true;
    // Start is called before the first frame update
    void Start()
    {
        startPositionX = transform.position.x;
        rightPositionX = startPositionX;// Initialize right position
        leftPositionX = startPositionX - distance;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftPositionX)
            {
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            }
            else
            {
                movingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightPositionX) // Move towards the left position
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
            else
            {
                movingLeft = true;
            }
        }
    }
}
