using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private GameObject currentTeleporter;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentTeleporter != null)
            {
                // 디버그: 현재 포탈과 연결된 destination의 이름 출력
                Transform dest = currentTeleporter.GetComponent<Portal>().GetDestination();
                if(dest != null)
                {
                    Debug.Log("Teleporting to: " + dest.name + " / " + dest.position);
                    transform.position = dest.position;
                }
                else
                {
                    Debug.LogWarning("Destination is not assigned!");
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("portal"))
        {
            currentTeleporter = collision.gameObject;
            Debug.Log("Entered portal: " + collision.gameObject.name);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("portal"))
        {
            if(collision.gameObject == currentTeleporter)
            {
                Debug.Log("Exited portal: " + collision.gameObject.name);
                currentTeleporter = null;
            }
        }
    }
}
