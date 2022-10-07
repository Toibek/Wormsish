using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int MaxPlayers = 8;
    public int MinPlayers = 2;
    [Space]
    public int AirdropHeight = 10;
    [Space]
    public List<SpawnableObject> Spawnables = new();
    public List<SpawnableObject> Pickups = new();
    public bool Paused = false;

    [SerializeField] GameObject airdropPrefab;

    internal IslandManager IslandGen;
    internal ObjectManager ObjectGen;
    internal PlayerManager PlayerManager;
    internal MenuManager MenuManager;

    internal int Units;
    internal int Moves;
    internal int Specials;
    internal int PickupPerTurn;

    public GameObject RandomPickup
    {
        get
        {
            SpawnableObject spawnable = Pickups[Random.Range(0, Pickups.Count)];
            GameObject go = spawnable.GetObject(new(0, 0, 0), ObjectGen.transform);
            ObjectGen._activeObjects.Add(go);
            go.SetActive(false);
            return go;
        }
    }
    public GameObject Airdrop
    {
        get
        {
            if (pooledAirdrops == null) pooledAirdrops = new();
            if (pooledAirdrops.Count > 0)
            {
                GameObject go = pooledAirdrops[0];
                pooledAirdrops.Remove(go);
                return go;
            }
            return Instantiate(airdropPrefab, transform);
        }
        set
        {
            value.SetActive(false);
            if (pooledAirdrops == null) pooledAirdrops = new();
            pooledAirdrops.Add(value);
        }
    }
    List<GameObject> pooledAirdrops;
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
    public BaseTool RandomToolDrop
    {
        get
        {
            if (DroppableTools == null || DroppableTools.Count == 0) return null;
            return Instantiate(DroppableTools[Random.Range(0, DroppableTools.Count)]);
        }
    }
    public List<BaseTool> AllTools = new();
    internal List<BaseTool> DroppableTools = new();
    internal List<BaseTool> InfiniteTools = new();

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
        MenuManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<MenuManager>();
    }
    public void SetTeam(string name, Color color)
    {
        PlayerManager.Teams.Add(new(name, color));
    }
    public void StartGame()
    {
        if (DroppableTools.Count == 0)
        {
            for (int i = 0; i < Spawnables.Count; i++)
                if (Spawnables[i].Name == "Weapon") Spawnables.RemoveAt(i);
            for (int i = 0; i < Pickups.Count; i++)
                if (Pickups[i].Name == "Weapon") Pickups.RemoveAt(i);
        }

        PlayerManager.InfiniteTools = InfiniteTools;
        PlayerManager.TeamSize = Units;
        PlayerManager.Moves = Moves;
        PlayerManager.Specials = Specials;
        StartCoroutine(PlayerManager.SpawnPlayers());
    }
    public void SpawnAirdrops()
    {
        for (int i = 0; i < PickupPerTurn; i++)
        {
            GameObject go = Airdrop;
            go.transform.position = ValidPosition + (Vector3.up * AirdropHeight);
            go.SetActive(true);
        }
    }
    public void GameOver(Team winningTeam)
    {
        ObjectGen.ClearObjects();
        MenuManager.GameOver(winningTeam);
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
            go.GetComponent<DamageableObject>().ReturnTo = this;
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject go = GameObject.Instantiate(Prefab, position, Quaternion.identity, parent);
            go.GetComponent<DamageableObject>().ReturnTo = this;
            return go;
        }

    }
    public void DeactivateObject(GameObject go)
    {
        if (InactiveObjects == null) InactiveObjects = new List<GameObject>();
        InactiveObjects.Add(go);
        go.SetActive(false);
    }
}