using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnPauseGame;
    public event EventHandler OnUnPauseGame;
    public static GameManager Instance;
    public event EventHandler<OnStateChangedEventArg> OnStateChanged;
    public event EventHandler OnLocalPlayerReadyChanged;

    public class OnStateChangedEventArg : EventArgs
    {
        public State state;
    }

    public enum State
    {
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private bool isLocalPlayerReady = false;
    private float waitingToStartTimer = .5f;
    private float countDownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 90;
    private bool isGamePause;

    private void Awake()
    {
        state = State.WaitingToStart;
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnPause += GameInput_OnPause;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f)
                {
                    state = State.CountDownToStart;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArg { state = state });
                }
                break;
            case State.CountDownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArg { state = state });
                    gamePlayingTimer = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArg { state = state });
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void GameInput_OnPause(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    public bool GetIsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public void SetIsLocalPlayerReady(bool value)
    {
        isLocalPlayerReady = value;
        OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TogglePauseGame()
    {
        isGamePause = !isGamePause;
        if (isGamePause)
        {
            Time.timeScale = 0f;
            OnPauseGame?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnUnPauseGame?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public float GetCountDownToStartTimer()
    {
        return countDownToStartTimer;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }
}
