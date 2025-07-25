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
    private PlayerType currentPlayablePlayerType;

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

        if (IsServer)
        {
            currentPlayablePlayerType = PlayerType.Cross;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("Grid (" + x + ", " + y + ") Clicked!");
        // this function needs to be run on server because,
        // if it runs locally, client will not ever get a chance to play
        // since currentPlayablePlayerType is changed only on client. (please observe it by dry-run)
        if (playerType != currentPlayablePlayerType)
        {
            return;
        }

        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionArgs
        {
            x = x,
            y = y,
            playerType = playerType
        });

        switch (currentPlayablePlayerType)
        {
            case PlayerType.Cross:
                currentPlayablePlayerType = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType = PlayerType.Cross;
                break;
            default:
                Debug.LogError("currentPlayablePlayerType is None!");
                break;
        }
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }
}
