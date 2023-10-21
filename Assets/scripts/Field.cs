using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Field : MonoBehaviour {

    private readonly float _maxScale = 1.0f;
    private readonly float _speed = 1f;
    private SpriteRenderer _sprite;
    private int _type = 0;
    private bool _playAnimation = false;
    private float _scale = 1.0f;

    public int Type => _type;
    public bool IsEmpty => _type == 0;

    private void Awake() {
        _sprite = GetComponent<SpriteRenderer>();
        }
    private void Update() {
        if (_playAnimation) {
            _scale += Time.deltaTime * _speed;
            if (_scale >= _maxScale) {
                _scale = 1.0f;
                _playAnimation = false;
                }
            transform.localScale = new Vector3(_scale, _scale, transform.localScale.z);
            }
        }

    public void ChangeType(int i) => _type = i;
    public void NextType() {
        if (_type <= 1024)
            _type *= 2;
        }
    public void SetSprite(Sprite sprite) => _sprite.sprite = sprite;
    public void Play() {
        _scale = .8f;
        transform.localScale = new Vector3(_scale, _scale, transform.localScale.z);
        _playAnimation = true;
        }
    }