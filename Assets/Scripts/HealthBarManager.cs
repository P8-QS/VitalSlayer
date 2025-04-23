using System.Collections.Generic;
using UnityEngine;


public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance { get; private set; }

    [Header("Prefabs")] [Tooltip("The health bar prefab to be instantiated")]
    public GameObject healthBarPrefab;

    [Header("Settings")] [Tooltip("The parent transform where health bars will be created")]
    public Transform healthBarContainer;

    [Tooltip("Whether to show health bars only when entities are damaged")]
    public bool showOnlyWhenDamaged = false;

    // Dictionary to track active health bars
    private Dictionary<MonoBehaviour, GameObject> activeHealthBars = new Dictionary<MonoBehaviour, GameObject>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Create container if it doesn't exist
        if (healthBarContainer == null)
        {
            healthBarContainer = new GameObject("HealthBarContainer").transform;
            healthBarContainer.SetParent(transform);
        }
    }

    /// <summary>
    /// Creates a health bar for the given entity
    /// </summary>
    /// <param name="entity">The Fighter or Enemy entity</param>
    /// <param name="customOffset">Optional custom offset from entity</param>
    /// <param name="type">Type of health bar to create</param>
    /// <returns>The created HealthBar component</returns>
    public HealthBar CreateHealthBar(MonoBehaviour entity, Vector3? customOffset = null)
    {
        // If this entity already has a health bar, return it
        if (activeHealthBars.TryGetValue(entity, out GameObject existingBar))
        {
            return existingBar.GetComponent<HealthBar>();
        }

        // Create new health bar
        GameObject healthBar = Instantiate(healthBarPrefab, healthBarContainer);
        healthBar.name = $"{entity.name}_HealthBar";

        // Set up the HealthBar component
        HealthBar healthBarComponent = healthBar.GetComponent<HealthBar>();

        healthBarComponent.showOnlyWhenDamaged = showOnlyWhenDamaged;

        // Set the target entity
        healthBarComponent.SetTarget(entity);

        // Track this health bar
        activeHealthBars[entity] = healthBar;

        return healthBarComponent;
    }


    /// <summary>
    /// Removes the health bar for the given entity
    /// </summary>
    /// <param name="entity">The entity whose health bar should be removed</param>
    public void RemoveHealthBar(MonoBehaviour entity)
    {
        if (activeHealthBars.TryGetValue(entity, out GameObject healthBar))
        {
            Destroy(healthBar);
            activeHealthBars.Remove(entity);
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