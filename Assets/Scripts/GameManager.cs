using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameObject Grid;
    public Image[] Blocks;

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
    public Button playAgainButton;
    public Button menuButton;

    public GameObject creditsPanel;

    public AudioSource UiTextScaleAudioSource;
    public AudioSource UiTextMovementAudioSource;
    public AudioSource NewHighScoreAudioSource;
    public AudioSource GridClearedAudioSource;
    public AudioSource MissAudioSource;
    public AudioSource PlaceAudioSource;
    public AudioSource TickAudioSource;
    public AudioSource GameOverAudioSource;

    private Stacker _stacker;
    private Coroutine _tickCoroutine;
    private float _timeBetweenTicks = 0.5f;
    private bool _canPlace;
    private float _initialTimeBetweenTicks;
    private int _rowsCleared;
    private int _gridsCleared;
    private int _speed;
    private int _score;
    private int _highScore;

    private Vector3 _currentScoreTextPosition;
    private Vector3 _speedTextPosition;
    private Vector3 _gpHighScoreTextPosition;
    private Vector3 _rowsClearedTextPosition;
    private Vector3 _gridsClearedTextPosition;

    private bool updateHighScoreColor;

    private SortedList<int, float> _speedModifiers;
    private readonly string _highScoreKey = "HighScore";

    private void InitializeSpeedModifiers()
    {
        _speedModifiers = new SortedList<int, float>();
        _speedModifiers.Add(3, 0.475f);
        _speedModifiers.Add(7, 0.45f);
        _speedModifiers.Add(10, 0.425f);
        _speedModifiers.Add(14, 0.4f);
        _speedModifiers.Add(21, 0.375f);
        _speedModifiers.Add(27, 0.35f);
        _speedModifiers.Add(34, 0.325f);
        _speedModifiers.Add(40, 0.3f);
        _speedModifiers.Add(47, 0.275f);
        _speedModifiers.Add(53, 0.25f);
        _speedModifiers.Add(60, 0.225f);
        _speedModifiers.Add(66, 0.2f);
        _speedModifiers.Add(73, 0.175f);
        _speedModifiers.Add(79, 0.165f);
        _speedModifiers.Add(92, 0.155f);
        _speedModifiers.Add(105, 0.145f);
        _speedModifiers.Add(118, 0.135f);
        _speedModifiers.Add(131, 0.125f);
        _speedModifiers.Add(144, 0.115f);
        _speedModifiers.Add(157, 0.105f);
        _speedModifiers.Add(170, 0.095f);
        _speedModifiers.Add(183, 0.085f);
        _speedModifiers.Add(196, 0.08f);
        _speedModifiers.Add(209, 0.075f);
        _speedModifiers.Add(222, 0.07f);
        _speedModifiers.Add(235, 0.065f);
        _speedModifiers.Add(248, 0.06f);
        _speedModifiers.Add(261, 0.055f);
        _speedModifiers.Add(274, 0.05f);
        _speedModifiers.Add(287, 0.045f);
        _speedModifiers.Add(300, 0.04f);
        _speedModifiers.Add(313, 0.035f);
        _speedModifiers.Add(326, 0.03f);
        _speedModifiers.Add(352, 0.025f);
    }

    private void SetInitialTextFieldPositions()
    {
        _currentScoreTextPosition = currentScoreText.transform.position;
        _speedTextPosition = speedText.transform.position;
        _gpHighScoreTextPosition = gpHighScoreText.transform.position;
        _rowsClearedTextPosition = rowsClearedText.transform.position;
        _gridsClearedTextPosition = gridsClearedText.transform.position;
    }

    private void ResetTextFieldPositions()
    {
        currentScoreText.transform.position = _currentScoreTextPosition;
        currentScoreText.transform.localScale = Vector3.one;

        speedText.transform.position = _speedTextPosition;
        speedText.transform.localScale = Vector3.one;

        gpHighScoreText.transform.position = _gpHighScoreTextPosition;
        gpHighScoreText.transform.localScale = Vector3.one;

        rowsClearedText.transform.position = _rowsClearedTextPosition;
        rowsClearedText.transform.localScale = Vector3.one;

        gridsClearedText.transform.position = _gridsClearedTextPosition;
        gridsClearedText.transform.localScale = Vector3.one;
    }

    private void Start()
    {
        InitializeSpeedModifiers();
        SetInitialTextFieldPositions();

        _highScore = PlayerPrefs.GetInt(_highScoreKey, 0);
        ssHighScoreText.text = _highScore.ToString();

        _initialTimeBetweenTicks = _timeBetweenTicks;
        
        ShowStartMenu();

        _canPlace = false;
        Blocks = Grid.GetComponentsInChildren<Image>();
        Blocks = Blocks.Skip(1).ToArray();
    }

    public void StartGame()
    {
        updateHighScoreColor = false;
        gpHighScoreText.color = Color.white;
        DOTween.Kill(gpHighScoreText.transform);
        
        startScreenPanel.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        gameplayScreenPanel.SetActive(true);

        InitializeGameVariables();
        ResetTextFieldPositions();

        _stacker = new Stacker(7, 14, 3);
        DisplayGrid();
        _stacker.Tick();
        _tickCoroutine = StartCoroutine(TickGrid());
        StartCoroutine(EnablePlacement());
    }

    public void ShowStartMenu()
    {
        gameplayScreenPanel.SetActive(false);
        startScreenPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        gameplayScreenPanel.SetActive(false);
        startScreenPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    private void InitializeGameVariables()
    {
        _timeBetweenTicks = _initialTimeBetweenTicks;
        _score = 0;
        _speed = 1;
        _gridsCleared = 0;
        _rowsCleared = 0;

        currentScoreText.text = _score.ToString();
        speedText.text = _speed.ToString();
        gridsClearedText.text = _gridsCleared.ToString();
        rowsClearedText.text = _rowsCleared.ToString();
        gpHighScoreText.text = _highScore.ToString();
    }

    private IEnumerator TickGrid()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeBetweenTicks);
            _stacker.Tick();
            TickAudioSource.Play();
            DisplayGrid();
        }
    }

    private IEnumerator EnablePlacement()
    {
        //we are doubling the speed before we can place so the player
        //cannot just stack one cell on top of the other in quick succession
        yield return new WaitForSeconds(_timeBetweenTicks * 2);
        _canPlace = true;
    }

    private float lerpTime = 5f;
    private float t = 0f;
    private int colourIndex = 0;

    private Color[] highScoreColours = new[]
    {
        Color.red, new Color(1, 0.5f, 0), new Color(1,1,0), Color.green, Color.blue, new Color(75f/255f, 0, 130f/255f), new Color(148f/255f,0,211f/255f) 
    };

    private void Update()
    {
        if (updateHighScoreColor)
        {
            gpHighScoreText.color = Color.Lerp(gpHighScoreText.color, highScoreColours[colourIndex],
                lerpTime * Time.deltaTime);
            
            t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);
            if (t > 0.9f)
            {
                t = 0f;
                colourIndex++;
                colourIndex = colourIndex >= highScoreColours.Length ? 0 : colourIndex;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameOver();
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _highScore = 0;
            _score = 100;
            GameOver();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _canPlace)
        {
            StopCoroutine(_tickCoroutine);
            var missedSquares = _stacker.Place();
            if (missedSquares.Count != 0)
            {
                if (_stacker.StackWidth < 1)
                {
                    GameOver();
                    return;
                }

                MissAudioSource.Play();
            }
            else
            {
                PlaceAudioSource.Play();
            }

            UpdateScore();
            UpdateRowsCleared();
            IncreaseSpeed();

            _canPlace = false;

            if (_stacker.ActiveRow == _stacker.Height)
            {
                _stacker.ResetHeight();
                GridClearedAudioSource.Play();
                UpdateGridsCleared();
            }

            _tickCoroutine = StartCoroutine(TickGrid());
            StartCoroutine(EnablePlacement());
        }
    }

    private void UpdateGridsCleared()
    {
        _gridsCleared++;
        gridsClearedText.text = _gridsCleared.ToString();
    }

    private void UpdateRowsCleared()
    {
        _rowsCleared++;
        rowsClearedText.text = _rowsCleared.ToString();
    }

    private void UpdateScore()
    {
        _score += _stacker.StackWidth;
        currentScoreText.text = _score.ToString();
    }

    private void IncreaseSpeed()
    {
        if (!_speedModifiers.ContainsKey(_rowsCleared))
            return;

        _speed = _speedModifiers.IndexOfKey(_rowsCleared) + 2;
        _timeBetweenTicks = _speedModifiers[_rowsCleared];

        speedText.text = _speed == _speedModifiers.Count + 1 ? "MAX" : _speed.ToString();
    }

    private void GameOver()
    {
        GameOverAudioSource.Play();
        StopCoroutine(_tickCoroutine);
        _canPlace = false;
        var newHighScoreAchieved = false;

        if (_score > _highScore)
        {
            newHighScoreAchieved = true;
            _highScore = _score;
            PlayerPrefs.SetInt(_highScoreKey, _score);
            PlayerPrefs.Save();
        }

        Effects(newHighScoreAchieved);
    }

    private void Effects(bool newHighScoreAchieved)
    {
        float moveDistance = 50f;
        float moveDuration = 0.1f;
        Vector3 scaleEffectVector = new Vector3(2, 2, 2);
        Vector3 effectVector = new Vector3(1, 1, 1);
        float scaleEffectDuration = 0.25f;
        float effectDuration = 0.1f;

        List<TMP_Text> test = new List<TMP_Text> { currentScoreText, speedText, rowsClearedText, gridsClearedText };

        if (!newHighScoreAchieved)
        {
            test.Add(gpHighScoreText);
        }
        else
        {
            gpHighScoreText.gameObject.SetActive(false);
        }

        test.ForEach(x => x.gameObject.SetActive(false));

        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(2f);

        foreach (var text in test)
        {
            mySequence.Append(text.transform.DOMoveX(text.transform.position.x - moveDistance, moveDuration).OnStart(() =>
                {
                    text.gameObject.SetActive(true);
                    UiTextMovementAudioSource.Play();
                }));
            mySequence.Append(text.transform.DOScale(scaleEffectVector, scaleEffectDuration).OnComplete(() =>
            {
                UiTextScaleAudioSource.Play();
                text.transform.DOPunchScale(new Vector3(0, 2, 2), effectDuration);
                text.transform.DOPunchRotation(effectVector, effectDuration);
                text.transform.DOPunchPosition(effectVector, effectDuration);
            }));

            mySequence.AppendInterval(0.5f);
        }

        if (newHighScoreAchieved)
        {
            mySequence.AppendInterval(0.5f);

            mySequence.Append(gpHighScoreText.transform.DOMoveX(gpHighScoreText.transform.position.x - moveDistance, moveDuration).OnStart(() =>
            {
                gpHighScoreText.gameObject.SetActive(true);
                UiTextMovementAudioSource.Play();
            }));
            mySequence.Append(gpHighScoreText.transform.DOScale(scaleEffectVector, scaleEffectDuration).OnComplete(() =>
            {
                UiTextScaleAudioSource.Play();
                NewHighScoreAudioSource.Play();
                gpHighScoreText.transform.DOPunchScale(new Vector3(0, 2, 2), effectDuration);
                gpHighScoreText.transform.DOPunchRotation(effectVector, effectDuration);
                gpHighScoreText.transform.DOPunchPosition(effectVector, effectDuration);
                gpHighScoreText.transform.DOScaleY(3f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                updateHighScoreColor = true;
            }));
        }

        mySequence.OnComplete(() =>
        {
            playAgainButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
        });
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
