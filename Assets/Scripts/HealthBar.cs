using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Tooltip("Reference to the UI Image component used as the health bar fill")]
    public Image fillImage;

    public Image bgImage;

    public Image borderImage;

    [Tooltip("The Fighter or Enemy component this health bar is tracking")]
    public MonoBehaviour targetEntity;
    

    [Tooltip("Whether to show the health bar only when damaged")]
    public bool showOnlyWhenDamaged = false;

    private int currentHitpoint;
    private int maxHitpoint;

    private void Awake()
    {
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
            var spriteRenderer = targetEntity.GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                // Adjust the position based on the sprite size
                var yOffset = -(spriteRenderer.bounds.size.y / 2) - 0.05f;
                Debug.LogError("Y Offset: " + yOffset);
                transform.position = targetEntity.transform.position + new Vector3(0, yOffset, 0);
            }
            else
            {
                transform.position = targetEntity.transform.position;
            }
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
            Debug.LogWarning("Entity is an Enemy HEALTHBAR");
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

        // Set dimensions of the health bar to match the target entity collider
        var spriteRenderer = targetEntity.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            var fillTransform = fillImage.GetComponent<RectTransform>();
            var bgTransform = bgImage.GetComponent<RectTransform>();
            var borderTransform = borderImage.GetComponent<RectTransform>();
            if (fillTransform && bgTransform && borderTransform)
            {
                var spriteWidth = spriteRenderer.bounds.size.x;
                var barWidth = spriteWidth + 0.25f;
                
                fillTransform.sizeDelta = new Vector2(barWidth, 0.05f);
                bgTransform.sizeDelta = new Vector2(barWidth, 0.05f);
                borderTransform.sizeDelta = new Vector2(barWidth + 0.05f, 0.10f);
            }
        }


        UpdateHealthBar();
    }
}