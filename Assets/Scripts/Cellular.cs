using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Cellular
{
    static public Vector2Int Size = new(50, 50);
    static public int NoiseDensity = 88;
    static public int Neighbors = 6;
    static public int Iterations = 10;

    static public bool[,] GenerateArray(int size = 50, int noise = 88, int neighbors = 6, int iterations = 10)
    {
        Size = new(size, size);
        NoiseDensity = noise;
        Neighbors = neighbors;
        Iterations = iterations;

        bool[,] pixels = Create();
        for (int i = 0; i < Iterations; i++)
            pixels = Iterate(pixels);
        return pixels;
    }
    static public Texture2D GenerateTexture(int size = 50, int noise = 88, int neighbors = 6, int iterations = 10)
    {
        Size = new(size, size);
        NoiseDensity = noise;
        Neighbors = neighbors;
        Iterations = iterations;

        bool[,] pixels = GenerateArray();
        return Texturize(pixels);
    }
    static bool[,] Create()
    {
        bool[,] pixels = new bool[Size.x, Size.y];
        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                float dis = Vector2Int.Distance(new Vector2Int(x, y), new Vector2Int(Size.x/2, Size.y/2));
                pixels[x, y] = Random.Range(0, 100) < NoiseDensity - dis;
            }
        }
        return pixels;
    }
    static public bool[,] Iterate(bool[,] pixels)
    {
        bool[,] copy = new bool[pixels.GetLength(0), pixels.GetLength(1)];
        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                copy[x, y] = CheckNeighbors(x, y, pixels);
            }
        }
        return copy;
    }
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
    static bool CheckNeighbors(int xo, int yo, bool[,] pixels)
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
        bool output = (on >= Neighbors);
        return output;
    }
}
