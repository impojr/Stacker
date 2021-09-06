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
    public Image[] blocks;
    private Stacker _stacker;


    private Coroutine tickCoroutine;

    public float Speed = 1f;
    public bool CanPlace;


    // Start is called before the first frame update
    private void Start()
    {
        CanPlace = false;
        blocks = Grid.GetComponentsInChildren<Image>();
        blocks = blocks.Skip(1).ToArray();
        _stacker = new Stacker(7, 14, 3);
        _stacker.Tick();
        tickCoroutine = StartCoroutine(TickGrid());
        StartCoroutine(EnablePlacement());
    }

    private IEnumerator TickGrid()
    {
        while (true)
        {
            yield return new WaitForSeconds(Speed);
            _stacker.Tick();
            DisplayGrid();
        }
    }

    private IEnumerator EnablePlacement()
    {
        //we are doubling the speed before we can place so the player
        //cannot just stack one on top of the other in quick succession
        yield return new WaitForSeconds(Speed * 2);
        CanPlace = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanPlace)
        {
            StopCoroutine(tickCoroutine);
            _stacker.Place();
            CanPlace = false;
            tickCoroutine = StartCoroutine(TickGrid());
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
                    blocks[_stacker.Width * j + i].color = Color.red;
                }
                else
                {
                    blocks[_stacker.Width * j + i].color = Color.green;
                }
            }
        }
    }
}
