using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MatchManager
{
    private BoardManager boardManager;
    private Board board;
    private int minMatch;

    public MatchManager(BoardManager boardManager, int minMatch = 3)
    {
        this.boardManager = boardManager;
        this.minMatch = minMatch;
        board = boardManager.Board;
    }

    public IEnumerator CheckMatch()
    {
        List<List<Tile>> allMatches = new List<List<Tile>>();
        SearchMatchesOnBoard(allMatches);
        
        yield return null;
    }

    private void SearchMatchesOnBoard(List<List<Tile>> allMatches)
    {
        for (int x = 0; x < board.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < board.Tiles.GetLength(1); y++)
            {
                CollectMatches(allMatches, board.Tiles[x, y]);
            }
        }
    }

    private void CollectMatches(List<List<Tile>> allMatches, Tile tile)
    {
        List<Tile> match = new List<Tile>();
        Vector2Int tileLoc = BoardManager.CoordsToVectorInt(tile.Coordinates);

        match.Add(tile);
        tile.Monster.IsMatched = true;

        CollectMatchesHorizontal(tile, tileLoc, match);
        CollectMatchesVertical(tile, tileLoc, match);

        if (match.Count < minMatch)
        {
            foreach (Tile t in match)
            {
                t.Monster.IsMatched = false;
            }
        }
        else
        {
            allMatches.Add(match);
        }
    }

    private void CollectMatchesHorizontal(Tile tile, Vector2Int tileLoc, List<Tile> match)
    {
        Tile[] horzMatches = new Tile[boardManager.boardWidth];
        Tile nextTile;

        horzMatches[0] = tile;

        int index = 1;

        for (int x = tileLoc.x + 1; x < boardManager.boardWidth; x++)
        {
            nextTile = board.Tiles[x, tileLoc.y];
            if (!nextTile.Monster.IsMatched &&
                tile.Monster.CompareToMonster(nextTile.Monster))
            {
                horzMatches[index] = nextTile;
                nextTile.Monster.IsMatched = true;
                index++;
            }
            else break;
        }

        for (int x = tileLoc.x - 1; x > 0; x--)
        {
            nextTile = board.Tiles[x, tileLoc.y];
            if (!nextTile.Monster.IsMatched &&
                tile.Monster.CompareToMonster(nextTile.Monster))
            {
                horzMatches[index] = nextTile;
                nextTile.Monster.IsMatched = true;
                index++;
            }
            else break;
        }

        if (horzMatches.Length < minMatch)
        {
            foreach (Tile t in horzMatches)
            {
                t.Monster.IsMatched = false;
            }
        }
        else
        {
            match.AddRange(horzMatches);
        }
    }

    private void CollectMatchesVertical(Tile tile, Vector2Int tileLoc, List<Tile> match)
    {
        Tile[] vertMatches = new Tile[boardManager.boardHeight];
        Tile nextTile;

        vertMatches[0] = tile;

        int index = 1;

        for (int y = tileLoc.y + 1; y < boardManager.boardHeight; y++)
        {
            nextTile = board.Tiles[tileLoc.x, y];
            if (!nextTile.Monster.IsMatched &&
                tile.Monster.CompareToMonster(nextTile.Monster))
            {
                vertMatches[index] = nextTile;
                nextTile.Monster.IsMatched = true;
                index++;
            }
            else break;
        }

        for (int y = tileLoc.x - 1; y > 0; y--)
        {
            nextTile = board.Tiles[tileLoc.x, y];
            if (!nextTile.Monster.IsMatched &&
                tile.Monster.CompareToMonster(nextTile.Monster))
            {
                vertMatches[index] = nextTile;
                nextTile.Monster.IsMatched = true;
                index++;
            }
            else break;
        }

        if (vertMatches.Length < minMatch)
        {
            foreach (Tile t in vertMatches)
            {
                t.Monster.IsMatched = false;
            }
        }
        else
        {
            match.AddRange(vertMatches);
        }
    }
}
