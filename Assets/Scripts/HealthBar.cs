using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Tooltip("Reference to the UI Image component used as the health bar fill")]
    public Image fillImage;

    [Tooltip("The Fighter or Enemy component this health bar is tracking")]
    private MonoBehaviour targetEntity;

    [Tooltip("Offset from the entity position")]
    public Vector3 offset = new Vector3(0, 0.005f, 0);

    [Tooltip("Whether to show the health bar only when damaged")]
    public bool showOnlyWhenDamaged = false;

    private Camera mainCamera;
    private int currentHitpoint;
    private int maxHitpoint;

    private void Awake()
    {
        mainCamera = Camera.main;

        // Hide health bar initially if it should only show when damaged
        if (showOnlyWhenDamaged && fillImage)
        {
            fillImage.transform.parent.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (targetEntity == null || fillImage == null)
        {
            // Target entity was destroyed, remove this health bar
            if (targetEntity == null && gameObject != null)
            {
                Destroy(gameObject);
            }
            return;
        }

        // Update position to follow the entity
        UpdatePosition();

        // Update the health display
        UpdateHealthBar();
    }

    private void UpdatePosition()
    {
        if (targetEntity != null)
        {
            transform.position = targetEntity.transform.position + offset;
        }
    }

    public void UpdateHealthBar()
    {
        if (targetEntity == null || fillImage == null)
            return;

        // Get hitpoint and maxHitpoint based on the entity type
        GetEntityHealthValues();

        float healthPercentage = (float)currentHitpoint / maxHitpoint;
        fillImage.fillAmount = healthPercentage;

        // Show/hide based on damage if that option is enabled
        if (showOnlyWhenDamaged)
        {
            fillImage.transform.parent.gameObject.SetActive(healthPercentage < 1f);
        }

        // Optional: Change color based on health percentage
        // if (fillImage)
        // {
        //     fillImage.color = Color.Lerp(Color.red, Color.green, healthPercentage);
        // }
    }

    private void GetEntityHealthValues()
    {
        if (targetEntity is Fighter fighter)
        {
            currentHitpoint = fighter.hitpoint;
            maxHitpoint = fighter.maxHitpoint;
        }
        else if (targetEntity is Enemy enemy)
        {
            currentHitpoint = enemy.hitpoint;
            maxHitpoint = enemy.maxHitpoint;
        }
        else
        {
            // Default values if no valid entity type is found
            currentHitpoint = 0;
            maxHitpoint = 1;
        }
    }

    // Method to manually set the target entity (used by HealthBarManager)
    public void SetTarget(MonoBehaviour entity)
    {
        targetEntity = entity;
        UpdateHealthBar();
    }
}