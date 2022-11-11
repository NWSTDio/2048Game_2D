using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
    {
    [SerializeField] Sprite[] sprites;// изображения ячеек
    [SerializeField] GameObject particlePrefab;// частица разрушения клетки
    [SerializeField] Text scoreText, bestScoreText;// текстовые метки на экране для очков
    [SerializeField] GameObject gameOverPanel, gamePanel;// панели с кнопками
    [SerializeField] Text bestScoreTextFromGameOver, scoreTextFromGameOver, gameOverText;
    [SerializeField] AudioClip boom;// звук оазрушения клетки
    [SerializeField] private GameObject _fields;
    
    [HideInInspector] public int fieldWidth = 4, fieldHeight = 4;// размеры поля (ячейки)
    
    private Field[,] fields;// массив ячеек
    private bool _gameOver = false;// проверка состояния игры
    private int score, bestScore;// хранение очков в игре
    private AudioSource sound;// компонент для воспроизведения звуков
    private bool playAudio = false;// можно ли проигрывать звук?

    private void OnEnable()
        {
        SwipeDetection.SwipeEvent += OnSwipe;// добавим слушателя свайпов
        }
    private void Start()
        {
        sound = GetComponent<AudioSource>();// для работы с музыкой
        fields = new Field[fieldWidth, fieldHeight];// поле клеток
        NewGame();// начнем игру
        playAudio = PlayerPrefs.GetInt("sounds", 1) == 1;// проигрывать ли звуки
        }
	private void OnDisable() // при разрушении сцены
		{
        SwipeDetection.SwipeEvent -= OnSwipe;// сбросим слушателя, иначе ошибка
		}
	private void OnSwipe(SwipeDetection.DIRECTION dir)
        {
        if (!_gameOver) // если игра не окончена
            {
            bool isMove = false;// проверка было ли какоето движение
            // осуществим ход в указанном направлении
            if (!isMove && dir == SwipeDetection.DIRECTION.LEFT)
                isMove = MoveLeft();
            else if (!isMove && dir == SwipeDetection.DIRECTION.RIGHT)
                isMove = MoveRight();
            else if (!isMove && dir == SwipeDetection.DIRECTION.UP)
                isMove = MoveUp();
            else if (!isMove && dir == SwipeDetection.DIRECTION.DOWN)
                isMove = MoveDown();
            if (isMove) // если да
                SetField();// добавим цифру на поле, и проверим есть ли пустые клетки на поле
            }
        }
    void Update()
        {
        if(!_gameOver) // если игра не окончена
            {
            bool isMove = false;// проверка было ли какоето движение
            // осуществим ход в указанном направлении
            if (!isMove && Input.GetKeyDown(KeyCode.A))
                isMove = MoveLeft();
            if (!isMove && Input.GetKeyDown(KeyCode.D))
                isMove = MoveRight();
            if (!isMove && Input.GetKeyDown(KeyCode.W))
                isMove = MoveUp();
            if (!isMove && Input.GetKeyDown(KeyCode.S))
                isMove = MoveDown();
            if (isMove) // если да
                SetField();// добавим цифру на поле, и проверим есть ли пустые клетки на поле
            }
        // сбросс сцены
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("GameScene");
        }
    void SetField()
        {
        var emptyField = new List<int>();// адреса (зашифрованные) всех пустых клеток на поле
        Field field;// для получения компонента у обьекта ячейки
        // пойдемся по массиву ячеек
        for (int i = 0; i < fieldHeight; i++)
            for (int j = 0; j < fieldWidth; j++)
                {
                field = fields[i, j];// получим ссылку на скрипт
                if (field.IsEmpty) // если ячейка пустая
                    emptyField.Add(i * fieldWidth + j);// добавим в список адрес массива в линейном виде (зашифрованном)
                }
        // рандомно получим линейный адрес ячецки
        int addr = emptyField[Random.Range(0, emptyField.Count)];
        // переведем его в столбци и строки
        int addrj = addr % fieldWidth;
        int addri = (addr - addrj) / fieldWidth;
        // получим тип ячейки
        int type = Random.Range(0, 10) == 1 ? 4 : 2;
        fields[addri, addrj].ChangeType(type);// установим тип
        Sprite temp = GetSprite(type);// получим спрайт для ячейки
        fields[addri, addrj].GetComponent<SpriteRenderer>().sprite = temp;// установим спрайт
        fields[addri, addrj].Play();// проиграем анимацию
        bool isCanMove = false;// проверка возможен ли следующи ход
        if (emptyField.Count <= 1) // если это последняя свободная ячейка
            isCanMove = ChekGameOver();// проверим возможен ли ход
        if (isCanMove) // если нет свободных ячеек
            GameOver();// конец игры
        }
    // получение спрайта по типу ячейки
    private Sprite GetSprite(int num)
        {
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
    // получим ячейки в виде массива
    private void GetFieldsArray()
        {
        int i = 0, j = 0;// столбци и строки
        foreach (Field obj in _fields.GetComponentsInChildren<Field>()) // получим дрчерние элементы
            {
            if (obj.name == "fields") // если обьект поле
                continue;
            if (j >= fieldHeight) // если вышли за пределы столбцов
                {
                j = 0;// сброс столбцов
                i++;// переход на след строку
                }
            fields[i, j] = obj;// получение обьекта
            fields[i, j].ChangeType(0);// установка пустого типа
            fields[i, j].GetComponent<SpriteRenderer>().sprite = GetSprite(0);// установка спрайта
            j++;
            }
        }
    private bool MoveLeft() // движение в лево
        {
        bool isMoved = false;// было ли движение
        Field fA, fB;// скрипты сравниваемых ячеек
        // проходим по полю ячеек
        for (int i = 0; i < fieldHeight; i++) // высота
            {
            for (int j = 0; j < fieldWidth; j++) // ширина
                {
                fA = fields[i, j];// скрипт для первого обьекта
                if (fA.IsEmpty) // если обьект пуст
                    continue;
                // проходим обратно по строке, чтобы сравнить
                for (int k = 0; k < fieldWidth; k++)
                    {
                    if (k <= j) // если итератор меньше или равен тому что у обьекта
                        continue;// следю итерация
                    fB = fields[i, k];// получим скрипт второго обьекта
                    if (fB.IsEmpty) // если ячейка пуста
                        continue;
                    if (fA.Type != fB.Type) // или ячейка с несовпадающим типом
                        break;
                    if (fA.Type == fB.Type) // если ячейки равны
                        {
                        SumFields(fA, fB, i, j);// суммируем ячейки
                        isMoved = true;
                        break;
                        }
                    }
                if (j > 0) // если ячейка не с краю
                    {
                    bool b = false;// проверка передвижения отключена
                    Field oldObjB = null;// для хранения предыдущего обьекта ячейки
                    for (int k = j - 1; k >= 0; k--) // пройдем обратно по рядку
                        {
                        fB = fields[i, k];// получим код обьекта
                        if (fB.IsEmpty) // если ячейка пустая
                            b = true;// можно перемещать
                        if (!fB.IsEmpty) // если ячейка на пути занята
                            break;// остановим цикл
                        oldObjB = fields[i, k];// сохраним лбьект
                        }
                    if (b) // если можно перемещатся
                        {
                        MoveFields(fA, oldObjB);// двигаем ячейку
                        isMoved = true;// укажем что был шаг
                        }
                    }
                }
            }
        return isMoved;// вернем результат, был шаг или нет
        }
    private bool MoveRight() // движение в право
        {
        bool isMoved = false;// было ли движение
        Field fA, fB;// скрипты сравниваемых ячеек
        // проходим по полю ячеек
        for (int i = 0; i < fieldHeight; i++) // высота
            {
            for (int j = fieldWidth - 1; j >= 0; j--) // ширина
                {
                fA = fields[i, j];// скрипт для первого обьекта
                if (fA.IsEmpty) // если обьект пуст
                    continue;
                // проходим обратно по строке, чтобы сравнить
                for (int k = fieldWidth - 1; k >= 0; k--)
                    {
                    if (k >= j) // если итератор меньше или равен тому что у обьекта
                        continue;// следю итерация
                    fB = fields[i, k];// получим скрипт второго обьекта
                    if (fB.IsEmpty) // если ячейка пуста
                        continue;
                    if (fA.Type != fB.Type) // или ячейка с несовпадающим типом
                        break;
                    if (fA.Type == fB.Type) // если ячейки равны
                        {
                        SumFields(fA, fB, i, j);// сумируем ячейку
                        isMoved = true;// укажем что был шаг
                        break;// прервем цикл
                        }
                    }
                if (j < fieldWidth - 1) // если ячейка не с краю
                    {
                    bool b = false;// проверка передвижения отключена
                    Field oldObjB = null;// для хранения предыдущего обьекта ячейки
                    for (int k = j + 1; k < fieldWidth; k++) // пройдем обратно по рядку
                        {
                        fB = fields[i, k];// получим код обьекта
                        if (fB.IsEmpty) // если ячейка пустая
                            b = true;// можно перемещать
                        if (!fB.IsEmpty) // если ячейка на пути занята
                            break;// остановим цикл
                        oldObjB = fields[i, k];// сохраним лбьект
                        }
                    if (b) // если можно перемещатся
                        {
                        MoveFields(fA, oldObjB);// двигаем ячейку
                        isMoved = true;// укажем что был шаг
                        }
                    }
                }
            }
        return isMoved;// вернем результат, был шаг или нет
        }
    private bool MoveUp() // движение вверх
        {
        bool isMoved = false;// было ли движение
        Field fA, fB;// скрипты сравниваемых ячеек
        // проходим по полю ячеек
        for (int j = 0; j < fieldHeight; j++) // высота
            {
            for (int i = 0; i < fieldWidth; i++) // ширина
                {
                fA = fields[i, j];// скрипт для первого обьекта
                if (fA.IsEmpty) // если обьект пуст
                    continue;
                // проходим обратно по строке, чтобы сравнить
                for (int k = 0; k < fieldWidth; k++)
                    {
                    if (k <= i) // если итератор меньше или равен тому что у обьекта
                        continue;// следю итерация
                    fB = fields[k, j];// получим скрипт второго обьекта
                    if (fB.IsEmpty) // если ячейка пуста
                        continue;
                    if (fA.Type != fB.Type) // или ячейка с несовпадающим типом
                        break;
                    if (fA.Type == fB.Type) // если ячейки равны
                        {
                        SumFields(fA, fB, i, j);// суммируем ячейку
                        isMoved = true;// укажем что был шаг
                        break;// прервем цикл
                        }
                    }
                if (i > 0) // если ячейка не с краю
                    {
                    bool b = false;// проверка передвижения отключена
                    Field oldObjB = null;// для хранения предыдущего обьекта ячейки
                    for (int k = i - 1; k >= 0; k--) // пройдем обратно по рядку
                        {
                        fB = fields[k, j];// получим код обьекта
                        if (fB.IsEmpty) // если ячейка пустая
                            b = true;// можно перемещать
                        if (!fB.IsEmpty) // если ячейка на пути занята
                            break;// остановим цикл
                        oldObjB = fields[k, j];// сохраним лбьект
                        }
                    if (b) // если можно перемещатся
                        {
                        MoveFields(fA, oldObjB);// двигаем ячейку
                        isMoved = true;// укажем что был шаг
                        }
                    }
                }
            }
        return isMoved;// вернем результат, был шаг или нет
        }
    private bool MoveDown() // движение в низ
        {
        bool isMoved = false;// было ли движение
        Field fA, fB;// скрипты сравниваемых ячеек
        // проходим по полю ячеек
        for (int j = 0; j < fieldHeight; j++) // высота
            {
            for (int i = fieldWidth - 1; i >= 0; i--) // ширина
                {
                fA = fields[i, j];// скрипт для первого обьекта
                if (fA.IsEmpty) // если обьект пуст
                    continue;
                // проходим обратно по строке, чтобы сравнить
                for (int k = fieldWidth - 1; k >= 0; k--)
                    {
                    if (k >= i) // если итератор меньше или равен тому что у обьекта
                        continue;// следю итерация
                    fB = fields[k, j];// получим скрипт второго обьекта
                    if (fB.IsEmpty) // если ячейка пуста
                        continue;
                    if (fA.Type != fB.Type) // или ячейка с несовпадающим типом
                        break;
                    if (fA.Type == fB.Type) // если ячейки равны
                        {
                        SumFields(fA, fB, i, j);// суммируем ячйку
                        isMoved = true;// укажем что был шаг
                        break;// прервем цикл
                        }
                    }
                if (i < fieldHeight - 1) // если ячейка не с краю
                    {
                    bool b = false;// проверка передвижения отключена
                    Field oldObjB = null;// для хранения предыдущего обьекта ячейки
                    for (int k = i + 1; k < fieldWidth; k++) // пройдем обратно по рядку
                        {
                        fB = fields[k, j];// получим код обьекта
                        if (fB.IsEmpty) // если ячейка пустая
                            b = true;// можно перемещать
                        if (!fB.IsEmpty) // если ячейка на пути занята
                            break;// остановим цикл
                        oldObjB = fields[k, j];// сохраним лбьект
                        }
                    if (b) // если можно перемещатся
                        {
                        MoveFields(fA, oldObjB);// двигаем ячейку
                        isMoved = true;// укажем что был шаг
                        }
                    }
                }
            }
        return isMoved;// вернем результат, был шаг или нет
        }
    // функция добавления ячейки вручную
    private void SetDebugField(int i, int j, int type)
        {
        fields[i, j].ChangeType(type);// установим тип
        Sprite temp = GetSprite(type);// получим спрайт для ячейки
        fields[i, j].GetComponent<SpriteRenderer>().sprite = temp;// установим спрайт
        }
    private void SetScore(int i)
        {
        score += i;
        scoreText.text = "" + score;
        bestScore = PlayerPrefs.GetInt("best", 0);
        if (score >= bestScore)
            {
            bestScore = score;
            PlayerPrefs.SetInt("best", bestScore);
            }
        bestScoreText.text = "Best: " + bestScore;
        if (i == 2048)
            GameOver(true);
        }
    private void SumFields(Field a, Field b, int i, int j)
        {
        Instantiate(particlePrefab, fields[i, j].transform.position, Quaternion.identity);
        a.NextType();// увеличим тип ячейки в двое
        SetScore(a.Type);
        if (playAudio) // если можно проирывать звук
            sound.PlayOneShot(boom);// проиграем его
        b.ChangeType(0);// сбросим ячейку
        // обновим спрайты
        a.SetSprite(GetSprite(a.Type));
        b.SetSprite(GetSprite(b.Type));
        }
    private void MoveFields(Field a, Field b)
        {
        b.ChangeType(a.Type);// получим тип ячейки
        a.ChangeType(0);
        // обновим спрайти
        a.SetSprite(GetSprite(a.Type));
        b.SetSprite(GetSprite(b.Type));
        }
    private bool ChekGameOver()
        {
        for (int i = 0; i < fieldHeight; i++)
            for (int j = 0; j < fieldWidth; j++)
                {
                int typeField = fields[i, j].Type;
                // проверим всех соседей вокруг клетки
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
    private bool CanWalk(Field obj, int type)
        {
        if (obj.Type == type) // если клетки одинаковы
            return true;
        return false;
        }
    private void GameOver(bool winner = false) // конец игры
        {
        if (winner)
            gameOverText.text = "You Win!";
        gamePanel.SetActive(false);// убираем игрвую панель
        gameOverPanel.SetActive(true);// показываем панель конца игры
        // показываем полученные очки
        bestScoreTextFromGameOver.text = "Best Score: " + bestScore;
        scoreTextFromGameOver.text = "Your Score: " + score;
        _gameOver = true;// установим значение конца игры
		}
    public void NewGame() // новая игра
        {
        gameOverPanel.SetActive(false);// убираем панель конца игры
        gamePanel.SetActive(true);// показываем игровую панель
        ReloadGame();// перегрузим или загрузим основные данные игры
        _gameOver = false;// запустим игру
        }
    private void ReloadGame() // перегрузка или загрузка основных даных игры
        {
        GetFieldsArray();// получим поле с ячейками
        SetField();// установим рандомно ячейку
        score = 0;// обнулим очки
        SetScore(0);// перезапустим текстовые поля с очками
        }
    }