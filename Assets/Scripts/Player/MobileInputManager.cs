using UnityEngine;
using TravesiaColombia.Player;

namespace TravesiaColombia.Player
{
    /// <summary>
    /// Control táctil para móvil — sin botones visuales.
    /// Zona izquierda inferior → deslizar = moverse
    /// Zona derecha inferior   → tocar = saltar
    /// </summary>
    public class MobileInputController : MonoBehaviour
    {
        [Header("Zonas táctiles (píxeles desde abajo)")]
        [SerializeField] private float _zoneHeight = 300f;
        [SerializeField] private float _deadZone = 30f;

        [Header("Debug")]
        [SerializeField] private bool _showDebugZones = true;

        private InputManager _inputManager;
        private int _moveTouchId = -1;
        private int _jumpTouchId = -1;
        private float _moveStartX;
        private float _currentMoveInput;

        private void Start()
        {
            _inputManager = InputManager.Instance;
            Input.multiTouchEnabled = true;
        }

        private void Update()
        {
            if (_inputManager == null)
            {
                _inputManager = InputManager.Instance;
                return;
            }

            // Solo procesar táctil si hay toques activos
            if (Input.touchCount > 0)
            {
                ProcessTouches();
                _inputManager.SimulateMove(new Vector2(_currentMoveInput, 0f));
            }
            else
            {
                // Sin toques — resetear estado táctil
                if (_moveTouchId != -1)
                {
                    _moveTouchId = -1;
                    _currentMoveInput = 0f;
                    _inputManager.SimulateMove(Vector2.zero);
                }
            }
        }

        private void ProcessTouches()
        {
            foreach (Touch touch in Input.touches)
            {
                Vector2 pos = touch.position;
                bool isLeft = IsLeftZone(pos);
                bool isRight = IsRightZone(pos);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (isLeft && _moveTouchId == -1)
                        {
                            _moveTouchId = touch.fingerId;
                            _moveStartX = pos.x;
                        }
                        else if (isRight && _jumpTouchId == -1)
                        {
                            _jumpTouchId = touch.fingerId;
                            _inputManager.SimulateJump();
                        }
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (touch.fingerId == _moveTouchId)
                        {
                            float delta = pos.x - _moveStartX;
                            _currentMoveInput = Mathf.Abs(delta) > _deadZone
                                ? Mathf.Sign(delta) : 0f;
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == _moveTouchId)
                        {
                            _moveTouchId = -1;
                            _currentMoveInput = 0f;
                        }
                        if (touch.fingerId == _jumpTouchId)
                            _jumpTouchId = -1;
                        break;
                }
            }
        }

        private bool IsLeftZone(Vector2 p) => p.x < Screen.width * 0.5f && p.y < _zoneHeight;
        private bool IsRightZone(Vector2 p) => p.x >= Screen.width * 0.5f && p.y < _zoneHeight;

        private void OnGUI()
        {
            if (!_showDebugZones) return;
            GUI.color = new Color(0f, 1f, 0f, 0.15f);
            GUI.DrawTexture(new Rect(0, Screen.height - _zoneHeight,
                Screen.width * 0.5f, _zoneHeight), Texture2D.whiteTexture);
            GUI.color = new Color(0f, 0f, 1f, 0.15f);
            GUI.DrawTexture(new Rect(Screen.width * 0.5f, Screen.height - _zoneHeight,
                Screen.width * 0.5f, _zoneHeight), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}