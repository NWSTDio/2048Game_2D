using UnityEngine;

// проигрыватель звука

public class PlaySound : MonoBehaviour
    {
    private AudioSource source;
    private void Start()
        {
        if (PlayerPrefs.GetInt("sounds", 1) == 1) // если можно проигрывать звук
            {
            source = GetComponent<AudioSource>();
            source.Play();// проиграть звук
            }
        }
    }