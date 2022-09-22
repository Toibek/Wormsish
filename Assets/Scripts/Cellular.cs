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
    /// <param name="iterations">amount of itteration before it returns</param>
    /// <returns>a finished generated noise array</returns>
    static public bool[,,] GenerateArray(int seed = -1, int layers = 1, int size = 50, int noise = 88, int neighbors = 6, int iterations = 10)
    {
        if (seed == -1)
            seed = Random.Range(-99999, 99999);
        Seed = seed;

        bool[,,] pixels = Create(seed, new(size + 25, layers, size + 25), noise);
        for (int i = 0; i < iterations; i++)
            pixels = Iterate(pixels, neighbors);
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
        for (int y = 0; y < size.y; y++)
            for (int x = 0; x < pixels.GetLength(0); x++)
                for (int z = 0; z < pixels.GetLength(1); z++)
                {
                    float dis = Vector2Int.Distance(new Vector2Int(x, z), new Vector2Int(size.x / 2, size.y / 2)) / (size.x * 0.01f);
                    pixels[x, y, z] = Random.Range(0, 100) < density - dis;
                }
        return pixels;
    }

    /// <summary>
    /// Running the provided bool array through the generator and returning the itterated bool array, with an option for the amount of neighbors.
    /// </summary>
    /// <param name="pixels">Bool array to iterate</param>
    /// <param name="neighbors">Amount of neighbors required to change to alive</param>
    /// <returns>An iterated bool array</returns>
    static public bool[,,] Iterate(bool[,,] pixels, int neighbors)
    {
        bool[,,] copy = new bool[pixels.GetLength(0), pixels.GetLength(1), pixels.GetLength(2)];

        for (float y = 0; y < pixels.GetLength(1); y += 0.5f)
            for (int x = 0; x < pixels.GetLength(0); x++)
                for (int z = 0; z < pixels.GetLength(2); z++)
                    copy[x, Mathf.FloorToInt(y), z] = CheckNeighbors(new(x, Mathf.FloorToInt(y), z), pixels, neighbors);
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
            int x = Mathf.Clamp(origin.x - 1, 0, pixels.GetLength(0)-1);
            x <= Mathf.Clamp(origin.x + 1, 0, pixels.GetLength(0)-1);
            x++
            )
        {
            for
                (
                int z = Mathf.Clamp(origin.z - 1, 0, pixels.GetLength(2)-1);
                z <= Mathf.Clamp(origin.z - 1, 0, pixels.GetLength(2)-1);
                z++)
            {
                if (x == origin.x && x == origin.z) continue;
                if (pixels[x, origin.y, z]) on++;
            }
        }
        bool output = (on >= neighbors);
        return output;
    }
}
