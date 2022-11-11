using UnityEngine;

/*
    
    класс для работы со свайпами
    подлючаем его к пустому обьекту в игре
    в основном скрипте игры подключаем событие так
    SwipeDetection += ИМЯ
    и метод
    private void ИМЯ(SwipeDetection.DIRECTION dir)
    !!! если перегрузит сцену будет ошибка
    исправление, в коде основного скрипта
    private void OnDestroy()
		{
        SwipeDetection.SwipeEvent -= ИМЯ;
		}

*/


public class SwipeDetection : MonoBehaviour
    {
    public enum DIRECTION { LEFT, RIGHT, UP, DOWN } // направление свайпа
    public static event OnSwipeInput SwipeEvent;
    public delegate void OnSwipeInput(SwipeDetection.DIRECTION dir);

    private Vector2 tapPosition, swipeDelta;// позиция свайпа и дельта свайпа
    private float deadZone = 80f;// расстояние когда будет считатся что свайп
    private bool isSwipping;// происходит ли свайп
    private bool isMobile;// для проверки на мобильном ли устройстве игра
    void Start()
        {
        isMobile = Application.isMobilePlatform;// проверяем не на телефоне ли
        }
    void Update()
        {
        if(!isMobile) // если на компе
            {
            if (Input.GetMouseButtonDown(0)) // полуим нажатие, но не отпускание
                {
                isSwipping = true;// начался свайп
                tapPosition = Input.mousePosition;// заполним позицию касания
                }
            else if(Input.GetMouseButtonUp(0)) // если убрали палец
                ResetSwipe();// сбросим свайп
	    	}
        else // если на телефоне
            {
            if(Input.touchCount > 0) // если касание
                {
                if(Input.GetTouch(0).phase == TouchPhase.Began) // полуим нажатие, но не отпускание
                    {
                    isSwipping = true;// начался свайп
                    tapPosition = Input.GetTouch(0).position;// заполним позицию касания
                    }
                else if (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended) // если убрали палец
                    ResetSwipe();// сбросим свайп
                }
			}
        checkSwipe();// проверка свайпа
        }
    private void checkSwipe() // проверка свайпа
        {
        swipeDelta = Vector2.zero;// сбросим дельту свайпа
        if(isSwipping) // если свайпаем
            {
            if (!isMobile && Input.GetMouseButton(0)) // если с компа и держим кнопку мыщи
                swipeDelta = (Vector2) Input.mousePosition - tapPosition;// получим новую дельта свайпа
            else if (Input.touchCount > 0) // если с телефона и не убрали палец
                swipeDelta = Input.GetTouch(0).position - tapPosition;// получим новую дельта свайпа
            }
        if(swipeDelta.magnitude > deadZone) // если дельта больше указаной зоны свайпа (размера или длины
            {
            if(SwipeEvent != null) // если свайпы подключены
                {
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y)) // свайп по оси X
                    SwipeEvent(swipeDelta.x > 0 ? DIRECTION.RIGHT : DIRECTION.LEFT);// отправим в любой скрипт который подключился
                else  // свайп по оси Y
                    SwipeEvent(swipeDelta.y > 0 ? DIRECTION.UP : DIRECTION.DOWN);// отправим в любой скрипт который подключился
                }
            ResetSwipe();// сбросим свайп
			}
        }
    private void ResetSwipe() // сбросс свайпа
        {
        isSwipping = false;// сброс свайпа
        // сброс значений
        tapPosition = Vector2.zero;// позиции
        swipeDelta = Vector2.zero;// жедьты
		}
    }