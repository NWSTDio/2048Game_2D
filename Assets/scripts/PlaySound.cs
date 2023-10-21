using UnityEngine;

public class PlaySound : MonoBehaviour {

    private AudioSource source;
    private void Start() {
        if (PlayerPrefs.GetInt("sounds", 1) == 1) {
            source = GetComponent<AudioSource>();
            source.Play();
            }
        }

    }