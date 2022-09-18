using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public int Teams;
    public int TeamSize;
    public int Moves;
    public int Specials;
    [Space]
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private ObjectGen _objectGen;
    [SerializeField] private CinemachineVirtualCamera _orbitCam;
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [Space]
    [SerializeField] private Transform _movesHolder;
    [SerializeField] private Transform _specialsHolder;

    private Movement _activeUnit;
    private Movement[,] _units;
    private Vector2Int _activePlace;

    private int _currentMoves;
    private int _currentSpecials;

    private Coroutine _continousMoveRoutine;
    private Vector2 moveDirection;
    private bool _cameraActive;
    private void Start()
    {
        _orbitCam.Priority = 1;
        _followCam.Priority = 0;
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_continousMoveRoutine == null)
                _continousMoveRoutine = StartCoroutine(ContinousMoving());
            moveDirection = (Vector2)context.ReadValueAsObject();
        }
        else if (context.canceled)
        {
            StopCoroutine(_continousMoveRoutine);
            _continousMoveRoutine = null;
        }
    }
    IEnumerator ContinousMoving()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (_currentMoves > 0 && _activeUnit.Move(moveDirection))
            {
                if (--_currentMoves == 0 && _currentSpecials == 0)
                {
                    Invoke(nameof(ChangeUnit), 1);
                    StopCoroutine(_continousMoveRoutine);
                }
                for (int i = 0; i < _specialsHolder.childCount; i++)
                {
                    _specialsHolder.GetChild(i).GetChild(0).gameObject.SetActive(i < _currentSpecials);
                }
                for (int i = 0; i < _movesHolder.childCount; i++)
                {
                    _movesHolder.GetChild(i).GetChild(0).gameObject.SetActive(i < _currentMoves);
                }
            }
        }
    }
    public void Look(InputAction.CallbackContext context)
    {
        if(context.performed && _cameraActive)
            _activeUnit.Rotation();
    }
    public void Primary(InputAction.CallbackContext context)
    {

    }
    public void Secondary(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _followCam.GetComponent<CinemachineInputProvider>().enabled = true;
            _cameraActive = true;
        }
        if (context.canceled)
        {
            _followCam.GetComponent<CinemachineInputProvider>().enabled = false;
            _cameraActive = false;
        }
    }
    public void SwitchWeapon(InputAction.CallbackContext context)
    {

    }
    public void PassTurn(InputAction.CallbackContext context)
    {
        if (context.started)
            ChangeUnit();
    }
    public void ChangeUnit()
    {
        _activeUnit.transform.GetChild(1).gameObject.SetActive(false);
        if (_activePlace.y + 1 >= TeamSize)
        {
            if (_activePlace.x + 1 >= Teams)
                _activePlace = new(0, 0);
            else
                _activePlace = new(_activePlace.x + 1, 0);
        }
        else
            _activePlace += new Vector2Int(0, 1);
        _activeUnit = _units[_activePlace.x, _activePlace.y];
        _activeUnit.transform.GetChild(1).gameObject.SetActive(true);
        _followCam.LookAt = _activeUnit.transform;
        _followCam.Follow = _activeUnit.transform;
        _activeUnit.CamTransform = _followCam.transform;
        _currentMoves = Moves;
        _currentSpecials = Specials;

        for (int i = 0; i < _specialsHolder.childCount; i++)
        {
            _specialsHolder.GetChild(i).GetChild(0).gameObject.SetActive(i < _currentSpecials);
        }
        for (int i = 0; i < _movesHolder.childCount; i++)
        {
            _movesHolder.GetChild(i).GetChild(0).gameObject.SetActive(i < _currentMoves);
        }
    }
    public IEnumerator SpawnPlayers()
    {
        List<GameObject> spawnPoints = new List<GameObject>();
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("Respawn"));
        int n = Teams * TeamSize;
        if (spawnPoints.Count < n)
        {
            Debug.Log("Asking for more respawn points");
            spawnPoints.AddRange(_objectGen.More("Spawner", n - spawnPoints.Count));
        }


        _units = new Movement[Teams, TeamSize];
        for (int team = 0; team < Teams; team++)
        {
            for (int unit = 0; unit < TeamSize; unit++)
            {
                int r = Random.Range(0, spawnPoints.Count);
                GameObject go = Instantiate(_unitPrefab, spawnPoints[r].transform.position, Quaternion.identity);
                Destroy(spawnPoints[r]);
                spawnPoints.RemoveAt(r);

                _units[team, unit] = go.GetComponent<Movement>();
                yield return new WaitForEndOfFrame();
            }
        }
        for (int i = spawnPoints.Count - 1; i >= 0; i--)
        {
            Destroy(spawnPoints[i]);
            spawnPoints.RemoveAt(i);
        }
        _activeUnit = _units[0, 0];
        _activePlace = new(0, 0);
        _activeUnit.transform.GetChild(1).gameObject.SetActive(true);
        _followCam.LookAt = _activeUnit.transform;
        _followCam.Follow = _activeUnit.transform;
        _followCam.m_Priority = 1;
        _orbitCam.Priority = 0;
        _activeUnit.CamTransform = _followCam.transform;
        _currentMoves = Moves;
        _currentSpecials = Specials;


        GameObject UiRef = _specialsHolder.GetChild(0).gameObject;
        for (int i = 1; i < Specials; i++)
            Instantiate(UiRef, _specialsHolder);
        for (int i = 0; i < _specialsHolder.childCount; i++)
        {
            _specialsHolder.GetChild(i).GetChild(0).gameObject.SetActive(i < _currentSpecials);
        }
        _specialsHolder.gameObject.SetActive(true);

        UiRef = _movesHolder.GetChild(0).gameObject;
        for (int i = 1; i < Moves; i++)
            Instantiate(UiRef, _movesHolder);
        for (int i = 0; i < _movesHolder.childCount; i++)
        {
            _movesHolder.GetChild(i).GetChild(0).gameObject.SetActive(i < _currentMoves);
        }
        _movesHolder.gameObject.SetActive(true);
    }
}
