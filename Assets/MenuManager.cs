using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Doozy;

public class MenuManager : MonoBehaviour
{
    [SerializeField] PlayfabManager _pfManager;
    [Header("Login")]
    [SerializeField] TMP_InputField loginName;
    [Header("MainMenu")]
    [SerializeField] Button HostButton;
    [SerializeField] Button SocialButton;
    public void PlayOnline()
    {
        _pfManager.Login(loginName.text);
        HostButton.interactable = true;
        SocialButton.interactable = true;
    }
    public void PlayOffline()
    {
        HostButton.interactable = false;
        SocialButton.interactable = false;
    }
}
