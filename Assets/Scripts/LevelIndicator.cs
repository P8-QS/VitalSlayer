using TMPro;
using UnityEngine;

public class LevelIndicator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI levelText;
    [Tooltip("Additional vertical offset from the top of the sprite")]
    [SerializeField] private float additionalYOffset = -0.02f;

    [Header("Level Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;

    [Header("Level Thresholds")]
    [SerializeField] private int warningThreshold = 2; // Levels above player for yellow
    [SerializeField] private int dangerThreshold = 4;  // Levels above player for red

    private MonoBehaviour targetEntity;
    private int entityLevel;
    private RectTransform rectTransform;
    private Player player;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player not found for level comparison in LevelIndicator");
        }
    }

    private void LateUpdate()
    {
        if (targetEntity == null)
        {
            Destroy(gameObject);
            return;
        }

        UpdatePosition();
        UpdateLevelDisplay();
    }

    public void Initialize(MonoBehaviour entity)
    {
        targetEntity = entity;

        if (entity is Enemy enemy)
        {
            entityLevel = enemy.level;
        }
        else
        {
            entityLevel = 1;
            Debug.LogWarning($"Entity {entity.name} is not an Enemy, defaulting level to 1");
        }

        SetDimensions();

        UpdateLevelDisplay();
        gameObject.SetActive(true);
    }

    private void SetDimensions()
    {
        if (targetEntity != null && rectTransform != null)
        {
            var spriteRenderer = targetEntity.GetComponent<SpriteRenderer>();
        }
    }

    private void UpdatePosition()
    {
        if (targetEntity == null) return;

        var spriteRenderer = targetEntity.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            var yOffset = (spriteRenderer.bounds.size.y / 2) + additionalYOffset;
            transform.position = targetEntity.transform.position + new Vector3(0, yOffset, 0);
        }
        else
        {
            transform.position = targetEntity.transform.position + new Vector3(0, additionalYOffset, 0);
        }
    }

    private void UpdateLevelDisplay()
    {
        if (levelText == null) return;

        levelText.text = $"Lvl {entityLevel}";

        if (player != null)
        {
            int playerLevel = player.level;
            int levelDifference = entityLevel - playerLevel;

            if (levelDifference >= dangerThreshold)
            {
                levelText.color = dangerColor;
            }
            else if (levelDifference >= warningThreshold)
            {
                levelText.color = warningColor;
            }
            else
            {
                levelText.color = normalColor;
            }
        }
        else
        {
            levelText.color = normalColor;
        }
    }
}