using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {
    [SerializeField] private Sprite[] soundImg;
    [SerializeField] private Image soundButton;
    private AsyncOperation newGameOperation;

    private void Start() {
        newGameOperation = SceneManager.LoadSceneAsync("GameScene");
        newGameOperation.allowSceneActivation = false;
        SetSound();
        }
    public void NewGame() => newGameOperation.allowSceneActivation = true;
    public void SoundsOnOff() {
        PlayerPrefs.SetInt("sounds", PlayerPrefs.GetInt("sounds", 1) == 0 ? 1 : 0);
        SetSound();
        }
    public void ShowInfo() => SceneManager.LoadScene("InfoScene");
    public void ExitGame() => Application.Quit();
    private void SetSound() => soundButton.sprite = soundImg[PlayerPrefs.GetInt("sounds", 1)];
    }