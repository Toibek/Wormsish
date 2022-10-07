using UnityEngine;

public class Utils
{
    public static Vector3 RoundVector3(Vector3 vector)
    {
        return new Vector3
            (
                Mathf.Round(vector.x),
                Mathf.Round(vector.y),
                Mathf.Round(vector.z)
            );
    }
    public static Vector3Int RoundVector3ToInt(Vector3 vector)
    {
        return new Vector3Int
            (
                Mathf.RoundToInt(vector.x),
                Mathf.RoundToInt(vector.y),
                Mathf.RoundToInt(vector.z)
            );
    }
}
