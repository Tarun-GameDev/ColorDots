using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Player[] allPlayers;
    [SerializeField]bool searchAtStart = true;

    public int noOfCheckReached = 0;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    private void Start()
    {
        if(searchAtStart)
            allPlayers = FindObjectsOfType<Player>();
    }

    #region UNDO Feature
    public void ActivateUNDO()
    {
        foreach (Player _player in allPlayers)
        {
            if(_player.isActiveAndEnabled)
                _player.ReverseOn();
        }
    }

    public void DeActivateUNDO()
    {
        foreach (Player _player in allPlayers)
        {
            if (_player.isActiveAndEnabled)
                _player.ReverseOff();
        }
    }
    #endregion


    #region Erase Feature
    public void ActivateErase()
    {
        foreach(Player _player in allPlayers)
        {
            if (_player.isActiveAndEnabled)
                _player.EraseOn();
        }
    }

    public void DeactivateErase()
    {
        foreach (Player _player in allPlayers)
        {
            if (_player.isActiveAndEnabled)
                _player.EraseOff();
        }
    }
    #endregion

    #region Phantom Feature
    public void ActivatePhantom()
    {
        foreach (Player _player in allPlayers)
        {
            if (_player.isActiveAndEnabled)
                _player.PhantomOn();
        }
    }

    public void DeactivatePhantom()
    {
        foreach (Player _player in allPlayers)
        {
            if (_player.isActiveAndEnabled)
                _player.PhantomOff();
        }
    }
    #endregion

    #region Game Finish Mechanism
    public void PlayerCheckPointReached()
    {
        noOfCheckReached++;
        CheckForGameFinish();
    }

    public void CheckForGameFinish()
    {
        if (noOfCheckReached == allPlayers.Length)
        {
            UiManager.instance.LevelComplete();
        }
    }
    #endregion
}
