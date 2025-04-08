using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab;
    public AudioClip gameMusic;

    public Player player;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRound()
    {
        // Generate map

        // Play game music
        SoundFxManager.Instance.PlayMusic(gameMusic, 0.05f, false, 2f);

        // Load the game scene with a transition
        SceneLoader.Instance.LoadSceneWithTransition("Game", () =>
        {
            Debug.Log("Game scene loaded, spawning player...");

            var spawnPoint = new GameObject("Spawn").transform;
            spawnPoint.position = new Vector3(0, 0, 0);
            var playerGo = Instantiate(playerPrefab, spawnPoint);
            player = playerGo.GetComponent<Player>();


            GameSummaryManager.Instance.Reset();
        });
    }
}