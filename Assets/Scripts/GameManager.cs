using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject Grid;
    public Image[] Blocks;
    private Stacker _stacker;

    private Coroutine _tickCoroutine;

    public float TimeBetweenTicks = 0.5f;
    public bool CanPlace;
    public float TimeDecreaseIncrement = 0.025f;
    public float MinTimeBetweenTicks = 0.05f;

    [Space] 
    [Header("UI Components")]
    public GameObject startScreenPanel;
    public TMP_Text ssHighScoreText;
    
    public GameObject gameplayScreenPanel;
    public TMP_Text gpHighScoreText;
    public TMP_Text currentScoreText;
    public TMP_Text speedText;
    public TMP_Text rowsClearedText;
    public TMP_Text gridsClearedText;

    public GameObject gameoverScreenPanel;
    public TMP_Text goHighScoreText;
    public TMP_Text finalScoreText;

    private float _initialTimeBetweenTicks;
    private int _rowsCleared;
    private int _gridsCleared;
    private int _speed;
    private int _score;
    private int _highScore;
    private readonly string _highScoreKey = "HighScore";

    // Start is called before the first frame update
    private void Start()
    {
        _highScore = PlayerPrefs.GetInt(_highScoreKey, 0);
        _initialTimeBetweenTicks = TimeBetweenTicks;

        ssHighScoreText.text = _highScore.ToString();

        gameplayScreenPanel.SetActive(false);
        gameoverScreenPanel.SetActive(false);
        startScreenPanel.SetActive(true);

        CanPlace = false;
        Blocks = Grid.GetComponentsInChildren<Image>();
        Blocks = Blocks.Skip(1).ToArray();
    }

    public void StartGame()
    {
        gameoverScreenPanel.SetActive(false);
        startScreenPanel.SetActive(false);
        gameplayScreenPanel.SetActive(true);

        //Initialize/Reset Game Variables
        TimeBetweenTicks = _initialTimeBetweenTicks;
        _score = 0;
        _speed = 1;
        _gridsCleared = 0;

        currentScoreText.text = _score.ToString();
        speedText.text = _speed.ToString();
        gridsClearedText.text = _gridsCleared.ToString();
        rowsClearedText.text = _rowsCleared.ToString();
        gpHighScoreText.text = _highScore.ToString();

        _stacker = new Stacker(7, 14, 3);
        DisplayGrid();
        _stacker.Tick();
        _tickCoroutine = StartCoroutine(TickGrid());
        StartCoroutine(EnablePlacement());
    }

    private IEnumerator TickGrid()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeBetweenTicks);
            _stacker.Tick();
            DisplayGrid();
        }
    }

    private IEnumerator EnablePlacement()
    {
        //we are doubling the speed before we can place so the player
        //cannot just stack one on top of the other in quick succession
        yield return new WaitForSeconds(TimeBetweenTicks * 2);
        CanPlace = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanPlace)
        {
            StopCoroutine(_tickCoroutine);
            var missedSquares = _stacker.Place();
            if (missedSquares.Count == 0)
            {
                if (TimeBetweenTicks > MinTimeBetweenTicks)
                {
                    TimeBetweenTicks -= TimeDecreaseIncrement;
                    _speed++;
                    speedText.text = _speed.ToString();
                }
                else
                {
                    speedText.text = "MAX";
                }

                _score += _stacker.StackWidth;
                currentScoreText.text = _score.ToString();
                _rowsCleared++;
                rowsClearedText.text = _rowsCleared.ToString();
            }
            else
            {
                if (_stacker.StackWidth < 1)
                {
                    GameOver();
                    return;
                }

                _score += _stacker.StackWidth;
                currentScoreText.text = _score.ToString();
                _rowsCleared++;
                rowsClearedText.text = _rowsCleared.ToString();
            }
            CanPlace = false;

            if (_stacker.ActiveRow == _stacker.Height)
            {
                _stacker.ResetHeight();
                _gridsCleared++;
                gridsClearedText.text = _gridsCleared.ToString();
            }

            _tickCoroutine = StartCoroutine(TickGrid());
            StartCoroutine(EnablePlacement());
        }
    }

    private void GameOver()
    {
        StopCoroutine(_tickCoroutine);
        CanPlace = false;

        if (_score > _highScore)
        {
            _highScore = _score;
            PlayerPrefs.SetInt(_highScoreKey, _score);
            PlayerPrefs.Save();
        }

        goHighScoreText.text = _highScore.ToString();
        finalScoreText.text = _score.ToString();

        startScreenPanel.SetActive(false);
        gameplayScreenPanel.SetActive(false);
        gameoverScreenPanel.SetActive(true);
    }

    private void DisplayGrid()
    {
        var stack = _stacker.Stack;
        for (var i = 0; i < _stacker.Width; i++)
        {
            for (var j = 0; j < _stacker.Height; j++)
            {
                if (stack[i, j].State == State.Occupied)
                {
                    Blocks[_stacker.Width * j + i].color = Color.red;
                }
                else
                {
                    Blocks[_stacker.Width * j + i].color = Color.green;
                }
            }
        }
    }
}
