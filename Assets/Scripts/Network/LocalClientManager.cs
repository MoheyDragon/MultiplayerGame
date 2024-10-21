using UnityEngine;
using UnityEngine.UI;

public class LocalClientManager:MonoBehaviour
{
    [SerializeField] GameObject introCanvas;
    NetworkMessagesBridge networkMessagesBridge;
    HackerRoomChanger hackerRoomChanger;

    #region Connecting

    [SerializeField] GameObject loadingAnimation;
    [SerializeField] GameObject connectionFailureWarning;
    [SerializeField] GameObject waitingForHostWarning;
    public void PlayLoadAnimation()
    {
        loadingAnimation.SetActive(true);
        connectionFailureWarning.SetActive(false);
    }
    public void ConnectionFailure()
    {
        connectionFailureWarning.SetActive(true);
        loadingAnimation.SetActive(false);
    }
    public void ConnectionSuccess(NetworkMessagesBridge p_networkMessagesBridge)
    {
        networkMessagesBridge = p_networkMessagesBridge;

        loadingAnimation.SetActive(false);
        waitingForHostWarning.SetActive(true);
    }

    #endregion

    #region CallsFromToServer
    public void OnRoomReached(int roomReachedIndex)
    {
        hackerRoomChanger.ReachedRoom(roomReachedIndex);
    }

    public void HideIntroCanvas()
    {
        introCanvas.SetActive(false);
    }


    #endregion

    #region LocalCalls

    public void SetHackerRoomChanger(HackerRoomChanger p_hackerRoomChanger)
    {
        hackerRoomChanger = p_hackerRoomChanger;
    }
    public void RoomButtonClick(int roomIndex)
    {
        networkMessagesBridge.ChangeRoom(roomIndex);
    }

    #endregion

}
