using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionArgs> OnClickedOnGridPosition;
    public class OnClickedOnGridPositionArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }

    private PlayerType localPlayerType;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManagers are not allowed!");
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }
    }

    public void ClickedOnGridPosition(int x, int y)
    {
        Debug.Log("Grid (" + x + ", " + y + ") Clicked!");
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionArgs
        {
            x = x,
            y = y,
            playerType = localPlayerType
        });
    }

}
