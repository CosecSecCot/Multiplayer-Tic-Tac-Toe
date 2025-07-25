using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 3.0f;

    [SerializeField] private GameObject crossPrefab;
    [SerializeField] private GameObject circlePrefab;

    private void Start()
    {
        // here we handle events in Start() to avoid NullReferenceException,
        // as we initialize GameManager.Instance in Awake method.
        GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
    }

    private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionArgs e)
    {
        // since only server can spawn objects, we use Rpc.
        SpawnObjectRpc(e.x, e.y, e.playerType);
    }

    [Rpc(SendTo.Server)]
    public void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {
        // cannot compare the GameManager.Instance.GetPlayerType()
        // as this is rpc function, it always runs on server, so GameManager.Instance.GetPlayerType()
        // will always return player type of server.
        Debug.Log("[SERVER] Spawned Object at: (" + x + ", " + y + ")");
        GameObject prefab;
        switch (playerType)
        {
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
            default:
                prefab = null;
                break;
        }

        if (prefab == null)
        {
            Debug.LogError("Error spawning object: playerType is None!");
        }
        else
        {
            GameObject spawnedObject = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
            spawnedObject.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    private Vector2 GetGridWorldPosition(int x, int y)
    {
        return new Vector2((y - 1) * GRID_SIZE, (1 - x) * GRID_SIZE);
    }
}
