using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.Collections;
using UnityEngine.Assertions;

[Serializable]
public struct RandomWalkerOptions
{
   public Vector2 RoomSizeWorldUnits;
   public float WorldUnitsPerCell;
   public float WalkerChangeDirChance;
   public float WalkerSpawnChance;
   public float WalkerDestroyChance;
   public int MaxWalkers;
   public float PercentToFill;
   public int Iterations;

   public RandomWalkerOptions(Vector2 size, float worldUnitsPerCell = 1.0f, float walkerChangeDirChance = 0.5f,
      float walkerSpawnChance = 0.05f, float walkerDestroyChance = 0.05f, int maxWalkers = 10, float percentToFill = 0.2f, int iterations = 100000)
   {
      RoomSizeWorldUnits = size;
      WorldUnitsPerCell = worldUnitsPerCell;
      WalkerChangeDirChance = walkerChangeDirChance;
      WalkerSpawnChance = walkerSpawnChance;
      WalkerDestroyChance = walkerDestroyChance;
      MaxWalkers = maxWalkers;
      PercentToFill = percentToFill;
      Iterations = iterations;
   }
}

public static class RandomWalkerGenerator
{
   private struct Walker
   {
      public Vector2 Dir;
      public Vector2 Pos;
   }
   
   private static List<Walker> _walkers;
   
   private static int _roomHeight, _roomWidth;

   private static float Rnd
   {
      get
      {
         return UnityEngine.Random.value;
      } 
   }

   public static GridSpace[,] Generate(RandomWalkerOptions ops)
   {
      var grid = Setup(ops.RoomSizeWorldUnits, ops.WorldUnitsPerCell);
      CreateFloors(ops.Iterations, ops.WalkerDestroyChance, ops.WalkerChangeDirChance, ops.WalkerSpawnChance, ops.MaxWalkers, ops.PercentToFill, ref grid);
      CreateWalls(ref grid);
      return grid;
   }

   private static GridSpace[,] Setup(Vector2 roomSizeWorldUnits, float worldUnitsInOneGridCell)
   {
      // get room height/width in grid units
      _roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
      _roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
      
      // create a new empty grid
      var grid = new GridSpace[_roomWidth,_roomHeight];
      for (var x = 0; x < _roomWidth; ++x)
         for (var y = 0; y < _roomHeight; ++y)
            grid[x, y] = GridSpace.Empty;
      
      // set the first walker's position to the centre of the grid
      var spawnPos = new Vector2(Mathf.RoundToInt(_roomWidth / 2.0f), Mathf.RoundToInt(_roomHeight / 2.0f));
      var newWalker = new Walker {Dir = RandomDirection(), Pos = spawnPos};

      _walkers = new List<Walker> {newWalker};
      return grid;
   }
   
   private static void CreateFloors(int iterations, float walkerDestroyChance, float walkerChangeDirChance, float walkerSpawnChance, int maxWalkers, float percentToFill, ref GridSpace[,] grid)
   {
      // for a certain number of iterations
      for (var num = 0; num < iterations; ++num)
      {
         // set the grid to a floor at the position of each walker
         foreach (var w in _walkers)
            grid[(int) w.Pos.x, (int) w.Pos.y] = GridSpace.Floor;

         // get rid of at most 1 walker based on a random chance if there's more than 1 in total
         for (var i = 0; i < _walkers.Count; ++i)
         {
            if (Rnd < walkerDestroyChance && _walkers.Count > 1)
            {
               _walkers.RemoveAt(i);
               break;
            }
         }
         
         // change the directions of each walker based on a random chance, and spawn new ones based on a random chance
         for (var i = 0; i < _walkers.Count; ++i)
         {
            if (Rnd < walkerChangeDirChance)
            {
               var thisWalker = _walkers[i];
               thisWalker.Dir = RandomDirection();
               _walkers[i] = thisWalker;
            }
            
            if (Rnd < walkerSpawnChance && _walkers.Count < maxWalkers)
            {
               var newWalker = new Walker {Pos = _walkers[i].Pos, Dir = RandomDirection()};
               _walkers.Add(newWalker);
            }
         }
         
         // move the walker in the right direction, clamp it so it's always within the grid
         for (var i = 0; i < _walkers.Count; ++i)
         {
            var thisWalker = _walkers[i];
            thisWalker.Pos += thisWalker.Dir;
            thisWalker.Pos.x = Mathf.Clamp(thisWalker.Pos.x, 1, _roomWidth - 2);
            thisWalker.Pos.y = Mathf.Clamp(thisWalker.Pos.y, 1, _roomHeight - 2);
            _walkers[i] = thisWalker;
         }
         
         // if we've filled up too much of the grid, stop.
         if ((float) NumberOfFloors(grid) / grid.Length > percentToFill) break;
      }
   }
   
   private static void CreateWalls(ref GridSpace[,] grid)
   {
      for (var x = 0; x < _roomWidth; ++x)
      {
         for (var y = 0; y < _roomHeight; ++y)
         {
            // if the current position in the grid is a floor, add a wall to any empty space above, below, to the left or to the right of it
            if (grid[x, y] == GridSpace.Floor)
            {
               if (grid[x, y + 1] == GridSpace.Empty) grid[x, y + 1] = GridSpace.Wall;
               if (grid[x, y - 1] == GridSpace.Empty) grid[x, y - 1] = GridSpace.Wall;
               if (grid[x + 1, y] == GridSpace.Empty) grid[x + 1, y] = GridSpace.Wall;
               if (grid[x - 1, y] == GridSpace.Empty) grid[x - 1, y] = GridSpace.Wall;
            }
         }
      }
   }

   private static int NumberOfFloors(GridSpace[,] grid)
   {
      // return the amount of "floor" tiles in the grid
      return grid.Cast<GridSpace>().Count(space => space == GridSpace.Floor);
   }

   private static Vector2 RandomDirection()
   {
      var choice = Mathf.FloorToInt(Rnd * 3.99f);

      switch (choice)
      {
         case 0:
            return Vector2.down;
         case 1:
            return Vector2.left;
         case 2:
            return Vector2.up;
         default:
            return Vector2.right;
      }
   }
}
