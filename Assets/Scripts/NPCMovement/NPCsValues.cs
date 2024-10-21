using UnityEngine;

[System.Serializable]
public class NPCsValues
{
    public Room[] rooms;
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.5f;
    public Vector2 idleTimeRange = new Vector2(1f, 3f);
    public Vector2 movesInRoomRange = new Vector2(3, 10);
    public int maxIdleStateInRow = 3;
}
