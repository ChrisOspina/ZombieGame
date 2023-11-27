using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown || Input.GetButtonDown("Jump"))
        {
            GameData.lives = 3;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
        }
    }
}