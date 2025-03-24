using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Score : NetworkBehaviour
{
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject joinCodeInputField;
    public TextMeshProUGUI displayText;
    public TextMeshProUGUI scoreText;
    private int requiredPlayers = 2; // Set this to the number of players required to start the countdown
    private int connectedPlayers = 0;
    private int objectiveScore = 2500;
    private NetworkVariable<int> score = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            score.Value = 0;
        }
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        UpdateScoreText(score.Value);
        HideUIElements();
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
        if (connectedPlayers >= requiredPlayers)
        {
            if (IsServer)
            {
                StartCoroutine(StartScore());
            }
        }
    }

    private IEnumerator StartScore()
    {
        while (objectiveScore > score.Value)
        {
            yield return new WaitForSeconds(10f);
            UpdateScoreServerRpc(5);
        }

        if (displayText != null)
        {
            displayText.text = "Objective reached!";
        }
        else
        {
            displayText = GameObject.FindGameObjectWithTag("Objective").GetComponent<TextMeshProUGUI>();
            displayText.text = "Objective reached!";
        }
    }

    private void UpdateScoreText(int scoreValue)
    {
        if (scoreText != null)
        {
            scoreText.text = scoreValue.ToString();
        }
        else
        {
            scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateScoreServerRpc(int points)
    {
        score.Value += points;
        UpdateScoreClientRpc(score.Value);
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int scoreValue)
    {
        UpdateScoreText(scoreValue);
    }

    public void EnemyDestroyedByButton()
    {
        if (IsServer)
        {
            UpdateScoreServerRpc(30);
        }
    }

    public void EnemyDestroyedByLimitZone()
    {
        if (IsServer)
        {
            UpdateScoreServerRpc(-20);
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void HideUIElements()
    {
        hostButton.SetActive(false);
        joinButton.SetActive(false);
        joinCodeInputField.SetActive(false);
    }
}
