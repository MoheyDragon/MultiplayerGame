using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NpcsManager : MonoBehaviour
{
    [SerializeField] NPCsValues npcsValues;
    SpawnPoint[] roomsSpawnPoints;
    private void Start()
    {
        ArrangeNpcsPositions();
        SetupNpcs();
    }
    private void SetupNpcs()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            NPCMovement newNpc = transform.GetChild(i).GetComponent<NPCMovement>();
            newNpc.Setup(npcsValues);
            newNpc.SpawnInRoom(roomsSpawnPoints[i]);
        }
    }
    private void ArrangeNpcsPositions()
    {
        SetRoomSpawnPoints();
        roomsSpawnPoints = GetShuffledSpawnPoints();
    }
    private void SetRoomSpawnPoints()
    {
        int roomsSpawnPointsCount = 0;
        for (int i = 0; i < npcsValues.rooms.Length; i++)
            roomsSpawnPointsCount += npcsValues.rooms[i].SpawnPointsCount;
        roomsSpawnPoints = new SpawnPoint[roomsSpawnPointsCount];
        int currentPointIndex = 0;
        for (int i = 0; i < npcsValues.rooms.Length; i++)
        {
            for (int j = 0; j < npcsValues.rooms[i].SpawnPointsCount; j++)
            {
                roomsSpawnPoints[currentPointIndex] = new SpawnPoint();

                roomsSpawnPoints[currentPointIndex].roomIndex = i;
                roomsSpawnPoints[currentPointIndex].position=npcsValues.rooms[i].GetSpawnPoints[j].position;
                currentPointIndex++;
            }
        }
    }
    private SpawnPoint[] GetShuffledSpawnPoints()
    {
        SpawnPoint[] shuffledPoints = roomsSpawnPoints.ToArray();

        for (int i = shuffledPoints.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            SpawnPoint temp = shuffledPoints[i];
            shuffledPoints[i] = shuffledPoints[randomIndex];
            shuffledPoints[randomIndex] = temp;
        }

        return shuffledPoints;
    }
}
