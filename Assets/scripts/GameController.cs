using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject particlePrefab;
    [SerializeField] Text scoreText, bestScoreText;
    [SerializeField] GameObject gameOverPanel, gamePanel;
    [SerializeField] Text bestScoreTextFromGameOver, scoreTextFromGameOver, gameOverText;
    [SerializeField] AudioClip boom;
    [SerializeField] private GameObject _fields;

    [HideInInspector] public int fieldWidth = 4, fieldHeight = 4;

    private Field[,] fields;
    private bool _gameOver = false;
    private int score, bestScore;
    private AudioSource sound;
    private bool playAudio = false;

    private void OnEnable() {
        SwipeDetection.SwipeEvent += OnSwipe;
        }
    private void Start() {
        sound = GetComponent<AudioSource>();
        fields = new Field[fieldWidth, fieldHeight];
        NewGame();
        playAudio = PlayerPrefs.GetInt("sounds", 1) == 1;
        }
    private void OnDisable() {
        SwipeDetection.SwipeEvent -= OnSwipe;
        }
    private void OnSwipe(SwipeDetection.DIRECTION dir) {
        if (!_gameOver) {
            bool isMove = false;
            if (!isMove && dir == SwipeDetection.DIRECTION.LEFT)
                isMove = MoveLeft();
            else if (!isMove && dir == SwipeDetection.DIRECTION.RIGHT)
                isMove = MoveRight();
            else if (!isMove && dir == SwipeDetection.DIRECTION.UP)
                isMove = MoveUp();
            else if (!isMove && dir == SwipeDetection.DIRECTION.DOWN)
                isMove = MoveDown();
            if (isMove)
                SetField();
            }
        }
    void Update() {
        if (!_gameOver) {
            bool isMove = false;
            if (!isMove && Input.GetKeyDown(KeyCode.A))
                isMove = MoveLeft();
            if (!isMove && Input.GetKeyDown(KeyCode.D))
                isMove = MoveRight();
            if (!isMove && Input.GetKeyDown(KeyCode.W))
                isMove = MoveUp();
            if (!isMove && Input.GetKeyDown(KeyCode.S))
                isMove = MoveDown();
            if (isMove)
                SetField();
            }

        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("GameScene");
        }
    void SetField() {
        var emptyField = new List<int>();
        Field field;

        for (int i = 0; i < fieldHeight; i++)
            for (int j = 0; j < fieldWidth; j++) {
                field = fields[i, j];
                if (field.IsEmpty)
                    emptyField.Add(i * fieldWidth + j);
                }

        int addr = emptyField[Random.Range(0, emptyField.Count)];
        int addrj = addr % fieldWidth;
        int addri = (addr - addrj) / fieldWidth;
        int type = Random.Range(0, 10) == 1 ? 4 : 2;
        fields[addri, addrj].ChangeType(type);
        Sprite temp = GetSprite(type);
        fields[addri, addrj].GetComponent<SpriteRenderer>().sprite = temp;
        fields[addri, addrj].Play();
        bool isCanMove = false;
        if (emptyField.Count <= 1)
            isCanMove = ChekGameOver();
        if (isCanMove)
            GameOver();
        }

    private Sprite GetSprite(int num) {
        if (num == 2)
            return sprites[1];
        else if (num == 4)
            return sprites[2];
        else if (num == 8)
            return sprites[3];
        else if (num == 16)
            return sprites[4];
        else if (num == 32)
            return sprites[5];
        else if (num == 64)
            return sprites[6];
        else if (num == 128)
            return sprites[7];
        else if (num == 256)
            return sprites[8];
        else if (num == 512)
            return sprites[9];
        else if (num == 1024)
            return sprites[10];
        else if (num == 2048)
            return sprites[11];
        return sprites[0];
        }
    private void GetFieldsArray() {
        int i = 0, j = 0;
        foreach (Field obj in _fields.GetComponentsInChildren<Field>()) {
            if (obj.name == "fields")
                continue;
            if (j >= fieldHeight) {
                j = 0;
                i++;
                }
            fields[i, j] = obj;
            fields[i, j].ChangeType(0);
            fields[i, j].GetComponent<SpriteRenderer>().sprite = GetSprite(0);
            j++;
            }
        }
    private bool MoveLeft() {
        bool isMoved = false;
        Field fA, fB;
        for (int i = 0; i < fieldHeight; i++) {
            for (int j = 0; j < fieldWidth; j++) {
                fA = fields[i, j];
                if (fA.IsEmpty)
                    continue;

                for (int k = 0; k < fieldWidth; k++) {
                    if (k <= j)
                        continue;
                    fB = fields[i, k];
                    if (fB.IsEmpty)
                        continue;
                    if (fA.Type != fB.Type)
                        break;
                    if (fA.Type == fB.Type) {
                        SumFields(fA, fB, i, j);
                        isMoved = true;
                        break;
                        }
                    }
                if (j > 0) {
                    bool b = false;
                    Field oldObjB = null;
                    for (int k = j - 1; k >= 0; k--) {
                        fB = fields[i, k];
                        if (fB.IsEmpty)
                            b = true;
                        if (!fB.IsEmpty)
                            break;
                        oldObjB = fields[i, k];
                        }
                    if (b) {
                        MoveFields(fA, oldObjB);
                        isMoved = true;
                        }
                    }
                }
            }
        return isMoved;
        }
    private bool MoveRight() {
        bool isMoved = false;
        Field fA, fB;
        for (int i = 0; i < fieldHeight; i++) {
            for (int j = fieldWidth - 1; j >= 0; j--) {
                fA = fields[i, j];
                if (fA.IsEmpty)
                    continue;
                for (int k = fieldWidth - 1; k >= 0; k--) {
                    if (k >= j)
                        continue;
                    fB = fields[i, k];
                    if (fB.IsEmpty)
                        continue;
                    if (fA.Type != fB.Type)
                        break;
                    if (fA.Type == fB.Type) {
                        SumFields(fA, fB, i, j);
                        isMoved = true;
                        break;
                        }
                    }
                if (j < fieldWidth - 1) {
                    bool b = false;
                    Field oldObjB = null;
                    for (int k = j + 1; k < fieldWidth; k++) {
                        fB = fields[i, k];
                        if (fB.IsEmpty)
                            b = true;
                        if (!fB.IsEmpty)
                            break;
                        oldObjB = fields[i, k];
                        }
                    if (b) {
                        MoveFields(fA, oldObjB);
                        isMoved = true;
                        }
                    }
                }
            }
        return isMoved;
        }
    private bool MoveUp() {
        bool isMoved = false;
        Field fA, fB;
        for (int j = 0; j < fieldHeight; j++) {
            for (int i = 0; i < fieldWidth; i++) {
                fA = fields[i, j];
                if (fA.IsEmpty)
                    continue;
                for (int k = 0; k < fieldWidth; k++) {
                    if (k <= i)
                        continue;
                    fB = fields[k, j];
                    if (fB.IsEmpty)
                        continue;
                    if (fA.Type != fB.Type)
                        break;
                    if (fA.Type == fB.Type) {
                        SumFields(fA, fB, i, j);
                        isMoved = true;
                        break;
                        }
                    }
                if (i > 0) {
                    bool b = false;
                    Field oldObjB = null;
                    for (int k = i - 1; k >= 0; k--) {
                        fB = fields[k, j];
                        if (fB.IsEmpty)
                            b = true;
                        if (!fB.IsEmpty)
                            break;
                        oldObjB = fields[k, j];
                        }
                    if (b) {
                        MoveFields(fA, oldObjB);
                        isMoved = true;
                        }
                    }
                }
            }
        return isMoved;
        }
    private bool MoveDown() {
        bool isMoved = false;
        Field fA, fB;
        for (int j = 0; j < fieldHeight; j++) {
            for (int i = fieldWidth - 1; i >= 0; i--) {
                fA = fields[i, j];
                if (fA.IsEmpty)
                    continue;
                for (int k = fieldWidth - 1; k >= 0; k--) {
                    if (k >= i)
                        continue;
                    fB = fields[k, j];
                    if (fB.IsEmpty)
                        continue;
                    if (fA.Type != fB.Type)
                        break;
                    if (fA.Type == fB.Type) {
                        SumFields(fA, fB, i, j);
                        isMoved = true;
                        break;
                        }
                    }
                if (i < fieldHeight - 1) {
                    bool b = false;
                    Field oldObjB = null;
                    for (int k = i + 1; k < fieldWidth; k++) {
                        fB = fields[k, j];
                        if (fB.IsEmpty)
                            b = true;
                        if (!fB.IsEmpty)
                            break;
                        oldObjB = fields[k, j];
                        }
                    if (b) {
                        MoveFields(fA, oldObjB);
                        isMoved = true;
                        }
                    }
                }
            }
        return isMoved;
        }
    private void SetScore(int i) {
        score += i;
        scoreText.text = "" + score;
        bestScore = PlayerPrefs.GetInt("best", 0);
        if (score >= bestScore) {
            bestScore = score;
            PlayerPrefs.SetInt("best", bestScore);
            }
        bestScoreText.text = "Best: " + bestScore;
        if (i == 2048)
            GameOver(true);
        }
    private void SumFields(Field a, Field b, int i, int j) {
        Instantiate(particlePrefab, fields[i, j].transform.position, Quaternion.identity);
        a.NextType();
        SetScore(a.Type);
        if (playAudio)
            sound.PlayOneShot(boom);
        b.ChangeType(0);
        a.SetSprite(GetSprite(a.Type));
        b.SetSprite(GetSprite(b.Type));
        }
    private void MoveFields(Field a, Field b) {
        b.ChangeType(a.Type);
        a.ChangeType(0);
        a.SetSprite(GetSprite(a.Type));
        b.SetSprite(GetSprite(b.Type));
        }
    private bool ChekGameOver() {
        for (int i = 0; i < fieldHeight; i++)
            for (int j = 0; j < fieldWidth; j++) {
                int typeField = fields[i, j].Type;
                if (j > 0)
                    if (CanWalk(fields[i, j - 1], typeField))
                        return false;
                if (j < fieldWidth - 1)
                    if (CanWalk(fields[i, j + 1], typeField))
                        return false;
                if (i > 0)
                    if (CanWalk(fields[i - 1, j], typeField))
                        return false;
                if (i < fieldHeight - 1)
                    if (CanWalk(fields[i + 1, j], typeField))
                        return false;
                }
        return true;
        }
    private bool CanWalk(Field obj, int type) {
        if (obj.Type == type)
            return true;
        return false;
        }
    private void GameOver(bool winner = false) {
        if (winner)
            gameOverText.text = "You Win!";
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        bestScoreTextFromGameOver.text = "Best Score: " + bestScore;
        scoreTextFromGameOver.text = "Your Score: " + score;
        _gameOver = true;
        }
    public void NewGame() {
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);
        ReloadGame();
        _gameOver = false;
        }
    private void ReloadGame() {
        GetFieldsArray();
        SetField();
        score = 0;
        SetScore(0);
        }
    }