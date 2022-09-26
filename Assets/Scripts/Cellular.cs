using UnityEngine;

public static class Cellular
{
    public static int Seed = -1;
    /// <summary>
    /// Generate a bool array with cellular automata
    /// </summary>
    /// <param name="size">size of the array</param>
    /// <param name="noise">base noise density</param>
    /// <param name="neighbors">neighbors required to remain true through the itterations</param>
    /// <param name="TaperingIterations">amount of itteration before it returns</param>
    /// <returns>a finished generated noise array</returns>
    static public bool[,,] GenerateArray(int seed = -1, int layers = 1, int size = 50, int noise = 88, int neighbors = 6, int baseIterations = 4, int TaperingIterations = 10)
    {
        if (seed == -1)
            seed = Random.Range(-99999, 99999);
        Seed = seed;

        bool[,,] pixels = Create(seed, new(size, layers, size), noise);
        for (int i = 0; i < baseIterations; i++)
            pixels = Iterate(pixels, neighbors, 0);
        for (int i = 0; i < TaperingIterations; i++)
        {
            int yStart = Mathf.FloorToInt(((float)i / (float)TaperingIterations) * layers);
            pixels = Iterate(pixels, neighbors, yStart);
        }
        return pixels;
    }
    /// <summary>
    /// Creates a bool array with the size and noise density provided
    /// </summary>
    /// <param name="size">the size of the array</param>
    /// <param name="density">the noise density</param>
    /// <returns></returns>
    static bool[,,] Create(int seed, Vector3Int size, int density)
    {
        bool[,,] pixels = new bool[size.x, size.y, size.z];
        Random.InitState(seed);
        for (int x = 0; x < pixels.GetLength(0); x++)
            for (int z = 0; z < pixels.GetLength(2); z++)
            {
                float dis = Vector2Int.Distance(new Vector2Int(x, z), new Vector2Int(size.x / 2, size.z / 2)) * (size.x * 0.01f);
                bool val = Random.Range(0, 100) < density - dis;
                for (int y = 0; y < size.y; y++)
                {
                    pixels[x, y, z] = val;
                }
            }
        return pixels;
    }

    /// <summary>
    /// Running the provided bool array through the generator and returning the itterated bool array, with an option for the amount of neighbors.
    /// </summary>
    /// <param name="pixels">Bool array to iterate</param>
    /// <param name="neighbors">Amount of neighbors required to change to alive</param>
    /// <returns>An iterated bool array</returns>
    static public bool[,,] Iterate(bool[,,] pixels, int neighbors, int yStart = 0)
    {
        bool[,,] copy = new bool[pixels.GetLength(0), pixels.GetLength(1), pixels.GetLength(2)];

        for (int x = 0; x < pixels.GetLength(0); x++)
            for (int z = 0; z < pixels.GetLength(2); z++)
            {
                bool val = CheckNeighbors(new(x, yStart, z), pixels, neighbors);
                for (int y = 0; y < yStart; y++)
                {
                    copy[x, y, z] = pixels[x, y, z];
                }
                for (int y = yStart; y < pixels.GetLength(1); y++)
                {
                    copy[x, y, z] = val;
                }
            }
        return copy;
    }
    /// <summary>
    /// checking the nearest neighbors and returning wether or not they have more than "neighbors" neighbors
    /// </summary>
    /// <param name="xo">Origin on the x axis</param>
    /// <param name="yo">Origin on the y axis</param>
    /// <param name="pixels">Bool array that it's checking</param>
    /// <param name="neighbors">Amount of neighbors to match </param>
    /// <returns></returns>
    static bool CheckNeighbors(Vector3Int origin, bool[,,] pixels, int neighbors)
    {
        int on = 0;
        for (
            int x = Mathf.Clamp(origin.x - 1, 0, pixels.GetLength(0) - 1);
            x <= Mathf.Clamp(origin.x + 1, 0, pixels.GetLength(0) - 1);
            x++
            )
        {
            for
                (
                int z = Mathf.Clamp(origin.z - 1, 0, pixels.GetLength(2) - 1);
                z <= Mathf.Clamp(origin.z + 1, 0, pixels.GetLength(2) - 1);
                z++)
            {
                if (x == origin.x && z == origin.z || origin.y > 0 && !pixels[x, origin.y - 1, z]) continue;
                if (pixels[x, origin.y, z]) on++;
            }
        }
        bool output = (on >= neighbors);
        return output;
    }
}
