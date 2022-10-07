using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Bridge", menuName = "Tools/Bridge", order = 1)]
public class BuildingTool : BaseTool
{
    [Space]
    public int Distance = 5;
    public int Width = 1;
    public Vector3 buildOffset = new(0, -1, 0);
    public GameObject BlockPrefab;

    public override void Use(Transform unit, float force = 1)
    {
        IslandManager islandManager = GameObject.FindGameObjectWithTag("Island").GetComponent<IslandManager>();
        float rot = Mathf.Round(unit.transform.rotation.eulerAngles.y / 90) * 90f;
        Vector3 forward = Quaternion.Euler(0, rot, 0) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0, rot, 0) * Vector3.right;
        Vector3Int startPos = Utils.RoundVector3ToInt(unit.transform.position + buildOffset + (forward - right));
        for (int y = 0; y < Distance; y++)
        {
            Vector3Int currentPosition = startPos + Utils.RoundVector3ToInt(-right * Width / 2) + Utils.RoundVector3ToInt(forward) * y;
            for (int x = 0; x < Width; x++)
            {
                currentPosition += Utils.RoundVector3ToInt(right);

                islandManager.GetCube(currentPosition);
            }
        }
        base.Use(unit);
    }
}
