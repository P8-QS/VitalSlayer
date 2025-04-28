using System.Collections.Generic;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    public static EnemyUIManager Instance { get; private set; }

    [Header("Prefabs")]
    [Tooltip("The health bar prefab to be instantiated")]
    public GameObject healthBarPrefab;
    [Tooltip("The level indicator prefab to be instantiated")]
    public GameObject levelIndicatorPrefab;

    [Header("Settings")]
    [Tooltip("The parent transform where health bars will be created")]
    public Transform UIContainer;
    [Tooltip("Whether to show health bars only when entities are damaged")]
    public bool showOnlyWhenDamaged = false;

    // Dictionary to track active health bars
    private Dictionary<MonoBehaviour, GameObject> activeHealthBars = new Dictionary<MonoBehaviour, GameObject>();
    // Dictionary to track active level indicators
    private Dictionary<MonoBehaviour, GameObject> activeLevelIndicators = new Dictionary<MonoBehaviour, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (UIContainer == null)
        {
            UIContainer = new GameObject("UIContainer").transform;
            UIContainer.SetParent(transform);
        }
    }

    /// <summary>
    /// Creates a health bar for the given entity
    /// </summary>
    /// <param name="entity">The Fighter or Enemy entity</param>
    /// <param name="customOffset">Optional custom offset from entity</param>
    /// <returns>The created HealthBar component</returns>
    public HealthBar CreateHealthBar(MonoBehaviour entity, Vector3? customOffset = null)
    {
        if (activeHealthBars.TryGetValue(entity, out GameObject existingBar))
        {
            return existingBar.GetComponent<HealthBar>();
        }

        GameObject healthBar = Instantiate(healthBarPrefab, UIContainer);
        healthBar.name = $"{entity.name}_HealthBar";

        HealthBar healthBarComponent = healthBar.GetComponent<HealthBar>();
        healthBarComponent.showOnlyWhenDamaged = showOnlyWhenDamaged;
        healthBarComponent.SetTarget(entity);

        activeHealthBars[entity] = healthBar;

        if (entity is Enemy)
        {
            CreateLevelIndicator(entity, healthBarComponent);
        }

        return healthBarComponent;
    }

    /// <summary>
    /// Creates a level indicator for the given enemy entity
    /// </summary>
    private void CreateLevelIndicator(MonoBehaviour entity, HealthBar healthBar)
    {
        if (levelIndicatorPrefab == null)
        {
            Debug.LogWarning("Level indicator prefab not assigned to EnemyUIManager");
            return;
        }

        GameObject levelIndicator = Instantiate(levelIndicatorPrefab, UIContainer);
        levelIndicator.name = $"{entity.name}_LevelIndicator";

        LevelIndicator levelIndicatorComponent = levelIndicator.GetComponent<LevelIndicator>();
        levelIndicatorComponent.Initialize(entity, healthBar);

        activeLevelIndicators[entity] = levelIndicator;
    }

    /// <summary>
    /// Removes the health bar and level indicator for the given entity
    /// </summary>
    /// <param name="entity">The entity whose bars should be removed</param>
    public void RemoveHealthBar(MonoBehaviour entity)
    {
        if (activeHealthBars.TryGetValue(entity, out GameObject healthBar))
        {
            Destroy(healthBar);
            activeHealthBars.Remove(entity);
        }

        if (activeLevelIndicators.TryGetValue(entity, out GameObject levelIndicator))
        {
            Destroy(levelIndicator);
            activeLevelIndicators.Remove(entity);
        }
    }

    /// <summary>
    /// Updates all health bars
    /// </summary>
    public void UpdateHealthBars()
    {
        foreach (var kvp in activeHealthBars)
        {
            HealthBar healthBar = kvp.Value.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar();
            }
        }
    }
}