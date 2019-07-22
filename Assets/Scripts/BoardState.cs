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
        turnState = Random.Range(0, 2) == 0 ? TurnState.BlueEnd : TurnState.RedEnd;
        PlaceTiles();
        PlaceMice();
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
                tiles[r, c] = TileType.RedMouse;
                redMicePositions[red_mice] = new Vector2Int(c, r);
                red_mice++;
            }
        }
        while (MoveNext());
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
            if (BlueMiceFall())
                return true;
            if (BlueMiceWalk())
                return true;
            if (RedMiceFall())
                return true;
            if (RedMiceWalk())
                return true;
        }
        else
        {
            if (RedMiceFall())
                return true;
            if (RedMiceWalk())
                return true;
            if (BlueMiceFall())
                return true;
            if (BlueMiceWalk())
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

    bool BlueMiceFall()
    {
        int selectedMouse = -1;
        for (int i = 0; i < blueMicePositions.Length; i++)
        {
            var x = blueMicePositions[i].x;
            var y = blueMicePositions[i].y;
            if (x < 0)
                continue;
            if (y == 0)
            {
                if (x < tiles.GetLength(1) - 1)
                    continue;
                blueScore++;
                tiles[blueMicePositions[i].y, blueMicePositions[i].x] = TileType.Empty;
                blueMicePositions[i].x = -1;
                blueMicePositions[i].y = -1;
                return true;
            }
            if (tiles[y - 1, x] != TileType.Empty)
                continue;
            if (selectedMouse >= 0)
            {
                if (y > blueMicePositions[selectedMouse].y)
                    continue;
                if (y == blueMicePositions[selectedMouse].y && x < blueMicePositions[selectedMouse].x)
                    continue;
            }

            selectedMouse = i;
        }
        if (selectedMouse >= 0)
        {
            tiles[blueMicePositions[selectedMouse].y, blueMicePositions[selectedMouse].x] = TileType.Empty;
            blueMicePositions[selectedMouse].y -= 1;
            tiles[blueMicePositions[selectedMouse].y, blueMicePositions[selectedMouse].x] = TileType.BlueMouse;
            return true;
        }
        return false;
    }

    bool RedMiceFall()
    {
        int selectedMouse = -1;
        for (int i = 0; i < redMicePositions.Length; i++)
        {
            var x = redMicePositions[i].x;
            var y = redMicePositions[i].y;
            if (x < 0)
                continue;
            if (y == 0)
            {
                if (x > 0)
                    continue;
                redScore++;
                tiles[redMicePositions[i].y, redMicePositions[i].x] = TileType.Empty;
                redMicePositions[i].x = -1;
                redMicePositions[i].y = -1;
                return true;
            }
            if (tiles[y - 1, x] != TileType.Empty)
                continue;
            if (selectedMouse >= 0)
            {
                if (y > redMicePositions[selectedMouse].y)
                    continue;
                if (y == redMicePositions[selectedMouse].y && x > redMicePositions[selectedMouse].x)
                    continue;
            }

            selectedMouse = i;
        }
        if (selectedMouse >= 0)
        {
            tiles[redMicePositions[selectedMouse].y, redMicePositions[selectedMouse].x] = TileType.Empty;
            redMicePositions[selectedMouse].y -= 1;
            tiles[redMicePositions[selectedMouse].y, redMicePositions[selectedMouse].x] = TileType.RedMouse;
            return true;
        }
        return false;
    }

    bool BlueMiceWalk()
    {
        int selectedMouse = -1;
        for (int i = 0; i < blueMicePositions.Length; i++)
        {
            var x = blueMicePositions[i].x;
            var y = blueMicePositions[i].y;
            if (x < 0)
                continue;
            if (x == tiles.GetLength(1) - 1)
                continue;
            if (tiles[y, x + 1] != TileType.Empty)
                continue;
            if (selectedMouse >= 0)
            {
                if (y > blueMicePositions[selectedMouse].y)
                    continue;
                if (y == blueMicePositions[selectedMouse].y && x < blueMicePositions[selectedMouse].x)
                    continue;
            }

            selectedMouse = i;
        }
        if (selectedMouse >= 0)
        {
            tiles[blueMicePositions[selectedMouse].y, blueMicePositions[selectedMouse].x] = TileType.Empty;
            blueMicePositions[selectedMouse].x += 1;
            tiles[blueMicePositions[selectedMouse].y, blueMicePositions[selectedMouse].x] = TileType.BlueMouse;
            return true;
        }
        return false;
    }

    bool RedMiceWalk()
    {
        int selectedMouse = -1;
        for (int i = 0; i < redMicePositions.Length; i++)
        {
            var x = redMicePositions[i].x;
            var y = redMicePositions[i].y;
            if (x < 0)
                continue;
            if (x == 0)
                continue;
            if (tiles[y, x - 1] != TileType.Empty)
                continue;
            if (selectedMouse >= 0)
            {
                if (y > redMicePositions[selectedMouse].y)
                    continue;
                if (y == redMicePositions[selectedMouse].y && x > redMicePositions[selectedMouse].x)
                    continue;
            }

            selectedMouse = i;
        }
        if (selectedMouse >= 0)
        {
            tiles[redMicePositions[selectedMouse].y, redMicePositions[selectedMouse].x] = TileType.Empty;
            redMicePositions[selectedMouse].x -= 1;
            tiles[redMicePositions[selectedMouse].y, redMicePositions[selectedMouse].x] = TileType.RedMouse;
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
