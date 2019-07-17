using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState
{
    public enum TileType
    {
        Empty,
        Wall,
        RedMouse,
        BlueMouse
    }
    public enum TurnState
    {
        Blue,
        BlueEnd,
        Red,
        RedEnd
    }
    public TileType[,] tiles;
    public TurnState turnState;
    public Vector2Int[] blueMicePositions;
    public Vector2Int[] redMicePositions;
    public List<int> validTurns;
    public BoardState()
    {
        PlaceTiles();
        PlaceMice();
        turnState = Random.Range(0, 2) == 0 ? TurnState.Blue : TurnState.Red;
        validTurns = GetValidTurns();
    }

    private void PlaceTiles()
    {
        tiles = new TileType[13, 21];
        for (var r = 0; r < tiles.GetLength(0); r++)
        {
            for (int c = 0; c < tiles.GetLength(1); c++)
            {
                switch (c)
                {
                    case 1:
                    case 19:
                    case 9:
                    case 11:
                        if (r % 3 == 0)
                            tiles[r, c] = TileType.Wall;
                        break;
                    case 10:
                        if (r % 3 != 0)
                            tiles[r, c] = TileType.Wall;
                        break;
                }
            }
        }
        for (int c = 2; c < 19; c++)
        {
            if (c == 9) c = 12;
            int walls_in_col = Random.Range(5, 10);
            while (walls_in_col > 0)
            {
                int r = Random.Range(0, 13);
                if (tiles[r, c] == TileType.Wall) continue;
                tiles[r, c] = TileType.Wall;
                walls_in_col--;
            }
        }
    }

    private void PlaceMice()
    {
        blueMicePositions = new Vector2Int[12];
        int blue_mice = 0;
        while (blue_mice < 12)
        {
            var r = Random.Range(0, 13);
            var c = Random.Range(1, 10);
            if (tiles[r, c] == TileType.Empty)
            {
                while(true)
                {
                    if (r > 0 && tiles[r - 1, c] == TileType.Empty) r--;
                    else if (c < 9 && tiles[r, c + 1] == TileType.Empty) c++;
                    else break;
                }
                tiles[r, c] = TileType.BlueMouse;
                blueMicePositions[blue_mice] = new Vector2Int(c, r);
                blue_mice++;
            }
        }
        redMicePositions = new Vector2Int[12];
        int red_mice = 0;
        while (red_mice < 12)
        {
            var r = Random.Range(0, 13);
            var c = Random.Range(11, 20);
            if (tiles[r, c] == TileType.Empty)
            {
                while (true)
                {
                    if (r > 0 && tiles[r - 1, c] == TileType.Empty) r--;
                    else if (c > 11 && tiles[r, c - 1] == TileType.Empty) c--;
                    else break;
                }
                tiles[r, c] = TileType.RedMouse;
                redMicePositions[red_mice] = new Vector2Int(c, r);
                red_mice++;
            }
        }
    }

    List<int> GetValidTurns()
    {
        var turns = new HashSet<int>();
        switch(turnState)
        {
            case TurnState.Blue:
                foreach (var mouse in blueMicePositions)
                {
                    if (mouse.x > 0)
                        turns.Add(mouse.x);
                }
                break;
            case TurnState.Red:
                foreach (var mouse in redMicePositions)
                {
                    if (mouse.x > 0)
                        turns.Add(mouse.x);
                }
                break;
            case TurnState.BlueEnd:
                break;
            case TurnState.RedEnd:
                break;
        }
        var turnsList = new List<int>();
        foreach (var t in turns)
            turnsList.Add(t);
        turnsList.Sort();
        return turnsList;
    }

    public void MoveUp(int column)
    {
        TileType tmp = tiles[tiles.GetLength(0) - 1, column];
        for(int row = tiles.GetLength(0) - 1; row > 0; row--)
        {
            tiles[row, column] = tiles[row - 1, column];
        }
        tiles[0, column] = tmp;
        for (int i = 0; i < blueMicePositions.Length; i++)
        {
            if (blueMicePositions[i].x == column)
            {
                blueMicePositions[i].y += 1;
                if (blueMicePositions[i].y == tiles.GetLength(0))
                    blueMicePositions[i].y = 0;
            }
        }
        for (int i = 0; i < redMicePositions.Length; i++)
        {
            if (redMicePositions[i].x == column)
            {
                redMicePositions[i].y += 1;
                if (redMicePositions[i].y == tiles.GetLength(0))
                    redMicePositions[i].y = 0;
            }
        }
        if (turnState == TurnState.Blue)
            turnState = TurnState.Red;
        else
            turnState = TurnState.Blue;
        validTurns = GetValidTurns();
    }
    public void MoveDown(int column)
    {
        TileType tmp = tiles[0, column];
        for (int row = 0; row < tiles.GetLength(0) - 1; row++)
        {
            tiles[row, column] = tiles[row + 1, column];
        }
        tiles[tiles.GetLength(0) - 1, column] = tmp;
        for (int i = 0; i < blueMicePositions.Length; i++)
        {
            if (blueMicePositions[i].x == column)
            {
                blueMicePositions[i].y -= 1;
                if (blueMicePositions[i].y == -1)
                    blueMicePositions[i].y = tiles.GetLength(0) - 1;
            }
        }
        for (int i = 0; i < redMicePositions.Length; i++)
        {
            if (redMicePositions[i].x == column)
            {
                redMicePositions[i].y -= 1;
                if (redMicePositions[i].y == -1)
                    redMicePositions[i].y = tiles.GetLength(0) - 1;
            }
        }
        if (turnState == TurnState.Blue)
            turnState = TurnState.Red;
        else
            turnState = TurnState.Blue;
        validTurns = GetValidTurns();
    }
}
