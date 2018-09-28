using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.Collections;
using UnityEngine.Assertions;

[Serializable]
public struct CellularAutomataOptions
{
    public Vector2 RoomSizeWorldUnits;
    public float WorldUnitsPerCell;
    public float InitialFillPercent;

    public CellularAutomataOptions(Vector2 size, float worldUnitsPerCell, float initialFillPercent = 0.5f)
    {
        RoomSizeWorldUnits = size;
        InitialFillPercent = initialFillPercent;
        WorldUnitsPerCell = worldUnitsPerCell;
    }
}

public static class CellularAutomataGenerator
{
    private static int _roomHeight, _roomWidth;

    private static float Rnd
    {
        get { return UnityEngine.Random.value; }
    }

    public static GridSpace[,] Generate(CellularAutomataOptions ops)
    {
        var grid = Setup(ops.RoomSizeWorldUnits, ops.WorldUnitsPerCell);
        CreateFloors(ops.InitialFillPercent, ref grid);
        return grid;
    }

    private static GridSpace[,] Setup(Vector2 roomSizeWorldUnits, float worldUnitsInOneGridCell)
    {
        // get room height/width in grid units
        _roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        _roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);

        // create a new empty grid
        var grid = new GridSpace[_roomWidth, _roomHeight];
        for (var x = 0; x < _roomWidth - 1; ++x)
        for (var y = 0; y < _roomWidth - 1; ++y)
            grid[x, y] = GridSpace.Empty;

        return grid;
    }

    private static void CreateFloors(float initialFillPercent, ref GridSpace[,] grid)
    {
        RandomFillMap(initialFillPercent, ref grid);

        for (var i = 0; i < 10; ++i)
        {
            SmoothMap(ref grid);
        }
    }

    private static void RandomFillMap(float initialFillPercent, ref GridSpace[,] grid)
    {
        for (var x = 0; x < _roomWidth; ++x)
        {
            for (var y = 0; y < _roomHeight; ++y)
            {
                if (x == 0 || x == _roomWidth - 1 || y == 0 || y == _roomHeight - 1)
                    grid[x, y] = GridSpace.Floor;
                else
                    grid[x, y] = Rnd < initialFillPercent ? GridSpace.Floor : GridSpace.Empty;
            }
        }
    }

    private static int GetSurroundingWallCount(int checkX, int checkY, GridSpace[,] grid)
    {
        var wallCount = 0;
        for (var x = checkX - 1; x <= checkX + 1; ++x)
        {
            for (var y = checkY - 1; y <= checkY + 1; ++y)
            {
                if (x != checkX || y != checkY)
                {
                    if (InBounds(x, y))
                        wallCount += grid[x, y] == GridSpace.Floor ? 1 : 0;
                    else
                        ++wallCount;
                }
            }
        }

        return wallCount;
    }

    private static bool InBounds(int x, int y)
    {
        return x >= 0 && x < _roomWidth && y >= 0 && y < _roomHeight;
    }

    private static void SmoothMap(ref GridSpace[,] map)
    {
        for (var x = 0; x < _roomWidth; ++x)
        {
            for (var y = 0; y < _roomHeight; ++y)
            {
                var surroundingWalls = GetSurroundingWallCount(x, y, map);

                if (surroundingWalls > 4)
                    map[x, y] = GridSpace.Floor;
                else if (surroundingWalls < 4)
                    map[x, y] = GridSpace.Empty;
            }
        }
    }
}