using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public List<Team> Teams;
    public int TeamSize;
    public int Moves;
    public int Specials;
    [Space]
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Material _unitMaterial;
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [Space]
    [SerializeField] private Transform _movesHolder;
    [SerializeField] private Transform _specialsHolder;
    [SerializeField] private Transform _teamNamesHolder;
    [SerializeField] private GameObject _pressSpace;
    [SerializeField] private Transform _forceBar;

    internal List<BaseTool> InfiniteTools;

    private TMP_Text[] teamNames;
    private CinemachineOrbitalTransposer _followOrbital;
    private Unit _activeUnit;
    private Vector2Int _activePlace;
    private int _currentMoves;
    private int _currentSpecials;

    private GameManager _gameManager;
    private InputHandler _inputHandler;
    private bool _teamChange;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        _followOrbital = _followCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        _inputHandler = GetComponent<InputHandler>();
    }
    private void MoveUnit(Vector2 moveDirection)
    {
        if (_teamChange || _gameManager.Paused) return;
        if (_currentMoves > 0 && _activeUnit.Movement.Move(moveDirection))
        {
            --_currentMoves;
            UpdateUiChildren(_specialsHolder, _currentSpecials, 0);
            UpdateUiChildren(_movesHolder, _currentMoves, 0);
        }
    }
    public void Rotation(Vector2 rotation)
    {
        if (_teamChange || _gameManager.Paused) return;
        _followOrbital.m_XAxis.Value = rotation.x * 0.2f;
        _activeUnit.Rotate();
    }
    private void ToggleOverview()
    {
        if (_teamChange || _gameManager.Paused) return;
        _followCam.Priority *= -1;
    }
    public void SkipTurn()
    {
        if (_teamChange)
            StartNewTeam();
        else
            ChangeUnit();
    }
    public void ChangeUnit()
    {
        _activeUnit.Active = false;
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
            {
                _followCam.Priority = -1;
                _teamChange = true;
                _pressSpace.SetActive(true);
                _activePlace = new(avaliableTeam, 0);
                UpdateTeamorder();
            }
            else _activePlace = new(_activePlace.x, 0);
        }
        else
            _activePlace += new Vector2Int(0, 1);
        _gameManager.SpawnAirdrops();

        SetActivePlayer(_activePlace);
        UpdateUiChildren(_specialsHolder, _currentSpecials, 0);
        UpdateUiChildren(_movesHolder, _currentMoves, 0);
    }
    void ChangeTool(float direction)
    {
        if (_teamChange || _gameManager.Paused) return;
        _activeUnit.Tools.ChangeTool(Mathf.Sign(direction));
    }
    private void StartNewTeam()
    {
        _followCam.Priority = 1;
        _pressSpace.SetActive(false);
        _teamChange = false;
    }
    private void UpdateUiChildren(Transform uiToReset, int amountToEnable, int childToEnable)
    {
        for (int i = 0; i < uiToReset.childCount; i++)
            uiToReset.GetChild(i).GetChild(childToEnable).gameObject.SetActive(i < amountToEnable);
    }
    private void UpdateTeamorder()
    {
        if (teamNames == null || teamNames.Length == 0)
        {
            teamNames = _teamNamesHolder.GetComponentsInChildren<TMP_Text>();
        }
        List<string> teamsInOrder = new();
        for (int i = _activePlace.x; i < Teams.Count; i++)
            if (Teams[i].Active) teamsInOrder.Add(Teams[i].Name);
        for (int i = 0; i < _activePlace.x; i++)
            if (Teams[i].Active) teamsInOrder.Add(Teams[i].Name);

        for (int i = 0; i < teamNames.Length; i++)
        {
            if (teamsInOrder.Count > i)
            {
                teamNames[i].transform.parent.gameObject.SetActive(true);
                teamNames[i].text = teamsInOrder[i];
            }
            else
                teamNames[i].transform.parent.gameObject.SetActive(false);
        }
    }
    public void DisplayForce(float f)
    {
        f = Mathf.Clamp01(f);
        _forceBar.GetChild(1).localScale = new(f, 1);
    }
    private void SetActivePlayer(Vector2Int player)
    {
        _activeUnit = Teams[player.x].Units[player.y];
        _activePlace = new(player.x, player.y);
        _followCam.LookAt = _activeUnit.transform;
        _followCam.Follow = _activeUnit.transform;
        _activeUnit.Camera = _followCam.transform;
        _currentMoves = Moves;
        _currentSpecials = Specials;
        _activeUnit.Active = true;
    }
    public IEnumerator SpawnPlayers()
    {
        int n = Teams.Count * TeamSize;
        for (int team = 0; team < Teams.Count; team++)
        {
            Teams[team].Active = true;
            for (int unit = 0; unit < TeamSize; unit++)
            {
                Material mat = new(_unitMaterial);
                mat.color = Teams[team].Color;

                Ray ray = new(_gameManager.ValidPosition + Vector3.up * 50, Vector2.down);
                Physics.Raycast(ray, out var hit);
                GameObject go = Instantiate(_unitPrefab, hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity, transform);
                go.GetComponentInChildren<MeshRenderer>().material = mat;
                go.GetComponent<UnitDamageable>().Material = mat;
                Unit u = go.GetComponent<Unit>();
                u.PlayerManager = this;
                Teams[team].Units.Add(u);

                for (int i = 0; i < InfiniteTools.Count; i++)
                {
                    InfiniteTools[i].Uses = -1;
                    u.Tools.AddTool(Instantiate(InfiniteTools[i]));
                }
                //u.Tools.ChangeTool(0);

                yield return new WaitForEndOfFrame();
            }
        }
        SetActivePlayer(new(0, 0));

        _inputHandler.OnMovementStay = MoveUnit;
        _inputHandler.OnRotationStay = Rotation;
        _inputHandler.OnPassTurn = SkipTurn;
        _inputHandler.OnShowMap = ToggleOverview;
        _inputHandler.OnShootStart = ShootStart;
        _inputHandler.OnShootEnd = ShootEnd;
        _inputHandler.OnChangeTool = ChangeTool;

        DuplicateFirstChild(_specialsHolder, Specials);
        _specialsHolder.gameObject.SetActive(true);

        DuplicateFirstChild(_movesHolder, Moves);
        _movesHolder.gameObject.SetActive(true);

        _pressSpace.SetActive(true);
        _teamChange = true;

        UpdateTeamorder();
    }
    public void ShootStart()
    {
        if (_teamChange || _activeUnit.Tools.ActiveTool == null) return;
        if (_currentSpecials <= 0) return;

        _activeUnit.Tools.ShootStart();
        if (_activeUnit.Tools.ActiveTool.UsesForce)
            _forceBar.gameObject.SetActive(true);
    }
    public void ShootEnd()
    {
        if (_teamChange || _activeUnit.Tools.ActiveTool == null) return;
        if (_currentSpecials <= 0) return;

        if (_activeUnit.Tools.ShootEnd())
            UpdateUiChildren(_specialsHolder, --_currentSpecials, 0);

        _forceBar.gameObject.SetActive(false);
    }
    private void DuplicateFirstChild(Transform content, int amount)
    {
        GameObject uiRef = content.GetChild(0).gameObject;
        for (int i = content.childCount; i < amount; i++)
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
        ClearPlayers();
        _gameManager.GameOver(team);
    }
    public void ClearPlayers()
    {
        for (int t = Teams.Count - 1; t >= 0; t--)
        {
            for (int u = Teams[t].Units.Count - 1; u >= 0; u--)
            {
                Destroy(Teams[t].Units[u].gameObject);
                Teams[t].Units.RemoveAt(u);
            }
        }
        Teams = new List<Team>();
        _activePlace = new(0, 0);
        _inputHandler.OnMovementStay = null;
        _inputHandler.OnRotationStay = null;
        _inputHandler.OnPassTurn = null;
        _inputHandler.OnShowMap = null;
        _inputHandler.OnShootStart = null;
        _inputHandler.OnShootEnd = null;
        _inputHandler.OnChangeTool = null;
        _followCam.Priority = -1;
    }
}


[System.Serializable]
public class Team : object
{
    public Team(string name, Color color)
    {
        Name = name;
        Color = color;
        Units = new List<Unit>();
    }
    public bool Active;
    public string Name;
    public Color Color;
    public List<Unit> Units;
}
