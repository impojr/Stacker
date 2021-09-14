using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    private void Start()
    {
        CanPlace = false;
        Blocks = Grid.GetComponentsInChildren<Image>();
        Blocks = Blocks.Skip(1).ToArray();
        _stacker = new Stacker(7, 14, 3);
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
                }
            }
            CanPlace = false;

            if (_stacker.ActiveRow == _stacker.Height)
            {
                _stacker.ResetHeight();
            }

            _tickCoroutine = StartCoroutine(TickGrid());
            StartCoroutine(EnablePlacement());
        }
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
