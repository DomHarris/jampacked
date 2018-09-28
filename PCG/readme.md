# Procedural Content Generators
In here are a couple of PCG algorithms that return a GridSpace[,]. Use this to generate rooms!

Usage:
## RandomWalker
```cs
// options: 
// - Room Size: the size of the room in world units
// - World Units Per Cell: the amount of world units per grid cell. Grid size = round(roomSize/worldUnits)
// - Walker Change Direction Chance (0-1 range, around 0.3 - 0.7 works best): the chance a walker changes direction
// - Walker Spawn Chance (0-1 range, around 0 - 0.15 works best): the chance a new walker spawns
// - Walker Destroy Chance (0-1 range, around 0 - 0.15 works best): the chance a walker is destroyed
// - Max Walkers (around 5 - 15 works best): the max number of walkers in total
// - Percent to Fill (0-1 range, around 0.1-0.3 works best): how much of the grid should be filled with space
// - Iterations (around 100000 works best): how many loops to go through, will stop early if percent to fill is met sooner

GridSpace[,] grid = RandomWalkerGenerator.Generate (new RandomWalkerOptions(new Vector2 (width, height));
for (int x = 0; x < width; ++x)
{
    for (int y = 0; y < height; ++y)
    {
        switch (grid[x,y])
        {
            case: GridSpace.Empty: break;
            case GridSpace.Floor:
                Spawn (floorPF, x, y);
                break;
            case GridSpace.Wall: 
                Spawn (wallPF, x, y);
                break;
        }
    }
}
```

## Cellular Automata
```cs
// options: 
// - Room Size: the size of the room in world units
// - World Units Per Cell: the amount of world units per grid cell. Grid size = round(roomSize/worldUnits)
// - Initial Fill Percent: the percentage of the map that should be randomly filled when the generator starts

GridSpace[,] grid = CellularAutomataGenerator.Generate (new CellularAutomataOptions(new Vector2 (width, height));
for (int x = 0; x < width; ++x)
{   
    for (int y = 0; y < height; ++y)
    {   
        switch (grid[x,y])
        {   
            case: GridSpace.Empty: break;
            case GridSpace.Floor:
                Spawn (floorPF, x, y);
                break;
        }
    }
}
```
