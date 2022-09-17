using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public int Teams;
    public int TeamSize;
    [SerializeField] private GameObject _UnitPrefab;
    [SerializeField] private ObjectGen _objectGen;
    [SerializeField] private CinemachineVirtualCamera _OrbitCam;
    [SerializeField] private CinemachineVirtualCamera _FollowCam;

    private Movement _activeUnit;
    private Movement[,] _units;
    private Vector2Int _activePlace;

    private void Start()
    {
        _OrbitCam.Priority = 1;
        _FollowCam.Priority = 0;
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Performed: " + context.ReadValueAsObject());
            _activeUnit.Move((Vector2)context.ReadValueAsObject());
        }
        else if (context.canceled)
        {
            Debug.Log("Canceled: " + context.ReadValueAsObject());
        }
    }
    public void Look(InputAction.CallbackContext context)
    {

    }
    public void Fire(InputAction.CallbackContext context)
    {

    }
    public void SwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _activeUnit.transform.GetChild(1).gameObject.SetActive(false);
            if (_activePlace.y + 1 >= TeamSize)
            {
                if (_activePlace.x + 1 >= Teams)
                    _activePlace = new(0, 0);
                else
                    _activePlace = new(_activePlace.x+1, 0);
            }
            else
                _activePlace += new Vector2Int(0, 1);
            _activeUnit = _units[_activePlace.x, _activePlace.y];
            _activeUnit.transform.GetChild(1).gameObject.SetActive(true);
            _FollowCam.LookAt = _activeUnit.transform;
            _FollowCam.Follow = _activeUnit.transform;
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
                GameObject go = Instantiate(_UnitPrefab, spawnPoints[r].transform.position, Quaternion.identity);
                Destroy(spawnPoints[r]);
                spawnPoints.RemoveAt(r);

                _units[team, unit] = go.GetComponent<Movement>();
                yield return new WaitForEndOfFrame();
            }
        }
        _activeUnit = _units[0, 0];
        _activePlace = new(0, 0);
        _activeUnit.transform.GetChild(1).gameObject.SetActive(true);
        _FollowCam.LookAt = _activeUnit.transform;
        _FollowCam.Follow = _activeUnit.transform;
        _FollowCam.m_Priority = 1;
        _OrbitCam.Priority = 0;
    }
}
