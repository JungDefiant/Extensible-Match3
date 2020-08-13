using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform slider;
    [SerializeField] private MonsterBlock blockPrefab;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private MonsterType[] allTypes;

    private bool gameStart = true;

    public MatchManager MatchManager { get; private set; }
    public BlockManager BlockManager { get; private set; }
    public InputManager InputManager { get; private set; }
    public BoardManager BoardManager { get; private set; }
    public MonsterManager MonsterManager { get; private set; }
    
    /// <summary>
    /// Initializes all manager classes
    /// </summary>
    void Awake()
    {
        BoardManager = new BoardManager();
        BoardManager.PopulateBoard(blockPrefab, allTypes, transform);

        InputManager = new InputManager();

        BlockManager = new BlockManager(BoardManager, InputManager, canvas.GetComponent<GraphicRaycaster>(), slider, transform);
        BlockManager.SliderSpeed = sliderSpeed;

        MatchManager = new MatchManager(BoardManager, BlockManager);
    }

    private void Start()
    {
        StartCoroutine("Game");
    }

    private IEnumerator Game()
    {
        InputManager.InputMode = InputMode.Wait;
        yield return MatchManager.CheckMatch();
        InputManager.InputMode = InputMode.Move;

        while (gameStart)
        {
            yield return InputManager.HandleInput();
            if (InputManager.InputMode == InputMode.Wait) {
                yield return MatchManager.CheckMatch();
                InputManager.InputMode = InputMode.Move;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    /*
     * TO DO [Gameplay Loop]:
     * 
     * BlockManager
     * 1. Method that gets all monster blocks in a row or column [DONE]
     * 2. Method that places all blocks on a slider that moves with the cursor [DONE]
     *      2a. Method that checks if monster is off the board and places 
     *      it on the end of the slider
     * 3. Method that finds where blocks will be placed based on their current location [DONE]
     * 4. Method that moves monster blocks to location [DONE]
     * 
     * MatchManager
     * 5. Method that checks matches on board
     * 6. Method that combines monsters into a match
     * 7. Method that moves all monsters down to fill empty tiles
     * 8. Method that respawns monsters in remaining tiles
     * 
     * TO DO [Data]:
     * 1. Update score whenever monster match is made
     * 2. Tick down moves counter whenever move is made
     */
}
