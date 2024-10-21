using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkMessagesBridge : NetworkBehaviour
{
    #region ClientOnly

    LocalClientManager localClientManager;
    private void Start()
    {
        if (IsServer) return;
        localClientManager = FindObjectOfType<LocalClientManager>();
        localClientManager.ConnectionSuccess(this);
    }
    public void ChangeRoom(int index)
    {
        ChangeRoomServerRpc(index);
    }
    #endregion

    #region ServerOnly

    NPCMovement hacker;
    public void SetHackerOnSceneStart(NPCMovement p_hacker)
    {
        hacker = p_hacker;
        OnHackerReachedRoomClientRpc(hacker.CurrentRoom);
    }

    #endregion

    #region LaunchGame

    Button launchGameButton;
    public void AssignLaunchButton(Button p_launchButton)
    {
        launchGameButton = p_launchButton;
        launchGameButton.onClick.AddListener(LaunchGame);
    }
    private void LaunchGame()
    {
        LaunchGameFromServer();
        LaunchGameClientRpc();
    }
    [ClientRpc]
    public void LaunchGameClientRpc()
    {
        localClientManager.HideIntroCanvas();
        LaunchGameSceneClientRpc();

    }
    public void LaunchGameFromServer()
    {
        LaunchGameScene();
    }
    private void LaunchGameScene()
    {
        SceneManager.LoadSceneAsync("SecurityCCTV", LoadSceneMode.Additive);

    }
    [ClientRpc]
    public void LaunchGameSceneClientRpc()
    {
        SceneManager.LoadSceneAsync("Hacker", LoadSceneMode.Additive);
    }

    #endregion

    #region RoomChange

    [ServerRpc(RequireOwnership =false)]
    private void ChangeRoomServerRpc(int newRoomIndex)
    {
        hacker.GoToRoomForHacker(newRoomIndex);
    }
    public void OnHackerReachedRoom(int roomIndex)
    {
        OnHackerReachedRoomClientRpc(roomIndex);
    }
    [ClientRpc]
    public void OnHackerReachedRoomClientRpc(int roomIndex)
    {
        localClientManager.OnRoomReached(roomIndex);
    }

    #endregion
}
