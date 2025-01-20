using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SceneChange() {
        SceneManager.LoadScene("01_Multiplay");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
