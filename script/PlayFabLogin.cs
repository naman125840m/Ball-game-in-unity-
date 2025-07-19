using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabLogin : MonoBehaviour
{
    public static PlayFabLogin Instance { get; private set; }

    [Header("UI References")]
    public GameObject namepannal;
    [SerializeField] GameObject _playerprifab;
    [SerializeField] GameObject _playerparent;

    string leaderboardName = "HighScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "7BF0B";
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("✅ Logged in to PlayFab");

        if (PlayerPrefs.HasKey("NameSet"))
        {
            namepannal?.SetActive(false);
        }
        else
        {
            namepannal?.SetActive(true);
        }
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("❌ Login failed: " + error.GenerateErrorReport());
    }

    public void changeplayername(TMP_InputField nameinput)
    {
        if (string.IsNullOrEmpty(nameinput.text)) return;
        if (PlayerPrefs.HasKey("NameSet"))
        {
            namepannal?.SetActive(false);
            return;
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameinput.text
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
        {
            PlayerPrefs.SetString("PlayerName", result.DisplayName);
            PlayerPrefs.SetInt("NameSet", 1);
            PlayerPrefs.Save();
            namepannal?.SetActive(false);
            GetLeaderboard();
        },
        error => Debug.LogError("❌ Failed to update name: " + error.GenerateErrorReport()));
    }

    public void SendHighscore(int highscore)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = highscore
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => Debug.Log("✅ Highscore sent."),
            error => Debug.LogError("❌ Failed to send highscore: " + error.GenerateErrorReport()));
    }

    public void GetLeaderboard()
    {
        if (_playerprifab == null || _playerparent == null)
        {
            Debug.LogWarning("⚠️ Leaderboard UI not assigned");
            return;
        }

        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            for (int i = _playerparent.transform.childCount - 1; i >= 1; i--)
            {
                Destroy(_playerparent.transform.GetChild(i).gameObject);
            }

            foreach (var item in result.Leaderboard)
            {
                GameObject player = Instantiate(_playerprifab, _playerparent.transform, false);
                player.transform.GetChild(0).GetComponent<TMP_Text>().text = (item.Position + 1).ToString();
                player.transform.GetChild(1).GetComponent<TMP_Text>().text = item.DisplayName;
                player.transform.GetChild(2).GetComponent<TMP_Text>().text = item.StatValue.ToString();
            }

        }, error => Debug.LogError("❌ Failed to load leaderboard: " + error.GenerateErrorReport()));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (namepannal == null)
            namepannal = GameObject.Find("nameinputpannal");

        if (_playerparent == null)
            _playerparent = GameObject.Find("Content");

        if (_playerprifab == null)
            _playerprifab = Resources.Load<GameObject>("playername");

        if (PlayerPrefs.GetInt("refreshLeaderboard", 0) == 1)
        {
            PlayerPrefs.SetInt("refreshLeaderboard", 0);
            GetLeaderboard();
        }
    }
}