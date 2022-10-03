using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private List<UpDownValue> Objects;
    [Header("InGame")]
    [SerializeField] private View _hudView;
    [Header("Settings")]
    [SerializeField] private View _settingsView;
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
        for (int i = 0; i < _gameManager.Spawnables.Length; i++)
        {
            if (Objects.Count > i)
            {
                Objects[i].ValueName = _gameManager.Spawnables[i].Name;
                Objects[i].CurrentValue = _gameManager.Spawnables[i].BaseAmount;
            }
            else
            {
                GameObject go = Instantiate(Objects[0].gameObject, Objects[0].transform.parent);
                UpDownValue udv = go.GetComponent<UpDownValue>();
                udv.ValueName = _gameManager.Spawnables[i].Name;
                udv.CurrentValue = _gameManager.Spawnables[i].BaseAmount;
                Objects.Add(udv);
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
    public void OpenSettings()
    {
        //open the menu
    }
    public void CloseSettings()
    {
        //close and save stuff i guess
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
        for (int i = 0; i < Objects.Count; i++)
            _gameManager.ObjectGen.SpawnOnGround(_gameManager.Spawnables[i], Objects[i].CurrentValue);
        for (int i = 0; i < _playerSettings.Count; i++)
            _gameManager.SetTeam(_playerSettings[i].Name, _playerSettings[i].Color);
        _setupView.Hide();
        _hudView.Show();
        _gameManager.StartGame();
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
