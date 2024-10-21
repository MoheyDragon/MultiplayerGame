using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkLobby : NetworkBehaviour
{
    [Space]
    public bool isServer;
    [Header("Server")]
    [Space]
    [SerializeField] GameObject NetworkMessagesBridge;
    [SerializeField] Button launchGameButton;
    [SerializeField] GameObject waitingForPlayerToJoin;
    [SerializeField] GameObject ready;


    [Space(2)]

    [Header("Client")]
    [Space]
    [SerializeField] LocalClientManager localClientManager;
    [SerializeField] Button joinButton;
    [SerializeField] int connectionTries = 3;
    [SerializeField] float connectionTimeOut = 5;
    WaitForSeconds connectionAttemptWaitTime;


    private void Start()
    {
        if (isServer)
            InitiateServer();
        else
            InitiateClient();
    }
    private void InitiateServer()
    {
        HostGame();
        launchGameButton.interactable = false;
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        waitingForPlayerToJoin.SetActive(true);
        ready.SetActive(false);
        NetworkMessagesBridge bridge = Instantiate(NetworkMessagesBridge).GetComponent<NetworkMessagesBridge>();
        bridge.GetComponent<NetworkObject>().Spawn();
        bridge.AssignLaunchButton(launchGameButton);

    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        launchGameButton.interactable=true;
        waitingForPlayerToJoin.SetActive(false);
        ready.SetActive(true);
    }

    

    private void HostGame()
    {
        NetworkManager.Singleton.StartServer();
    }
    private void InitiateClient()
    {
        joinButton.onClick.AddListener(JoinGame);
    }
    public void JoinGame()
    {
        StartCoroutine(CO_TryToConnect());
    }
    private void CheckConnection(ulong clientId)
    {
        Debug.Log("Client connected with ID: " + clientId);
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            isConnected = true;
        }
    }
    bool isConnected;
    IEnumerator CO_TryToConnect()
    {
        Debug.Log("Attempting to connect as a client...");
        localClientManager.PlayLoadAnimation();
        joinButton.interactable = false;
        isConnected = false;

        NetworkManager.Singleton.OnClientConnectedCallback -= CheckConnection;
        NetworkManager.Singleton.OnClientConnectedCallback += CheckConnection;

        for (int i = 0; i < connectionTries; i++)
        {
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client started. Waiting for connection...");
                float timeElapsed = 0f;

                while (!isConnected && timeElapsed < connectionTimeOut)
                {
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                if (isConnected)
                {
                    Debug.Log("Connected successfully.");
                    yield break;
                }
                else
                {
                    Debug.LogWarning("Failed to connect within timeout.");
                }
            }
            else
            {
                Debug.LogWarning("Failed to start client. Retrying...");
            }

            // Shut down before retrying to ensure clean state
            NetworkManager.Singleton.Shutdown();
            yield return connectionAttemptWaitTime;
        }

        Debug.LogError("Failed to connect after all attempts.");
        OnConnectionFailure();
    }
    private void OnConnectionFailure()
    {
        joinButton.interactable = true;
        localClientManager.ConnectionFailure();
    }
}
