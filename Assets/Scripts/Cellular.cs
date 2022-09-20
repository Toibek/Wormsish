using UnityEngine;

public static class Cellular
{
    static int lastNeighbors = 6;
    /// <summary>
    /// Generate a bool array with cellular automata
    /// </summary>
    /// <param name="size">size of the array</param>
    /// <param name="noise">base noise density</param>
    /// <param name="neighbors">neighbors required to remain true through the itterations</param>
    /// <param name="iterations">amount of itteration before it returns</param>
    /// <returns>a finished generated noise array</returns>
    static public bool[,] GenerateArray(int size = 50, int noise = 88, int neighbors = 6, int iterations = 10)
    {
        bool[,] pixels = Create(new(size + 25, size + 25), noise);
        for (int i = 0; i < iterations; i++)
            pixels = Iterate(pixels, neighbors);
        return pixels;
    }
    /// <summary>
    /// Generate a texture throguh cellular automata
    /// </summary>
    /// <param name="size">size of the texture</param>
    /// <param name="noise">base noise density</param>
    /// <param name="neighbors">neighbors required to remain true through the itterations</param>
    /// <param name="iterations">amount of itteration before it returns</param>
    /// <returns>a finished generated noise texture </returns>
    static public Texture2D GenerateTexture(int size = 50, int noise = 88, int neighbors = 6, int iterations = 10)
    {
        bool[,] pixels = GenerateArray(size, noise, neighbors, iterations);
        return Texturize(pixels);
    }
    /// <summary>
    /// Creates a bool array with the size and noise density provided
    /// </summary>
    /// <param name="size">the size of the array</param>
    /// <param name="density">the noise density</param>
    /// <returns></returns>
    static bool[,] Create(Vector2Int size, int density)
    {
        bool[,] pixels = new bool[size.x, size.y];
        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                float dis = Vector2Int.Distance(new Vector2Int(x, y), new Vector2Int(size.x / 2, size.y / 2)) / (size.x * 0.01f);
                pixels[x, y] = Random.Range(0, 100) < density - dis;
            }
        }
        return pixels;
    }
    /// <summary>
    /// Running the provided bool array through the generator and returning the itterated bool array, using the last amount of required neighbors.
    /// </summary>
    /// <param name="pixels">Bool array to iterate</param>
    /// <returns>An iterated bool array</returns>
    static public bool[,] Iterate(bool[,] pixels) { return Iterate(pixels, lastNeighbors); }
    /// <summary>
    /// Running the provided bool array through the generator and returning the itterated bool array, with an option for the amount of neighbors.
    /// </summary>
    /// <param name="pixels">Bool array to iterate</param>
    /// <param name="neighbors">Amount of neighbors required to change to alive</param>
    /// <returns>An iterated bool array</returns>
    static public bool[,] Iterate(bool[,] pixels, int neighbors)
    {
        lastNeighbors = neighbors;
        bool[,] copy = new bool[pixels.GetLength(0), pixels.GetLength(1)];
        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                copy[x, y] = CheckNeighbors(x, y, pixels, neighbors);
            }
        }
        return copy;
    }
    /// <summary>
    /// Generating a texture from a bool array with white for true and black for false
    /// </summary>
    /// <param name="pixels">Bool array to use</param>
    /// <returns>Black and white texture with the </returns>
    static Texture2D Texturize(bool[,] pixels)
    {
        Texture2D texture = new(pixels.GetLength(0), pixels.GetLength(1));
        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                texture.SetPixel(x, y, pixels[x, y] ? Color.white : Color.black);
            }
        }
        texture.Apply();
        return texture;
    }
    /// <summary>
    /// checking the nearest neighbors and returning wether or not they have more than "neighbors" neighbors
    /// </summary>
    /// <param name="xo">Origin on the x axis</param>
    /// <param name="yo">Origin on the y axis</param>
    /// <param name="pixels">Bool array that it's checking</param>
    /// <param name="neighbors">Amount of neighbors to match </param>
    /// <returns></returns>
    static bool CheckNeighbors(int xo, int yo, bool[,] pixels, int neighbors)
    {
        int on = 0;

        for (int x = xo - 1; x <= xo + 1; x++)
        {
            int xt = x;
            for (int y = yo - 1; y <= yo + 1; y++)
            {
                int yt = y;

                if (xt < 0) xt = pixels.GetLength(0) - 1;
                if (yt < 0) yt = pixels.GetLength(1) - 1;

                if (xt > pixels.GetLength(0) - 1) xt = 0;
                if (yt > pixels.GetLength(1) - 1) yt = 0;

                if (xt == xo && yt == yo) continue;
                if (pixels[xt, yt]) on++;
            }
        }
        bool output = (on >= neighbors);
        return output;
    }
}
