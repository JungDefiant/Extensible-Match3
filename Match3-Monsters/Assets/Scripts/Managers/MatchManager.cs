﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;

public class MatchManager
{
    private BoardManager boardManager;
    private BlockManager blockManager;
    private Board board;
    private int minMatch;

    public MatchManager(BoardManager boardManager, BlockManager blockManager, int minMatch = 3)
    {
        this.boardManager = boardManager;
        this.blockManager = blockManager;
        this.minMatch = minMatch;
        board = boardManager.Board;
    }

    public IEnumerator CheckMatch()
    {
        List<Tile[]> allMatches = new List<Tile[]>();
        bool matchesFound = true;

        while(matchesFound)
        {
            SearchMatchesOnBoard(allMatches);
            if (allMatches.Count > 0)
            {
                yield return blockManager.CombineMonsters(allMatches);
                yield return boardManager.RepopulateBoard();
            }
            else matchesFound = false;
        }
    }

    private void SearchMatchesOnBoard(List<Tile[]> allMatches)
    {
        for (int x = 0; x < board.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < board.Tiles.GetLength(1); y++)
            {
                CollectMatches(allMatches, board.Tiles[x, y]);
            }
        }
    }

    private void CollectMatches(List<Tile[]> allMatches, Tile tile)
    {
        if (tile.Monster.IsMatched) return;
        
        List<Tile> match = new List<Tile>();
        Vector2Int tileLoc = BoardManager.CoordsToVectorInt(tile.Coordinates);

        tile.Monster.IsMatched = true;

        if (!CollectMatchesHorizontal(tile, tileLoc, match))
            CollectMatchesVertical(tile, tileLoc, match);

        Tile[] matchArray = match.Where(x => x != null)
                                 .ToArray();

        if (matchArray.Length < minMatch)
        {
            foreach (Tile t in matchArray)
            {
                t.Monster.IsMatched = false;
            }
        }
        else
        {
            allMatches.Add(matchArray);
        }
    }

    //private bool CollectMatchesDiagonal(Tile tile, Vector2Int tileLoc, List<Tile> match)
    //{
    //    Tile[] diagMatches = new Tile[boardManager.boardWidth ];
    //    Tile nextTile;

    //    diagMatches[0] = tile;

    //    int index = 1;

    //    for (int x = tileLoc.x + 1; x < boardManager.boardWidth; x++)
    //    {
    //        nextTile = board.Tiles[x, tileLoc.y];
    //        if (!nextTile.Monster.IsMatched &&
    //            tile.Monster.CompareToMonster(nextTile.Monster))
    //        {
    //            diagMatches[index] = nextTile;
    //            nextTile.Monster.IsMatched = true;
    //            index++;
    //        }
    //        else break;
    //    }

    //    for (int x = tileLoc.x - 1; x > 0; x--)
    //    {
    //        nextTile = board.Tiles[x, tileLoc.y];
    //        if (!nextTile.Monster.IsMatched &&
    //            tile.Monster.CompareToMonster(nextTile.Monster))
    //        {
    //            diagMatches[index] = nextTile;
    //            nextTile.Monster.IsMatched = true;
    //            index++;
    //        }
    //        else break;
    //    }

    //    diagMatches = diagMatches.Where(x => x != null)
    //                             .ToArray();

    //    if (diagMatches.Length < minMatch)
    //    {
    //        foreach (Tile t in diagMatches)
    //        {
    //            t.Monster.IsMatched = false;
    //        }

    //        return false;
    //    }
    //    else
    //    {
    //        match.AddRange(diagMatches);
    //        return true;
    //    }
    //}

    private bool CollectMatchesHorizontal(Tile tile, Vector2Int tileLoc, List<Tile> match)
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

        horzMatches = horzMatches.Where(x => x != null)
                                 .ToArray();

        if (horzMatches.Length < minMatch)
        {
            foreach (Tile t in horzMatches)
            {
                t.Monster.IsMatched = false;
            }

            return false;
        }
        else
        {
            match.AddRange(horzMatches);
            return true;
        }
    }

    private bool CollectMatchesVertical(Tile tile, Vector2Int tileLoc, List<Tile> match)
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

        for (int y = tileLoc.y - 1; y > 0; y--)
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

        vertMatches = vertMatches.Where(x => x != null)
                                 .ToArray();

        if (vertMatches.Length < minMatch)
        {
            foreach (Tile t in vertMatches)
            {
                t.Monster.IsMatched = false;
            }

            return false;
        }
        else
        {
            match.AddRange(vertMatches);

            return true;
        }
    }
    
}
