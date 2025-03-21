using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HealthBarType
{
    Standard,
    Boss
}

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance { get; private set; }

    [Header("Prefabs")]
    [Tooltip("The standard health bar prefab to be instantiated")]
    public GameObject standardHealthBarPrefab;

    [Tooltip("The boss health bar prefab to be instantiated")]
    public GameObject bossHealthBarPrefab;

    [Header("Settings")]
    [Tooltip("The parent transform where health bars will be created")]
    public Transform healthBarContainer;

    [Tooltip("Whether to show health bars only when entities are damaged")]
    public bool showOnlyWhenDamaged = false;

    [Header("Offset Settings")]
    [Tooltip("Default offset from the entity position for standard entities")]
    public Vector3 defaultOffset = new Vector3(0, -0.5f, 0);

    [Tooltip("Default offset from the entity position for boss entities")]
    public Vector3 bossOffset = new Vector3(0, -1.0f, 0);

    [Tooltip("Whether to automatically adjust the offset based on sprite size")]
    public bool autoAdjustOffsetBySize = true;

    [Tooltip("Multiplier for auto-adjustment (higher = health bar positioned higher)")]
    public float offsetMultiplier = 0.75f;

    // Dictionary to track active health bars
    private Dictionary<MonoBehaviour, GameObject> activeHealthBars = new Dictionary<MonoBehaviour, GameObject>();

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

        bool isBoss = entity.gameObject.GetComponent<Boss>() != null;
        HealthBarType type = isBoss ? HealthBarType.Boss : HealthBarType.Standard;
        GameObject prefabToUse = isBoss ? bossHealthBarPrefab : standardHealthBarPrefab;

        // If no boss prefab is set, fall back to standard
        if (prefabToUse == null)
        {
            prefabToUse = standardHealthBarPrefab;
        }

        // Create new health bar
        GameObject healthBar = Instantiate(prefabToUse, healthBarContainer);
        healthBar.name = $"{entity.name}_HealthBar";

        // Set up the HealthBar component
        HealthBar healthBarComponent = healthBar.GetComponent<HealthBar>();

        // Determine offset
        Vector3 offset;
        if (customOffset.HasValue)
        {
            // Use custom offset if provided
            offset = customOffset.Value;
        }
        else if (autoAdjustOffsetBySize)
        {
            // Calculate offset based on entity size
            offset = CalculateOffsetForEntity(entity);
        }
        else
        {
            // Use default offset based on type
            offset = type == HealthBarType.Boss ? bossOffset : defaultOffset;
        }

        healthBarComponent.offset = offset;
        healthBarComponent.showOnlyWhenDamaged = showOnlyWhenDamaged;

        // Set the target entity
        healthBarComponent.SetTarget(entity);

        // Track this health bar
        activeHealthBars[entity] = healthBar;

        return healthBarComponent;
    }

    /// <summary>
    /// Calculates an appropriate offset for the entity based on its sprite or collider size
    /// </summary>
    private Vector3 CalculateOffsetForEntity(MonoBehaviour entity)
    {
        // Start with the default offset
        Vector3 calculatedOffset = defaultOffset;

        // Check if this entity is a boss
        Debug.Log("Type of entity: " + entity.GetType() + " Name of entity: " + entity.name);
        bool isBoss = entity.gameObject.GetComponent<Boss>() != null;
        if (isBoss)
        {
            calculatedOffset = bossOffset;
        }

        // Try to get the sprite renderer
        SpriteRenderer spriteRenderer = entity.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // Adjust Y offset based on sprite height
            float spriteHeight = spriteRenderer.sprite.bounds.size.y * entity.transform.localScale.y;
            calculatedOffset.y = spriteHeight * offsetMultiplier;
            return calculatedOffset;
        }

        // If no sprite renderer, try to use collider
        Collider2D collider = entity.GetComponent<Collider2D>();
        if (collider != null)
        {
            // Adjust Y offset based on collider height
            float colliderHeight = collider.bounds.size.y;
            calculatedOffset.y = colliderHeight * offsetMultiplier;
            return calculatedOffset;
        }

        // Return the default offsets if we couldn't determine size
        return isBoss ? bossOffset : defaultOffset;
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