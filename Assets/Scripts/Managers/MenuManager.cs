using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] private View _mainView;
    [Header("Setup")]
    [SerializeField] private View _setupView;
    [SerializeField] private View _mapView;
    [Space]
    [SerializeField] private List<PlayerSettings> _playerSettings;
    [SerializeField] private string _randomNames;
    [SerializeField] private Button _removePlayerButton;
    [SerializeField] private Button _addPlayerButton;
    [Space]
    [SerializeField] private UpDownValue _units;
    [SerializeField] private UpDownValue _moves;
    [SerializeField] private UpDownValue _specials;
    [SerializeField] private UpDownValue _pickup;
    [Space]
    [SerializeField] private List<UpDownValue> _objects;
    [SerializeField] private List<StateToggle> _pickups;
    [Header("InGame")]
    [SerializeField] private View _hudView;
    [SerializeField] private View _pauseView;
    [Header("Pause")]
    [Header("GameOver")]
    [SerializeField] private View _gameOverView;
    [SerializeField] private TMP_Text WinnerText;


    private GameManager _gameManager;
    private string[] _randomNameArray;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        _randomNameArray = _randomNames.Split(' ');
        for (int i = 0; i < _playerSettings.Count; i++)
            _playerSettings[i].Name = _randomNameArray[Random.Range(0, _randomNameArray.Length)];

        for (int i = 0; i < _gameManager.Spawnables.Count; i++)
        {
            if (_objects.Count > i)
            {
                _objects[i].ValueName = _gameManager.Spawnables[i].Name;
                _objects[i].CurrentValue = _gameManager.Spawnables[i].BaseAmount;
            }
            else
            {
                GameObject go = Instantiate(_objects[0].gameObject, _objects[0].transform.parent);
                UpDownValue udv = go.GetComponent<UpDownValue>();
                udv.ValueName = _gameManager.Spawnables[i].Name;
                udv.CurrentValue = _gameManager.Spawnables[i].BaseAmount;
                _objects.Add(udv);
            }
        }

        for (int i = 0; i < _gameManager.AllTools.Count; i++)
        {
            if (_pickups.Count > i)
            {
                _pickups[i].Content.sprite = _gameManager.AllTools[i].ToolIcon;
                _pickups[i].CurrentState = 1;
            }
            else
            {
                GameObject go = Instantiate(_pickups[0].gameObject, _pickups[0].transform.parent);
                StateToggle stog = go.GetComponent<StateToggle>();
                stog.Content.sprite = _gameManager.AllTools[i].ToolIcon;
                _pickups.Add(stog);
                _pickups[i].CurrentState = 1;
            }
        }
        _removePlayerButton.interactable = false;
    }

    #region Main Menu
    public void StartSetup()
    {
        _mainView.Hide();
        _gameManager.IslandGen.GenerateIfEmpty();
        _setupView.Show();
    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Setup
    public void BackFromSetup()
    {
        _setupView.Hide();
        _mainView.Show();
    }
    public void ToggleMapView()
    {
        if (_mapView.gameObject.activeInHierarchy)
        {
            _mapView.Hide();
            _setupView.Show();
        }
        else
        {
            _setupView.Hide();
            _mapView.Show();
        }
    }
    public void AddPlayer()
    {
        GameObject go = Instantiate(_playerSettings[0].gameObject, _playerSettings[0].transform.parent);
        PlayerSettings ps = go.GetComponent<PlayerSettings>();
        ps.Name = _randomNameArray[Random.Range(0, _randomNameArray.Length)];
        _playerSettings.Add(ps);

        if (_playerSettings.Count >= _gameManager.MaxPlayers)
            _addPlayerButton.interactable = false;
        if (_playerSettings.Count == _gameManager.MinPlayers + 1)
            _removePlayerButton.interactable = true;
    }
    public void RemovePlayer()
    {
        GameObject go = _playerSettings[_playerSettings.Count - 1].gameObject;
        _playerSettings.RemoveAt(_playerSettings.Count - 1);
        Destroy(go);

        if (_playerSettings.Count == _gameManager.MaxPlayers - 1)
            _addPlayerButton.interactable = true;
        if (_playerSettings.Count == _gameManager.MinPlayers)
            _removePlayerButton.interactable = false;
    }
    public void RegenerateIsland()
    {
        _gameManager.IslandGen.GenerateIsland();
    }
    public void StartGame()
    {
        _gameManager.Units = _units.CurrentValue;
        _gameManager.Moves = _moves.CurrentValue;
        _gameManager.Specials = _specials.CurrentValue;
        _gameManager.PickupPerTurn = _pickup.CurrentValue;
        for (int i = 0; i < _objects.Count; i++)
            _gameManager.ObjectGen.SpawnOnGround(_gameManager.Spawnables[i], _objects[i].CurrentValue);
        for (int i = 0; i < _playerSettings.Count; i++)
            _gameManager.SetTeam(_playerSettings[i].Name, _playerSettings[i].Color);
        for (int i = 0; i < _pickups.Count; i++)
        {
            if (_pickups[i].CurrentState == 1) _gameManager.DroppableTools.Add(Instantiate(_gameManager.AllTools[i]));
            else if (_pickups[i].CurrentState == 2) _gameManager.InfiniteTools.Add(Instantiate(_gameManager.AllTools[i]));
        }


        _setupView.Hide();
        _hudView.Show();
        _gameManager.StartGame();
    }
    #endregion

    #region inGame
    public void Pause()
    {
        if (!_hudView.gameObject.activeInHierarchy) return;
        _gameManager.Paused = true;
        Debug.Log(_gameManager.Paused);
        _pauseView.Show();
    }
    public void Unpause()
    {
        if (!_hudView.gameObject.activeInHierarchy) return;
        _gameManager.Paused = false;
        _pauseView.Hide();
    }
    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!_hudView.gameObject.activeInHierarchy) return;
            if (_pauseView.gameObject.activeInHierarchy)
                Unpause();
            else
                Pause();
        }
    }
    public void CancelGame()
    {
        _gameManager.PlayerManager.ClearPlayers();
        _gameManager.ObjectGen.ClearObjects();
        _gameManager.Paused = false;
        _pauseView.Hide();
        _hudView.Hide();
        _setupView.Show();
    }


    #endregion

    #region GameOver

    public void GameOver(Team winner)
    {
        _hudView.Hide();
        _gameOverView.Show();
        WinnerText.text = winner.Name;
        WinnerText.color = winner.Color;
    }

    public void BackFromGameOver()
    {
        _gameOverView.Hide();
        _mainView.Show();
    }

    #endregion
}
