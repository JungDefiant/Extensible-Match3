using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public class MonsterBlock : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Image image;

    public bool IsMatched { set; get; }
    public Vector2Int TileCoordinates { set; get; }
    public MonsterType MonsterType { get; set; }
    public Sprite Sprite
    {
        get => image.sprite;
        set => image.sprite = value;
    }

    private void Start()
    {
        IsMatched = false;

        if(MonsterType != null)
        {
            Sprite = MonsterType.Sprite;
        }
    }

    public void SetPosition(Vector2 Coordinates)
    {
        transform.localPosition = Coordinates;
        TileCoordinates = BoardManager.CoordsToVectorInt(Coordinates);
    }

    public void MoveBlockToTileCoord(BoardManager boardManager)
    {
        StartCoroutine("MoveBlockToTile", boardManager);
    }

    public bool CompareToMonster(MonsterBlock monster) => MonsterType.MatchID == monster.MonsterType.MatchID;

    private IEnumerator MoveBlockToTile(BoardManager boardManager)
    {
        Vector2 dest = boardManager.Board.Tiles[TileCoordinates.x, TileCoordinates.y].Coordinates;

        while (Vector2.Distance(transform.localPosition, dest) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, dest, moveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
    }
}