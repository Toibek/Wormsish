using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int MaxPlayers = 8;
    public int MinPlayers = 2;

    public SpawnableObject[] Spawnables;
    public SpawnableObject[] Pickups;

    internal IslandManager IslandGen;
    internal ObjectManager ObjectGen;
    internal PlayerManager PlayerManager;

    internal int Units;
    internal int Moves;
    internal int Specials;
    internal int PickupFreq;

    internal int Barrels;
    internal int Mines;
    internal int Healthpacks;


    public Vector3 ValidPosition
    {
        get
        {
            if (_positions == null || _positions.Count == 0)
                _positions = RefreshPositions();
            Vector3 position = _positions[Random.Range(0, _positions.Count)];
            _positions.Remove(position);
            return position;
        }
    }
    List<Vector3> _positions;

    public List<Vector3> RefreshPositions()
    {
        bool[,] placeableArea = IslandGen.PlaceableArea;
        Vector3 offset = new(-placeableArea.GetLength(0) / 2, 0, -placeableArea.GetLength(1) / 2);
        List<Vector3> positions = new();
        for (int x = 0; x < placeableArea.GetLength(0); x++)
            for (int z = 0; z < placeableArea.GetLength(1); z++)
                if (placeableArea[x, z]) positions.Add(new Vector3(x, 0, z) + offset);
        return positions;
    }
    private void Awake()
    {
        IslandGen = GameObject.FindGameObjectWithTag("Island").GetComponent<IslandManager>();
        ObjectGen = GameObject.FindGameObjectWithTag("Objects").GetComponent<ObjectManager>();
        PlayerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
    }
    public void LoadIntoGame()
    {
        SceneManager.LoadScene(1);
    }
    public void SetTeam(string name, Color color)
    {
        PlayerManager.Teams.Add(new(name, color));
    }
    public void StartGame()
    {
        PlayerManager.TeamSize = Units;
        PlayerManager.Moves = Moves;
        PlayerManager.Specials = Specials;
        StartCoroutine(PlayerManager.SpawnPlayers());
    }
}
[System.Serializable]
public class SpawnableObject : object
{
    public string Name;
    public GameObject Prefab;
    public int BaseAmount;
    internal List<GameObject> InactiveObjects;
    public GameObject GetObject(Vector3 position, Transform parent)
    {
        if (InactiveObjects == null) InactiveObjects = new List<GameObject>();
        if (InactiveObjects.Count > 0)
        {
            GameObject go = InactiveObjects[0];
            go.transform.position = position;
            go.transform.SetParent(parent);
            InactiveObjects.Remove(go);
            go.SetActive(true);
            return go;
        }
        else
            return GameObject.Instantiate(Prefab, position, Quaternion.identity, parent);

    }
    public void DeactivateObject(GameObject go)
    {
        if (InactiveObjects == null) InactiveObjects = new List<GameObject>();
        InactiveObjects.Add(go);
        go.SetActive(false);
    }
}
