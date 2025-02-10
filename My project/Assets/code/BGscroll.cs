using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroll : MonoBehaviour
{
    #region Inspector

    public Renderer renderer;
    public float speed = 1f;

    #endregion

    private void Update()
    {
        float move = Time.deltaTime * speed;
        renderer.material.mainTextureOffset += Vector2.right * move;
    }
}