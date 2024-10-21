using System.Linq;
using UnityEngine;

public class Room:MonoBehaviour
{
    [SerializeField] private Transform wayPointsParent;
    [SerializeField] private Transform spawnPointsParent;
    RoomPoint[] roomPoints;
    Transform[] spawnPoints;

    #region Initialization

    private void Awake()
    {
        AssignWayPoints();
        AssignSpawnPoints();
    }
    private void AssignWayPoints()
    {
        int pointsCount = wayPointsParent.childCount;
        roomPoints = new RoomPoint[pointsCount];
        for (int i = 0; i < pointsCount; i++)
        {
            roomPoints[i] = new RoomPoint();
            roomPoints[i].isOccupied = false;
            roomPoints[i].position = wayPointsParent.GetChild(i).transform.position;
            Destroy(wayPointsParent.GetChild(i).GetComponent<MeshRenderer>());
            Destroy(wayPointsParent.GetChild(i).GetComponent<MeshFilter>());
        }
    }
    private void AssignSpawnPoints()
    {
        int pointsCount = spawnPointsParent.childCount;
        spawnPoints = new Transform[pointsCount];
        for (int i = 0; i < pointsCount; i++)
        {
            spawnPoints[i] = spawnPointsParent.GetChild(i); 
            Destroy(spawnPointsParent.GetChild(i).GetComponent<MeshRenderer>());
            Destroy(spawnPointsParent.GetChild(i).GetComponent<MeshFilter>());
        }
    }

    #endregion

    #region RoomPointsPublicCalls

    public RoomPoint[] GetShuffledRoomPoints()
    {
        RoomPoint[] shuffledPoints = roomPoints.ToArray();

        for (int i = shuffledPoints.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            RoomPoint temp = shuffledPoints[i];
            shuffledPoints[i] = shuffledPoints[randomIndex];
            shuffledPoints[randomIndex] = temp;
        }

        return shuffledPoints;
    }

    public int GetRoomFreePointsCount()
    {
        int freePoints = 0;
        for (int i = 0; i < roomPoints.Length; i++)
        {
            if (!roomPoints[i].isOccupied)
                freePoints++;
        }
        return freePoints;
    }

    public int SpawnPointsCount => spawnPoints.Length;
    public Transform[] GetSpawnPoints => spawnPoints;
    #endregion
}
