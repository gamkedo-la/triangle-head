using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndCollider : MonoBehaviour
{

    public string levelName;

    // This method is called when a collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider is the player
        if (other.CompareTag("Player"))
        {
            // Load the "Level1Complete" scene
            SceneManager.LoadScene(levelName);
        }
    }
}