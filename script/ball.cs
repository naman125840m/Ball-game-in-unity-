using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ball : MonoBehaviour
{
    public int totalcoin = 0;
    public Rigidbody rb;
    public float mospeed = 10f;
    public TMP_Text texts;
    public GameObject gameover;
    bool Isgameover = false;
    public roadmanager roadmanagers;
    public List<Color> _colour;
    public Material _mat;

    [Header("State Texts")]
    public TMP_Text scoretext;
    public TMP_Text hightext;
    public TMP_Text speedtext;
    public TMP_Text cointext;

    [Header("Speed Control")]
    public float baseSpeed = 3f;
    public float speedIncreaseRate = 0.05f;
    public float maxSpeed = 10f;

    private float currentSpeed;

    private void Start()
    {
        currentSpeed = baseSpeed;

        int currenthighscore = PlayerPrefs.GetInt("highscore", 0);
        hightext.text = "score: " + currenthighscore.ToString();

        totalcoin = PlayerPrefs.GetInt("coin", 0);
        cointext.text = "coin: " + totalcoin.ToString();

        _mat.EnableKeyword("_EMISSION");
        int activecolour = PlayerPrefs.GetInt("activecolour");

        if (_colour != null && activecolour < _colour.Count)
        {
            Color color = _colour[activecolour] * 4;
            _mat.SetColor("_EmissionColor", color);
        }
        else
        {
            Debug.LogWarning("Color index out of range or null.");
        }
    }

    void Update()
    {
        updatespeedui();

        if (!Isgameover)
        {
            currentSpeed += speedIncreaseRate * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            handlescore();

            Vector3 velocity = rb.linearVelocity;
            velocity.z = currentSpeed;
            rb.linearVelocity = velocity;

            // PC movement
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb.AddForce(Vector3.left * mospeed);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.AddForce(Vector3.right * mospeed);
            }

            // Mobile touch movement
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.position.x < Screen.width / 2)
                    rb.AddForce(Vector3.left * mospeed);
                else
                    rb.AddForce(Vector3.right * mospeed);
            }

            if (transform.position.y < -1)
            {
                ongameover();
            }
        }
    }

    void updatespeedui()
    {
        speedtext.text = "speed: " + rb.linearVelocity.magnitude.ToString("##") + " Km/h";
    }

    public void handlescore()
    {
        texts.text = "score " + (transform.position.z + 71).ToString("##");
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "hard")
        {
            ongameover();
            audiomanager.instance.onobstacall();
        }
    }

    public void ongameover()
    {
        gameover.SetActive(true);
        Isgameover = true;
        handlescore();
        highscores();
        PlayerPrefs.SetInt("refreshLeaderboard", 1);
    }

    void highscores()
    {
        int current = (int)(transform.position.z + 71);
        int currenthighscore = PlayerPrefs.GetInt("highscore", 0);

        if (current > currenthighscore)
        {
            PlayerPrefs.SetInt("highscore", current);
            PlayFabLogin.Instance.SendHighscore(current);
        }
    }

    public void playagain()
    {
        PlayerPrefs.SetInt("restartFlag", 1);
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "boxcooo")
        {
            roadmanagers.roadspam();
        }

        if (other.gameObject.tag == "coin")
        {
            Destroy(other.gameObject);
            coincollides();
        }
    }

    void coincollides()
    {
        totalcoin++;
        PlayerPrefs.SetInt("coin", totalcoin);
        cointext.text = "coin: " + totalcoin.ToString();
        audiomanager.instance.oncoincollect();
    }

    // ✅ Revive player after watching rewarded ad
    public void RevivePlayer()
    {
        Debug.Log("Reviving player after watching ad...");

        Isgameover = false;
        gameover.SetActive(false);

        // Move ball slightly above ground to avoid fall
        Vector3 revivePos = transform.position;
        revivePos.y = 0.5f;
        revivePos.x = -14f;
        transform.position = revivePos;

        // Stop all current motion
        rb.linearVelocity = Vector3.zero;
        currentSpeed = baseSpeed;

        // Optional: revive sound
        //if (audiomanager.instance != null)
            //audiomanager.instance.onrevive(); // If such sound exists
    }
}
