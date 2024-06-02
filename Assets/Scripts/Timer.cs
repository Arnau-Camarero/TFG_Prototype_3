using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{
    public float countdownTime = 20f; // in seconds
    public bool LevelCompleted = false;
    public TextMeshProUGUI displayText;
    private int requiredPlayers = 2; // Set this to the number of players required to start the countdown
    private int connectedPlayers = 0;
    private bool countdownStarted = false;
    private NetworkVariable<float> networkedCountdownTime = new NetworkVariable<float>(20f);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            networkedCountdownTime.Value = countdownTime;
        }

        networkedCountdownTime.OnValueChanged += OnCountdownTimeChanged;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        
        UpdateDisplayText(networkedCountdownTime.Value);
    }

    private void OnClientConnected(ulong clientId)
    {
        connectedPlayers++;
        CheckAndStartCountdown();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        connectedPlayers--;
    }

    private void CheckAndStartCountdown()
    {
        if (connectedPlayers >= requiredPlayers && !countdownStarted)
        {
            countdownStarted = true;
            if (IsServer)
            {
                StartCoroutine(StartCountdown());
            }
        }
    }

    private void OnCountdownTimeChanged(float oldValue, float newValue)
    {
        UpdateDisplayText(newValue);

        if (newValue <= 0 && !LevelCompleted)
        {
            LevelCompleted = true;
            DisableRemoveObjects();
        }
    }

    private IEnumerator StartCountdown()
    {
        while (networkedCountdownTime.Value > 0)
        {
            yield return new WaitForSeconds(1f);
            networkedCountdownTime.Value -= 1f;
        }

        LevelCompleted = true;
        networkedCountdownTime.Value = 0;
        UpdateDisplayText(networkedCountdownTime.Value);
    }

    private void UpdateDisplayText(float time)
    {
        if (displayText != null)
        {
            if (time <= 0)
            {
                displayText.text = "Completed! ESCAPE!";
            }
            else
            {
                displayText.text = time.ToString("F0");
            }
        }
        else
        {
            displayText = GameObject.FindGameObjectWithTag("CountDown").GetComponent<TextMeshProUGUI>();
            Debug.LogWarning("DisplayText is not assigned in the inspector.");
        }
    }

    public void DisableRemoveObjects(){
        GameObject[] removeObj = GameObject.FindGameObjectsWithTag("Remove");
        foreach (GameObject obj in removeObj)
        {
            obj.SetActive(false);
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        networkedCountdownTime.OnValueChanged -= OnCountdownTimeChanged;
    }
}
