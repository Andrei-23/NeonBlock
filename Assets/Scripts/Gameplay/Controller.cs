using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    private Gameplay gameplay;

    //public float fall_delay = 0.8f;
    private float push_down_delay = 0.1f; // fall speed
    private float push_hrz_delay = 0.2f; // horizontal hold delay
    private float lock_delay = 0.5f; // delay before lock

    private float fall_timer = 0f;
    private float push_down_timer = 0f;
    private float push_hrz_timer = 0f;
    private float lock_timer = 0f;

    private bool lockDelayActive = false;
    private int lockMovesCount = 0;
    private int lockMovesLimit = 15;
    private int piece_y_min = 100; // when decreased, reset delayMovesCount

    private bool is_paused = false;
    private bool is_dead = false;
    private bool is_piece_frozen = false;

    private float speed = 0.8f;
    private float initial_fall_delay = 0.8f;
    private float boss_fall_delay = 0.3f;
    private float level_amount = 10f; // expected amount of complete levels before boss

    private bool is_changed = false; // if true at the end of Update(), redraw glass
    private PieceManager pm => gameplay.pieceManager;

    //bool leftPressed = false;
    //bool rightPressed = false;
    //bool softDropPressed = false;

    //bool leftHeld = false;
    //bool rightHeld = false;
    //bool softDropHeld = false;

    //bool holdPressed = false;

    bool leftPressed;
    bool rightPressed;
    bool softDropPressed;
    
    bool leftHeld;
    bool rightHeld;
    bool softDropHeld;

    bool rotateLeftPressed;
    bool rotateRightPressed;
    bool hardDropPressed;
    bool holdPressed;

    private PlayerInput _playerInput;

    private InputAction _moveLeftAction;
    private InputAction _moveRightAction;
    private InputAction _softDropAction;
    private InputAction _hardDropAction;
    private InputAction _rotateLeftAction;
    private InputAction _rotateRightAction;
    private InputAction _holdAction;

    private void Awake()
    {
        gameplay = GetComponent<Gameplay>();

        GameStateManager.Instance.OnPauseStateChanged += OnPauseStateChanged;
        PieceStateManager.Instance.OnPieceStateChanged += OnPieceStateChanged;
        PlayerStatEventManager.Instance.OnDeath += OnDeath;

        _playerInput = gameObject.GetComponent<PlayerInput>();
        SetupInputActions();
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnPauseStateChanged -= OnPauseStateChanged;
        PieceStateManager.Instance.OnPieceStateChanged -= OnPieceStateChanged;
        PlayerStatEventManager.Instance.OnDeath -= OnDeath;
    }

    private void SetupInputActions()
    {
        _moveLeftAction = _playerInput.actions["Left"];
        _moveRightAction = _playerInput.actions["Right"];
        _softDropAction = _playerInput.actions["Soft Drop"];
        _hardDropAction = _playerInput.actions["Hard Drop"];
        _rotateLeftAction = _playerInput.actions["Rotate Left"];
        _rotateRightAction = _playerInput.actions["Rotate Right"];
        _holdAction = _playerInput.actions["Hold"];
    }
    private void UpdateInputs()
    {
        leftPressed = _moveLeftAction.WasPressedThisFrame();
        rightPressed = _moveRightAction.WasPressedThisFrame();
        softDropPressed = _softDropAction.WasPressedThisFrame();
        hardDropPressed = _hardDropAction.WasPressedThisFrame();
        rotateLeftPressed = _rotateLeftAction.WasPressedThisFrame();
        rotateRightPressed = _rotateRightAction.WasPressedThisFrame();
        holdPressed = _holdAction.WasPressedThisFrame();

        leftHeld = _moveLeftAction.IsPressed();
        rightHeld = _moveRightAction.IsPressed();
        softDropHeld = _softDropAction.IsPressed();

        //Debug.Log(leftPressed);
        //Debug.Log(leftHeld);
    }

    private void Start()
    {
        float pw = Mathf.Clamp(Stats.Instance.event_cnt / level_amount, 0f, 1f);
        speed = initial_fall_delay * Mathf.Pow(boss_fall_delay / initial_fall_delay, pw);
        speed /= Mathf.Pow(0.9f, RelicsManager.Instance.GetCount(RelicsManager.RelicType.Drone));
        speed /= Mathf.Pow(1.3f, RelicsManager.Instance.GetCount(RelicsManager.RelicType.ExtraWeight));
    }
    void Update()
    {
        if (!gameplay.started) // Controls diabled on level info
        {
            return;
        }

        UpdateInputs();

        is_changed = false;


        if(leftHeld && rightHeld)
        {
            leftHeld = rightHeld = false;
        }

        // Hold piece
        if(holdPressed)
        {
            if (gameplay.pieceManager.HoldPiece())
            {
                OnPieceHold();
            }
        }

        // Horizontal movement
        if (leftPressed || (leftHeld && push_hrz_timer >= push_hrz_delay))
        {
            push_hrz_timer = 0f;
            if (pm.TryMove(-1, 0))
            {
                OnPieceMoved();
            }
        }
        if (rightPressed || (rightHeld && push_hrz_timer >= push_hrz_delay))
        {
            push_hrz_timer = 0f;
            if(pm.TryMove(1, 0))
            {
                OnPieceMoved();
            }
        }

        // Rotation
        if (rotateLeftPressed) // counterclockwise
        {
            if (pm.TryRotateLeft())
            {
                OnPieceRotate();
            }
        }
        if (rotateRightPressed)
        {
            if (pm.TryRotateRight())
            {
                OnPieceRotate();
            }
        }
        // Hard Drop
        if (hardDropPressed)
        {
            pm.Drop();
            OnPieceLock();
        }

        // Lock delay
        if (pm.IsOnSurface())
        {
            lockDelayActive = true;
            if(lock_timer >= lock_delay || lockMovesCount >= lockMovesLimit)
            {
                // if lock delay ended and piece on surface, lock piece
                pm.PutPiece();
                OnPieceLock();
            }
        }
        // Move down / fall (don't lock pieces here)
        else
        {
            if (softDropPressed || (softDropHeld && push_down_timer >= push_down_delay))
            {
                push_down_timer = 0f;
                fall_timer = 0f;
                if(pm.TryMove(0, -1))
                {
                    OnPieceMoved();
                }
                else
                {
                    Debug.LogWarning("why no space here?2");
                }
            }
            if (fall_timer >= speed)
            {
                fall_timer = 0f; // bad code
                push_down_timer = 0f;

                if (pm.TryMove(0, -1))
                {
                    OnPieceMoved(false);
                }
                else
                {
                    Debug.LogWarning("why no space here?");
                }
            }
        }

        fall_timer += Time.deltaTime;
        push_down_timer += Time.deltaTime;
        push_hrz_timer += Time.deltaTime;
        if (lockDelayActive)
        {
            lock_timer += Time.deltaTime;
        }

        if (is_changed)
        {
            gameplay.drawGlass.Draw();
        }
    }

    private void OnPieceLock()
    {
        AudioManager.Instance.PlaySound(SoundClip.piecePlace);
        //SoundManager.Instance.lockSound.Play();
        OnPieceReset();
    }
    private void OnPieceHold()
    {
        //sound
        OnPieceReset();
    }
    private void OnPieceReset()
    {
        fall_timer = 0f;
        push_down_timer = 0f;
        push_hrz_timer = 0f;
        lock_timer = 0f;
        lockMovesCount = 0;
        piece_y_min = 100;
        lockDelayActive = false;

        is_changed = true;
    }

    private void OnPieceRotate()
    {
        AudioManager.Instance.PlaySound(SoundClip.pieceRotate);
        //SoundManager.Instance.rotateSound.Play();
        UpdateLockCount();
        is_changed = true;
    }

    private void OnPieceMoved(bool playSound = true)
    {
        if (playSound)
        {
            AudioManager.Instance.PlaySound(SoundClip.pieceMove);
        }
        UpdateLockCount();
        is_changed = true;
    }

    private void UpdateLockCount()
    {
        lock_timer = 0f;
        if (pm.y < piece_y_min)
        {
            piece_y_min = pm.y;
            lockMovesCount = 0;
            lockDelayActive = false;
        }
        if (lockDelayActive)
        {
            lockMovesCount++;
        }
    }

    private void OnPauseStateChanged(bool is_paused)
    {
        this.is_paused = is_paused;
        enabled = !is_paused && !is_piece_frozen && !is_dead;
    }


    private void OnPieceStateChanged(PieceState newPieceState)
    {
        is_piece_frozen = newPieceState == PieceState.Animation;
        enabled = !is_paused && !is_piece_frozen && !is_dead;
    }
    private void OnDeath()
    {
        is_dead = true;
        enabled = false;
    }

}
