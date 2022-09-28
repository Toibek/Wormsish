using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] View _mainView;
    [Header("Setup")]
    [SerializeField] View _setupView;
    [SerializeField] View _mapView;
    [Space]
    [SerializeField] List<PlayerSettings> PlayerSettings;
    [SerializeField] List<Button> AddPlayerButtons;
    [Space]
    [SerializeField] UpDownValue Units;
    [SerializeField] UpDownValue Moves;
    [SerializeField] UpDownValue Specials;
    [SerializeField] UpDownValue Pickup;
    [Space]
    [SerializeField] UpDownValue Barrels;
    [SerializeField] UpDownValue Mines;
    [SerializeField] UpDownValue Healthpacks;
    [Header("InGame")]
    [SerializeField] View _hudView;
    [Header("Settings")]
    [SerializeField] View _settingsView;

    GameManager _gameManager;
    IslandManager _islandManager;


    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        _islandManager = GameObject.FindGameObjectWithTag("Island").GetComponent<IslandManager>();
    }
    #region Main Menu
    public void StartSetup()
    {
        _mainView.Hide();
        _islandManager.GenerateIsland();
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
    public void BackToMain()
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
    public void RegenerateIsland()
    {
        _islandManager.GenerateIsland();
    }
    public void StartGame()
    {
        _gameManager.Units = Units.CurrentValue;
        _gameManager.Moves = Moves.CurrentValue;
        _gameManager.Specials = Specials.CurrentValue;
        _gameManager.PickupFreq = Pickup.CurrentValue;

        _gameManager.Barrels = Barrels.CurrentValue;
        _gameManager.Mines = Mines.CurrentValue;
        _gameManager.Healthpacks = Healthpacks.CurrentValue;

        _setupView.Hide();
        _gameManager.StartGame();
        _hudView.Show();
    }
    #endregion

}
