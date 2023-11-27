using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    bool hasKey = false;
    public string NextLevel;
    public float timeLeft = 60;
    public AudioMixer mixer;

    public GameObject pausePanel;
    public TMP_Text timeText;
    public TMP_Text livesText;
    public GameObject bloodPanel;


    AudioSource src;
    public AudioClip pickupkey;
    public AudioClip extraLife;
    public AudioClip dieSound;
    public AudioClip panelSound;
    public AudioClip clockSound;
    

    float volume_master = 0;
    float volume_music = 0;
    float volume_sfx = 0;
    float volume_ambient = 0;

    bool mute = false;
    bool pause = false;

    void SaveSettings()
    {
        mixer.GetFloat("volume_master", out volume_master);
        mixer.GetFloat("volume_music", out volume_music);
        mixer.GetFloat("volume_sfx", out volume_sfx);
        mixer.GetFloat("volume_ambient", out volume_ambient);
    }

    void RestoreSettings()
    {
        mixer.SetFloat("volume_master", volume_master);
        mixer.SetFloat("volume_music", volume_music);
        mixer.SetFloat("volume_sfx", volume_sfx);
        mixer.SetFloat("volume_ambient", volume_ambient);
    }


    // Start is called before the first frame update
    void Start()
    {
        SaveSettings();
        src = GetComponent<AudioSource>();
        livesText.text = "Lives : " + GameData.lives.ToString();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (mute == false)
            {
                mute = true;
                mixer.SetFloat("volume_master", -80);
            }
            else
            {
                mute = false;
                RestoreSettings();
            }
        }
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause == false)
            {
                pausePanel.SetActive(true);
                pause = true;
                Time.timeScale = 0;

                mixer.SetFloat("volume_music", -50);
                mixer.SetFloat("volume_ambient", -80);
                mixer.SetFloat("volume_sfx", -80);
            }
            else
            {
                pause = false;
                pausePanel.SetActive(false);
                RestoreSettings();
                Time.timeScale = 1;
            }

        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
        timeText.text = "Time: " + timeLeft.ToString("0.0");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(other.gameObject);
            src.PlayOneShot(pickupkey);
        }
        else if (other.CompareTag("ExtraLife"))
        {
            Destroy(other.gameObject);
            src.PlayOneShot(extraLife);
            GameData.lives += 1;
            livesText.text = "Lives : " + GameData.lives.ToString();
        }

        else if (other.CompareTag("clock"))
        {
            Destroy(other.gameObject);
            src.PlayOneShot(clockSound);
            timeLeft += 15.0f;
            timeText.text = "Time: " + timeLeft.ToString("0.0");
        }
        else if (other.CompareTag("Portal"))
        {
            if (hasKey)
            {
                LoadNextLevel();
            }
        }

        else if (other.CompareTag("AI"))
        {
            Die();
        }


    }

    void Die()
    {
        GameData.lives--;
        livesText.text = "Lives : " + GameData.lives.ToString();
        src.PlayOneShot(dieSound);
        if (GameData.lives == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
        else
        {
            StartCoroutine(ShowBloodPanel());
        }
    }

    void LoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(NextLevel);
        hasKey = false;
    }

    IEnumerator ShowBloodPanel()
    {
        bloodPanel.SetActive(true); // Activate the blood panel
        src.PlayOneShot(panelSound);
        yield return new WaitForSeconds(2.0f); // Wait for the specified duration
        bloodPanel.SetActive(false); // Deactivate the blood panel
    }

}
