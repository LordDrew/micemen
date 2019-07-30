using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState
{
    public enum TileType
    {
        Empty,
        Wall,
        Mouse
    }
    public enum TurnState
    {
        Blue,
        BlueEnd,
        BlueVictory,
        Red,
        RedEnd,
        RedVictory,
        Draw
    }
    public TileType[,] tiles;
    public TurnState turnState;
    public Vector2Int[] blueMicePositions;
    public Vector2Int[] redMicePositions;
    public int blueScore = 0;
    public int redScore = 0;
    public List<int> validTurns;
    public int lastTurn = -1;
    public bool moveBanned = false;
    public BoardState()
    {
        PlaceTiles();
        PlaceMice(ref blueMicePositions, 1, 10);
        PlaceMice(ref redMicePositions, 11, 20);

        // initial mice movement requires starting from end turn state
        turnState = Random.Range(0, 2) == 0 ? TurnState.BlueEnd : TurnState.RedEnd;
        while (MoveNext());
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
            int walls_in_col = Random.Range(5, 8);
            while (walls_in_col > 0)
            {
                int r = Random.Range(0, 13);
                if (tiles[r, c] == TileType.Wall) continue;
                tiles[r, c] = TileType.Wall;
                walls_in_col--;
            }
        }
    }

    private void PlaceMice(ref Vector2Int[] micePositions, int minColumn, int maxColumn)
    {
        micePositions = new Vector2Int[12];
        int mice = 0;
        while (mice < 12)
        {
            var r = Random.Range(0, 13);
            var c = Random.Range(minColumn, maxColumn);
            if (tiles[r, c] == TileType.Empty)
            {
                tiles[r, c] = TileType.Mouse;
                micePositions[mice] = new Vector2Int(c, r);
                mice++;
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
                    if (mouse.x > 0 && mouse.x < tiles.GetLength(1) - 1)
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
        if (turns.Count > 1 && turns.Contains(lastTurn))
        {
            turns.Remove(lastTurn);
            moveBanned = true;
        }
        else
            moveBanned = false;
        var turnsList = new List<int>();
        foreach (var t in turns)
            turnsList.Add(t);
        turnsList.Sort();
        return turnsList;
    }

    public void MoveUp(int column)
    {
        lastTurn = column;
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
        NextTurnState();
    }
    public void MoveDown(int column)
    {
        lastTurn = column;
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
        NextTurnState();
    }
    public bool MoveNext()
    {
        if (CheckVictory())
            return false;
        if (turnState == TurnState.BlueEnd)
        {
            if (MiceFall(blueMicePositions, ref blueScore))
                return true;
            if (MiceWalk(blueMicePositions, +1))
                return true;
            if (MiceFall(redMicePositions, ref redScore))
                return true;
            if (MiceWalk(redMicePositions, -1))
                return true;
        }
        else
        {
            if (MiceFall(redMicePositions, ref redScore))
                return true;
            if (MiceWalk(redMicePositions, -1))
                return true;
            if (MiceFall(blueMicePositions, ref blueScore))
                return true;
            if (MiceWalk(blueMicePositions, +1))
                return true;
        }
        NextTurnState();
        return false;
    }

    bool CheckVictory()
    {
        if (blueScore == 12)
        {
            turnState = TurnState.BlueVictory;
            return true;
        }
        if (redScore == 12)
        {
            turnState = TurnState.RedVictory;
            return true;
        }
        return false;
    }

    bool MiceFall(Vector2Int[] micePositions, ref int score)
    {
        int selectedMouse = -1;
        for (int i = 0; i < micePositions.Length; i++)
        {
            var x = micePositions[i].x;
            var y = micePositions[i].y;
            if (x < 0)
                continue;
            if (y == 0)
            {
                if (x > 0 && x < tiles.GetLength(1) - 1)
                    continue;
                score++;
                tiles[y, x] = TileType.Empty;
                micePositions[i].x = -1;
                micePositions[i].y = -1;
                return true;
            }
            if (tiles[y - 1, x] != TileType.Empty)
                continue;
            if (selectedMouse >= 0)
            {
                if (y > micePositions[selectedMouse].y)
                    continue;
                if (y == micePositions[selectedMouse].y && x > micePositions[selectedMouse].x)
                    continue;
            }

            selectedMouse = i;
        }
        if (selectedMouse >= 0)
        {
            tiles[micePositions[selectedMouse].y, micePositions[selectedMouse].x] = TileType.Empty;
            micePositions[selectedMouse].y -= 1;
            tiles[micePositions[selectedMouse].y, micePositions[selectedMouse].x] = TileType.Mouse;
            return true;
        }
        return false;
    }

    bool MiceWalk(Vector2Int[] micePositions, int direction)
    {
        int selectedMouse = -1;
        for (int i = 0; i < micePositions.Length; i++)
        {
            var x = micePositions[i].x;
            var y = micePositions[i].y;
            if (x < 0)
                continue;
            if (tiles[y, x + direction] != TileType.Empty)
                continue;
            if (selectedMouse >= 0)
            {
                if (y > micePositions[selectedMouse].y)
                    continue;
                if (y == micePositions[selectedMouse].y && x > micePositions[selectedMouse].x)
                    continue;
            }

            selectedMouse = i;
        }
        if (selectedMouse >= 0)
        {
            tiles[micePositions[selectedMouse].y, micePositions[selectedMouse].x] = TileType.Empty;
            micePositions[selectedMouse].x += direction;
            tiles[micePositions[selectedMouse].y, micePositions[selectedMouse].x] = TileType.Mouse;
            return true;
        }
        return false;
    }

    void NextTurnState()
    {
        switch (turnState)
        {
            case TurnState.Blue:
                turnState = TurnState.BlueEnd;
                break;
            case TurnState.BlueEnd:
                turnState = TurnState.Red;
                break;
            case TurnState.Red:
                turnState = TurnState.RedEnd;
                break;
            case TurnState.RedEnd:
                turnState = TurnState.Blue;
                break;
        }

        validTurns = GetValidTurns();
    }
}
