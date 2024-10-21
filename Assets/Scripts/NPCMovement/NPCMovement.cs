using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    [SerializeField] bool isHacker = false;
    NetworkMessagesBridge networkMessagesBridge;
    private Room[] rooms;
    private float moveSpeed = 2f;
    private float stoppingDistance = 0.5f;
    private Vector2 idleTimeRange = new Vector2(1f, 3f);
    private Vector2 movesInRoomRange = new Vector2(3, 10);
    private int maxIdleStateInRow = 3;

    private NavMeshAgent navAgent;
    private Animator animator;
    private RoomPoint currentRoomPoint;

    private int movesInRoomLeft;
    private int currentIdleStateInRow;
    private int currentRoomIndex;
    public int CurrentRoom => currentRoomIndex;

    private bool isHackerChangingRoom;

    private bool isIdle = true;
    private const string SpeedParameter = "Speed";

    private void Awake()
    {
        SetupComponents();
        InitializeNavMeshAgent();
        ResetState();
    }

    private void SetupComponents()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }
    private void SetupNetworkValuesForHacker()
    {
        networkMessagesBridge = FindObjectOfType<NetworkMessagesBridge>();
        networkMessagesBridge.SetHackerOnSceneStart(this);
    }

    private void InitializeNavMeshAgent()
    {
        navAgent.speed = moveSpeed;
        navAgent.stoppingDistance = stoppingDistance;
    }

   

    private void Start()
    {
        SetRandomMovesInRoom();
        StartCoroutine(CO_IdleState());
    }
    private void ResetState()
    {
        currentIdleStateInRow = 0;
    }


    public void Setup(NPCsValues nPCsValues)
    {
        rooms = nPCsValues.rooms;
        moveSpeed = nPCsValues.moveSpeed;
        stoppingDistance = nPCsValues.stoppingDistance;
        idleTimeRange = nPCsValues.idleTimeRange;
        movesInRoomRange = nPCsValues.movesInRoomRange;
        maxIdleStateInRow = nPCsValues.maxIdleStateInRow;
    }
    public void SpawnInRoom(SpawnPoint spawnPoint)
    {
        transform.position = spawnPoint.position;
        currentRoomIndex = spawnPoint.roomIndex;
        if (isHacker)
            SetupNetworkValuesForHacker();
    }
    bool hasOrder;
    int hackerNewTargetRoom;
    public void GoToRoomForHacker(int targetRoom)
    {
        hackerNewTargetRoom = targetRoom;
        if(isIdle)
            LaunchGoToRoom();
        else
            hasOrder= true;
    }
    private void LaunchGoToRoom()
    {
        hasOrder = false;
        isHackerChangingRoom = true;
        currentRoomIndex = hackerNewTargetRoom;
        StopAllCoroutines();
        StartCoroutine(CO_GoToRoom());
    }

    private IEnumerator CO_GoToRoom()
    {

        if (TryFindUnoccupiedPoint())
            MoveToNextPoint();
        else
        {
            isIdle = true;
            yield return new WaitForSeconds(idleTimeRange.x);
            isIdle = false;
            StartCoroutine(CO_GoToRoom());
        }
    }
    private void Update()
    {
        UpdateAnimatorSpeed();
    }

    private void UpdateAnimatorSpeed()
    {
        float speed = navAgent.velocity.magnitude / moveSpeed;
        animator.SetFloat(SpeedParameter, speed);
    }

    private void LateUpdate()
    {
        if (isIdle) return;

        if (HasReachedDestination())
        {
            if(isHacker)
            {
                if (isHackerChangingRoom)
                {
                    isHackerChangingRoom = false;
                    networkMessagesBridge.OnHackerReachedRoom(currentRoomIndex);
                }
                else if(hasOrder)
                {
                    LaunchGoToRoom();
                }
            }
            else
            {
                movesInRoomLeft--;
            }
            StartCoroutine(CO_IdleState());
        }
    }

    private bool HasReachedDestination()
    {
        return !navAgent.pathPending &&
               navAgent.remainingDistance <= navAgent.stoppingDistance &&
               (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f);
    }
    private IEnumerator CO_IdleState()
    {
        isIdle = true;
        navAgent.isStopped = true;

        float idleTime = Random.Range(idleTimeRange.x, idleTimeRange.y);
        yield return new WaitForSeconds(idleTime);
        NextMove();
    }

    private void SetRandomMovesInRoom()
    {
        movesInRoomLeft = Random.Range((int)movesInRoomRange.x, (int)movesInRoomRange.y);
    }

    private void SwitchRoom()
    {
        currentRoomPoint.isOccupied = false;
        currentRoomIndex = GetLeastOccupiedRoomIndex();
        SetRandomMovesInRoom();
        MoveToNextPoint();
    }
    private void NextMove()
    {
        if (!isHacker) currentIdleStateInRow++;
        if (movesInRoomLeft > 0)
        {
            if (TryFindUnoccupiedPoint())
                MoveToNextPoint();
            else if (currentIdleStateInRow < maxIdleStateInRow)
                StartCoroutine(CO_IdleState());
            else
                SwitchRoom();
        }
        else
        {
            SwitchRoom();
        }
    }

    private bool TryFindUnoccupiedPoint()
    {
        RoomPoint[] roomPoints = rooms[currentRoomIndex].GetShuffledRoomPoints();
        foreach (RoomPoint point in roomPoints)
        {
            if (!point.isOccupied)
            {
                if (currentRoomPoint != null)
                    currentRoomPoint.isOccupied = false;

                currentRoomPoint = point;
                return true;
            }
        }
        return false;
    }

    private void MoveToNextPoint()
    {
        isIdle = false;
        currentIdleStateInRow = 0;

        currentRoomPoint.isOccupied = true;
        navAgent.SetDestination(currentRoomPoint.position);
        navAgent.isStopped = false;
    }


    
    private int GetLeastOccupiedRoomIndex()
    {
        int leastOccupiedIndex = -1;
        int maxFreePoints = -1;

        for (int i = 0; i < rooms.Length; i++)
        {
            if (i == currentRoomIndex) continue;

            int freePoints = rooms[i].GetRoomFreePointsCount();
            if (freePoints > maxFreePoints)
            {
                maxFreePoints = freePoints;
                leastOccupiedIndex = i;
            }
        }

        return leastOccupiedIndex;
    }
}
