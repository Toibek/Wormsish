using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public List<Team> Teams;
    public int TeamSize;
    public int Moves;
    public int Specials;
    [Space]
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Material _unitMaterial;
    [SerializeField] private ObjectManager _objectGen;
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private CinemachineOrbitalTransposer _followOrbital;
    [Space]
    [SerializeField] private Transform _movesHolder;
    [SerializeField] private Transform _specialsHolder;

    private Unit _activeUnit;
    private Vector2Int _activePlace;

    private int _currentMoves;
    private int _currentSpecials;

    private InputHandler _inputHandler;
    private void Start()
    {
        _followOrbital = _followCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        _inputHandler = GetComponent<InputHandler>();
    }
    private void MoveUnit(Vector2 moveDirection)
    {
        if (_currentMoves > 0 && _activeUnit.Movement.Move(moveDirection))
        {
            if (--_currentMoves == 0 && _currentSpecials == 0)
            {
                Invoke(nameof(ChangeUnit), 1);
            }

            UpdateUiChildren(_specialsHolder, _currentSpecials, 0);
            UpdateUiChildren(_movesHolder, _currentMoves, 0);
        }
    }
    public void Rotation(Vector2 rotation)
    {
        _followOrbital.m_XAxis.Value = rotation.x * 0.2f;
        _activeUnit.Movement.Rotation();
    }
    private void ToggleOverview()
    {
        _followCam.Priority *= -1;
    }
    public void ChangeUnit()
    {
        _activeUnit.transform.GetChild(1).gameObject.SetActive(false);
        if (_activePlace.y + 1 >= Teams[_activePlace.x].Units.Count)
        {
            int avaliableTeam = -1;
            if (Teams.Count > _activePlace.x)
                for (int i = _activePlace.x + 1; i < Teams.Count; i++)
                {
                    if (Teams[i].Active)
                    {
                        avaliableTeam = i;
                        break;
                    }
                }
            if (avaliableTeam == -1)
                for (int i = 0; i < _activePlace.x; i++)
                {
                    if (Teams[i].Active)
                    {
                        avaliableTeam = i;
                        break;
                    }
                }
            if (avaliableTeam != -1)
                _activePlace = new(avaliableTeam, 0);
            else _activePlace = new(_activePlace.x, 0);
        }
        else
            _activePlace += new Vector2Int(0, 1);
        SetActivePlayer(_activePlace);

        UpdateUiChildren(_specialsHolder, _currentSpecials, 0);
        UpdateUiChildren(_movesHolder, _currentMoves, 0);
    }
    private void UpdateUiChildren(Transform uiToReset, int amountToEnable, int childToEnable)
    {
        for (int i = 0; i < uiToReset.childCount; i++)
            uiToReset.GetChild(i).GetChild(childToEnable).gameObject.SetActive(i < amountToEnable);
    }
    void SetActivePlayer(Vector2Int player)
    {
        _activeUnit = Teams[player.x].Units[player.y];
        _activePlace = new(player.x, player.y);
        _activeUnit.transform.GetChild(1).gameObject.SetActive(true);
        _followCam.LookAt = _activeUnit.transform;
        _followCam.Follow = _activeUnit.transform;
        _activeUnit.Movement.CamTransform = _followCam.transform;
        _currentMoves = Moves;
        _currentSpecials = Specials;
    }
    public IEnumerator SpawnPlayers()
    {
        List<GameObject> spawnPoints = new List<GameObject>();
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("Respawn"));
        int n = Teams.Count * TeamSize;
        if (spawnPoints.Count < n)
        {
            Debug.Log("Asking for more respawn points");
            spawnPoints.AddRange(_objectGen.More("Spawner", n - spawnPoints.Count));
        }

        for (int team = 0; team < Teams.Count; team++)
        {
            Teams[team].Active = true;
            Material mat = new(_unitMaterial);
            mat.color = Teams[team].color;
            for (int unit = 0; unit < TeamSize; unit++)
            {
                int r = Random.Range(0, spawnPoints.Count);
                GameObject go = Instantiate(_unitPrefab, spawnPoints[r].transform.position, Quaternion.identity, transform);
                go.GetComponentInChildren<MeshRenderer>().material = mat;
                Destroy(spawnPoints[r]);
                spawnPoints.RemoveAt(r);
                Unit u = go.GetComponent<Unit>();
                u.PlayerManager = this;
                Teams[team].Units.Add(u);
                yield return new WaitForEndOfFrame();
            }
        }
        for (int i = spawnPoints.Count - 1; i >= 0; i--)
        {
            Destroy(spawnPoints[i]);
            spawnPoints.RemoveAt(i);
        }
        SetActivePlayer(new(0, 0));

        _inputHandler.OnMovementStay = MoveUnit;
        _inputHandler.OnRotationStay = Rotation;
        _inputHandler.OnPassTurn = ChangeUnit;
        _inputHandler.OnShowMap = ToggleOverview;

        DuplicateFirstChild(_specialsHolder, Specials);
        _specialsHolder.gameObject.SetActive(true);

        DuplicateFirstChild(_movesHolder, Moves);
        _movesHolder.gameObject.SetActive(true);

        _followCam.m_Priority = 1;
    }
    private void DuplicateFirstChild(Transform content, int amount)
    {
        GameObject uiRef = content.GetChild(0).gameObject;
        for (int i = 1; i < amount; i++)
            Instantiate(uiRef, content);
    }
    public void ReportUnitDeath(Unit unit)
    {
        if (_activeUnit == unit)
            ChangeUnit();
        for (int i = 0; i < Teams.Count; i++)
        {
            if (Teams[i].Units.Contains(unit))
            {
                Teams[i].Units.Remove(unit);
                if (Teams[i].Units.Count <= 0)
                {
                    Teams[i].Active = false;
                    Team t = CheckForWinner();
                    if (t != null) DeclareWinner(t);

                }
                break;
            }
        }
    }
    private Team CheckForWinner()
    {
        int winner = -1;
        for (int i = 0; i < Teams.Count; i++)
        {
            if (winner == -1 && Teams[i].Active)
                winner = i;
            else if (Teams[i].Active)
                return null;
        }
        return Teams[winner];
    }
    private void DeclareWinner(Team team)
    {
        Debug.Log(team.Name + " is the winner!");
    }
}
[System.Serializable]
public class Team : object
{
    public bool Active;
    public string Name;
    public Color color;
    public List<Unit> Units;
}
