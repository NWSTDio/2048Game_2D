using UnityEngine;

public class SwipeDetection : MonoBehaviour {

    public enum DIRECTION { LEFT, RIGHT, UP, DOWN }
    public static event OnSwipeInput SwipeEvent;
    public delegate void OnSwipeInput(DIRECTION dir);

    private Vector2 tapPosition, swipeDelta;
    private float deadZone = 80f;
    private bool isSwipping;
    private bool isMobile;
    void Start() {
        isMobile = Application.isMobilePlatform;
        }
    void Update() {
        if (!isMobile) {
            if (Input.GetMouseButtonDown(0)) {
                isSwipping = true;
                tapPosition = Input.mousePosition;
                }
            else if (Input.GetMouseButtonUp(0))
                ResetSwipe();
            }
        else {
            if (Input.touchCount > 0) {
                if (Input.GetTouch(0).phase == TouchPhase.Began) {
                    isSwipping = true;
                    tapPosition = Input.GetTouch(0).position;
                    }
                else if (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended)
                    ResetSwipe();
                }
            }
        checkSwipe();
        }
    private void checkSwipe() {
        swipeDelta = Vector2.zero;
        if (isSwipping) {
            if (!isMobile && Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - tapPosition;
            else if (Input.touchCount > 0)
                swipeDelta = Input.GetTouch(0).position - tapPosition;
            }
        if (swipeDelta.magnitude > deadZone) {
            if (SwipeEvent != null) {
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    SwipeEvent(swipeDelta.x > 0 ? DIRECTION.RIGHT : DIRECTION.LEFT);
                else
                    SwipeEvent(swipeDelta.y > 0 ? DIRECTION.UP : DIRECTION.DOWN);
                }
            ResetSwipe();
            }
        }
    private void ResetSwipe() {
        isSwipping = false;
        tapPosition = Vector2.zero;
        swipeDelta = Vector2.zero;
        }

    }