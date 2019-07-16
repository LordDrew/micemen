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
    public BoardState()
    {
        PlaceTiles();
        PlaceMice();
        turnState = Random.Range(0, 2) == 0 ? TurnState.Blue : TurnState.Red;
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
                blue_mice++;
            }
        }
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
                red_mice++;
            }
        }
    }

    bool Fall()
    {
        return false;
    }
}
