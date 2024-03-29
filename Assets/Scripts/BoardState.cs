﻿using System.Collections.Generic;
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
    public interface IMouse
    {
        int X { get; set; }
        int Y { get; set; }
        int XForward { get; }
        bool IsActive { get; set; }
        bool IsCloserToFinishThan(IMouse other);
    }
    class BlueMouse : IMouse
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int XForward => X + 1;
        public bool IsActive { get; set; } = true;

        public bool IsCloserToFinishThan(IMouse other)
        {
            if (Y < other.Y)
                return true;
            if (Y > other.Y)
                return false;
            return X > other.X;
        }
    }
    class RedMouse : IMouse
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int XForward => X - 1;
        public bool IsActive { get; set; } = true;

        public bool IsCloserToFinishThan(IMouse other)
        {
            if (Y < other.Y)
                return true;
            if (Y > other.Y)
                return false;
            return X < other.X;
        }
    }
    public TileType[,] tiles;
    public TurnState turnState;
    public IMouse[] blueMice = new BlueMouse[12];
    public IMouse[] redMice = new RedMouse[12];
    public int blueScore = 0;
    public int redScore = 0;
    public List<int> validTurns;
    public int lastTurn = -1;
    public bool moveBanned = false;
    public BoardState()
    {
        bool fieldSymmetry = PlayerPrefs.GetInt(Settings.symmetricFildKey, 0) == 1;
        bool offsetSymmetry = PlayerPrefs.GetInt(Settings.symmetricTileOffsetKey, 0) == 1;
        bool miceSymmetry = PlayerPrefs.GetInt(Settings.symmetricMicePositionKey, 0) == 1;
        offsetSymmetry = offsetSymmetry && fieldSymmetry;
        miceSymmetry = miceSymmetry && offsetSymmetry;

        for (int i = 0; i < blueMice.Length; i++)
            blueMice[i] = new BlueMouse();
        for (int i = 0; i < redMice.Length; i++)
            redMice[i] = new RedMouse();
        PlaceTiles(fieldSymmetry, offsetSymmetry);
        PlaceMice(blueMice, 1, 10);
        if (!miceSymmetry)
            PlaceMice(redMice, 11, 20);
        else
            PlaceMiceSymmetric(blueMice, redMice);

        // initial mice movement requires starting from end turn state
        turnState = Random.Range(0, 2) == 0 ? TurnState.BlueEnd : TurnState.RedEnd;
        QuickMoveAllMice();
    }

    public BoardState(BoardState other)
    {
        tiles = (TileType[,])other.tiles.Clone();
        turnState = other.turnState;
        for (int i = 0; i < blueMice.Length; i++)
        {
            blueMice[i] = new BlueMouse
            {
                X = other.blueMice[i].X,
                Y = other.blueMice[i].Y,
                IsActive = other.blueMice[i].IsActive
            };
        }
        for (int i = 0; i < redMice.Length; i++)
        {
            redMice[i] = new RedMouse()
            {
                X = other.redMice[i].X,
                Y = other.redMice[i].Y,
                IsActive = other.redMice[i].IsActive
            };
        }
        blueScore = other.blueScore;
        redScore = other.redScore;
        lastTurn = other.lastTurn;
        validTurns = GetValidTurns();
    }
    private void PlaceTiles(bool symmetricField, bool symmetricOffset)
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
        if (symmetricField)
        {
            for (int c = 2; c < 9; c++)
            {
                int walls_in_col = Random.Range(5, 8);
                while (walls_in_col > 0)
                {
                    int r = Random.Range(0, 13);
                    if (tiles[r, c] == TileType.Wall) continue;
                    tiles[r, c] = TileType.Wall;
                    tiles[r, tiles.GetLength(1) - c - 1] = TileType.Wall;
                    walls_in_col--;
                }
            }
        }
        else
        {
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
        if (!symmetricOffset)
        {
            for (int c = 2; c < 19; c++)
            {
                if (c == 9) c = 12;
                int offset = Random.Range(0, 13);
                for (int o = 0; o < offset; o++)
                    MoveDown(c);
            }
        }
    }

    private void PlaceMice(IMouse[] mice, int minColumn, int maxColumn)
    {
        int mouse = 0;
        while (mouse < mice.Length)
        {
            var r = Random.Range(0, 13);
            var c = Random.Range(minColumn, maxColumn);
            if (tiles[r, c] == TileType.Empty)
            {
                tiles[r, c] = TileType.Mouse;
                mice[mouse].X = c;
                mice[mouse].Y = r;
                mouse++;
            }
        }
    }
    private void PlaceMiceSymmetric(IMouse[] placedMice, IMouse[] miceToPlace)
    {
        for (int i = 0; i < placedMice.Length; i++)
        {
            miceToPlace[i].Y = placedMice[i].Y;
            miceToPlace[i].X = tiles.GetLength(1) - 1 - placedMice[i].X;
            tiles[miceToPlace[i].Y, miceToPlace[i].X] = TileType.Mouse;
        }
    }

    List<int> GetValidTurns()
    {
        var turns = new HashSet<int>();
        switch (turnState)
        {
            case TurnState.Blue:
                foreach (var mouse in blueMice)
                {
                    if (mouse.IsActive)
                        turns.Add(mouse.X);
                }
                break;
            case TurnState.Red:
                foreach (var mouse in redMice)
                {
                    if (mouse.IsActive)
                        turns.Add(mouse.X);
                }
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
        for (int row = tiles.GetLength(0) - 1; row > 0; row--)
        {
            tiles[row, column] = tiles[row - 1, column];
        }
        tiles[0, column] = tmp;
        foreach (var mouse in blueMice)
        {
            if (mouse.X == column)
            {
                mouse.Y += 1;
                if (mouse.Y == tiles.GetLength(0))
                    mouse.Y = 0;
            }
        }
        foreach (var mouse in redMice)
        {
            if (mouse.X == column)
            {
                mouse.Y += 1;
                if (mouse.Y == tiles.GetLength(0))
                    mouse.Y = 0;
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
        foreach (var mouse in blueMice)
        {
            if (mouse.X == column)
            {
                mouse.Y -= 1;
                if (mouse.Y == -1)
                    mouse.Y = tiles.GetLength(0) - 1;
            }
        }
        foreach (var mouse in redMice)
        {
            if (mouse.X == column)
            {
                mouse.Y -= 1;
                if (mouse.Y == -1)
                    mouse.Y = tiles.GetLength(0) - 1;
            }
        }
        NextTurnState();
    }
    public bool MoveNextMice()
    {
        if (CheckVictory())
            return false;
        if (turnState == TurnState.BlueEnd)
        {
            if (MiceFall(blueMice, ref blueScore))
                return true;
            if (MiceWalk(blueMice))
                return true;
            if (MiceFall(redMice, ref redScore))
                return true;
            if (MiceWalk(redMice))
                return true;
        }
        else
        {
            if (MiceFall(redMice, ref redScore))
                return true;
            if (MiceWalk(redMice))
                return true;
            if (MiceFall(blueMice, ref blueScore))
                return true;
            if (MiceWalk(blueMice))
                return true;
        }
        NextTurnState();
        return false;
    }

    public void QuickMoveAllMice()
    {
        while (MoveNextMice()) ;
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

    bool MiceFall(IMouse[] mice, ref int score)
    {
        IMouse selectedMouse = null;
        foreach (var mouse in mice)
        {
            if (!mouse.IsActive)
                continue;
            if (mouse.Y == 0)
            {
                if (mouse.X > 0 && mouse.X < tiles.GetLength(1) - 1)
                    continue;
                score++;
                tiles[mouse.Y, mouse.X] = TileType.Empty;
                mouse.IsActive = false;
                return true;
            }
            if (tiles[mouse.Y - 1, mouse.X] != TileType.Empty)
                continue;
            if (selectedMouse != null && selectedMouse.IsCloserToFinishThan(mouse))
                continue;

            selectedMouse = mouse;
        }
        if (selectedMouse != null)
        {
            tiles[selectedMouse.Y, selectedMouse.X] = TileType.Empty;
            selectedMouse.Y -= 1;
            tiles[selectedMouse.Y, selectedMouse.X] = TileType.Mouse;
            return true;
        }
        return false;
    }

    bool MiceWalk(IMouse[] mice)
    {
        IMouse selectedMouse = null;
        foreach (var mouse in mice)
        {
            if (!mouse.IsActive)
                continue;
            if (tiles[mouse.Y, mouse.XForward] != TileType.Empty)
                continue;
            if (selectedMouse != null && selectedMouse.IsCloserToFinishThan(mouse))
                continue;

            selectedMouse = mouse;
        }
        if (selectedMouse != null)
        {
            tiles[selectedMouse.Y, selectedMouse.X] = TileType.Empty;
            selectedMouse.X = selectedMouse.XForward;
            tiles[selectedMouse.Y, selectedMouse.X] = TileType.Mouse;
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
