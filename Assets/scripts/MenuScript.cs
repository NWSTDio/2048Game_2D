using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
    {
    [SerializeField] private Sprite[] soundImg;// картинки кнопки вкл выкл звука
    [SerializeField] private Image soundButton;// кнопка звука
    private AsyncOperation newGameOperation;// для фоновой загрузки сцены

    private void Start()
        {
        newGameOperation = SceneManager.LoadSceneAsync("GameScene");// загрузим в фоне сцену
        newGameOperation.allowSceneActivation = false;// запретим ее открытие
        SetSound();// установка картинки звука
        }
    public void NewGame() => newGameOperation.allowSceneActivation = true;// откроем сцену игры
    public void SoundsOnOff() // переключатель звука
        {
        PlayerPrefs.SetInt("sounds", PlayerPrefs.GetInt("sounds", 1) == 0 ? 1 : 0);// переключатель звука
        SetSound();// установка картинки звука
        }
    public void ShowInfo() => SceneManager.LoadScene("InfoScene");// открыть сцену инфо
    public void ExitGame() => Application.Quit();// выход из игры
    private void SetSound() => soundButton.sprite = soundImg[PlayerPrefs.GetInt("sounds", 1)];// установка картинки звука
    }