using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackerRoomChanger : MonoBehaviour
{
    [SerializeField] Button[] roomButtons;
    LocalClientManager localClientManager;
    int startingRoomIndex;
    void Start()
    {
        SetupLocalClientManager();
        AssignRoomButtons();
    }
    private void SetupLocalClientManager()
    {
        localClientManager = FindObjectOfType<LocalClientManager>();
        localClientManager.SetHackerRoomChanger(this);
    }
    private void AssignRoomButtons()
    {
        for (int i = 0; i < roomButtons.Length; i++)
        {
            int roomIndex = i;
            roomButtons[i].onClick.AddListener(() => localClientManager.RoomButtonClick(roomIndex));
            roomButtons[i].onClick.AddListener(DisableAllButtons);
            roomButtons[i].interactable = false;
        }
    }

    private void DisableAllButtons()
    {
        foreach (Button button in roomButtons)
            button.interactable = false;
    }
 
    public void ReachedRoom(int roomReachedIndex)
    {
        for (int i = 0; i < roomButtons.Length; i++)
        {
            if (i == roomReachedIndex) continue;
            roomButtons[i].interactable = true;
        }
    }
}
