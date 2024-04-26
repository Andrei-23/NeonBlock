using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Gameplay gameplay;

    //public float fall_delay = 0.8f;
    private float push_down_delay = 0.125f; // fall speed
    private float push_hrz_delay = 0.25f; // horizontal hold delay
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

    private void Awake()
    {
        gameplay = GetComponent<Gameplay>();

        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        PieceStateManager.Instance.OnPieceStateChanged += OnPieceStateChanged;
        PlayerStatEventManager.Instance.OnDeath += OnDeath;
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        PieceStateManager.Instance.OnPieceStateChanged -= OnPieceStateChanged;
        PlayerStatEventManager.Instance.OnDeath -= OnDeath;
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

        bool clickHold = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.C);

        bool clickDown = Input.GetKeyDown(KeyCode.DownArrow);
        bool clickLeft = Input.GetKeyDown(KeyCode.LeftArrow);
        bool clickRight = Input.GetKeyDown(KeyCode.RightArrow);
        bool pushDown = Input.GetKey(KeyCode.DownArrow);
        bool pushLeft = Input.GetKey(KeyCode.LeftArrow);
        bool pushRight = Input.GetKey(KeyCode.RightArrow);

        is_changed = false;

        if(pushLeft && pushRight)
        {
            pushLeft = pushRight = false;
        }

        // Hold piece
        if(clickHold)
        {
            if (gameplay.pieceManager.HoldPiece())
            {
                OnPieceHold();
            }
        }

        // Horizontal movement
        if (clickLeft || (pushLeft && push_hrz_timer >= push_hrz_delay))
        {
            push_hrz_timer = 0f;
            if (pm.TryMove(-1, 0))
            {
                OnPieceMoved();
            }
        }
        if (clickRight || (pushRight && push_hrz_timer >= push_hrz_delay))
        {
            push_hrz_timer = 0f;
            if(pm.TryMove(1, 0))
            {
                OnPieceMoved();
            }
        }

        // Rotation
        if (Input.GetKeyDown(KeyCode.Z)) // counterclockwise
        {
            if (pm.TryRotateLeft())
            {
                OnPieceRotate();
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (pm.TryRotateRight())
            {
                OnPieceRotate();
            }
        }
        // Hard Drop
        if (Input.GetKeyDown(KeyCode.UpArrow))
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
            if (clickDown || (pushDown && push_down_timer >= push_down_delay))
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.piecePlace);
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.pieceRotate);
        //SoundManager.Instance.rotateSound.Play();
        UpdateLockCount();
        is_changed = true;
    }

    private void OnPieceMoved(bool playSound = true)
    {
        if (playSound)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.pieceMove);
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

    private void OnGameStateChanged(GameState newGameState)
    {
        is_paused = newGameState == GameState.Paused;
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
