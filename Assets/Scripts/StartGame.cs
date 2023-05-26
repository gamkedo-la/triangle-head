using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) {
            SceneManager.LoadScene("level 1");
        } else if (Input.GetKey(KeyCode.Alpha2)) {
            SceneManager.LoadScene("level 2");
        }
    }
}
