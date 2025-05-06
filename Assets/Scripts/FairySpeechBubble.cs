using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FairySpeechBubble : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject speechBubbleObject;
    [SerializeField] private GameObject questionMarkObject;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float playerDetectionRadius = 3f;
    private Transform playerTransform;
    [SerializeField] private string playerLayerName = "Actor";

    [Header("Messages")]
    [SerializeField] private List<FairyMessage> messages = new List<FairyMessage>();
    [SerializeField] private bool randomizeMessages = true;
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private float messageChangeInterval = 10f;

    [Header("Visual")]
    [SerializeField] private float bobAmplitude = 0.1f;
    [SerializeField] private float bobFrequency = 2f;
    [SerializeField] private float rotationSpeed = 45f;

    private Dictionary<string, float> messageLastShownTime = new Dictionary<string, float>();
    private int currentMessageIndex = 0;
    private bool isPlayerInRange = false;
    private bool isBubbleActive = false;
    private Vector3 startPosition;
    private Coroutine activeCoroutine;
    private Coroutine messageRotationCoroutine;
    private int playerLayerMask;

    private void Start()
    {
        playerTransform = GameManager.Instance.player.transform;

        startPosition = transform.position;

        playerLayerMask = 1 << LayerMask.NameToLayer(playerLayerName);

        setSpeechBubbleActive(false);

        if (messageText != null)
        {
            messageText.richText = true;
        }

        foreach (var message in messages)
        {
            if (!messageLastShownTime.ContainsKey(message.tag))
            {
                messageLastShownTime.Add(message.tag, -message.cooldownDuration);
            }
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            else
                Debug.LogWarning("Player transform not assigned to FairySpeechBubble. Using sphere overlap detection only.");
        }
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        CheckPlayerProximity();
    }

    private void CheckPlayerProximity()
    {
        bool wasPlayerInRange = isPlayerInRange;

        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            isPlayerInRange = distance <= playerDetectionRadius;
        }
        else
        {
            isPlayerInRange = Physics.CheckSphere(transform.position, playerDetectionRadius, playerLayerMask);
        }

        if (!wasPlayerInRange && isPlayerInRange)
        {
            if (activeCoroutine == null)
                activeCoroutine = StartCoroutine(ShowMessageAfterDelay());
        }
        else if (wasPlayerInRange && !isPlayerInRange)
        {
            if (isBubbleActive)
            {
                setSpeechBubbleActive(false);
                isBubbleActive = false;
            }

            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
                activeCoroutine = null;
            }

            if (messageRotationCoroutine != null)
            {
                StopCoroutine(messageRotationCoroutine);
                messageRotationCoroutine = null;
            }
        }
    }

    private IEnumerator ShowMessageAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);

        ShowNextMessage();

        if (messageChangeInterval > 0)
        {
            messageRotationCoroutine = StartCoroutine(RotateMessages());
        }
    }

    private IEnumerator RotateMessages()
    {
        while (isPlayerInRange)
        {
            yield return new WaitForSeconds(messageChangeInterval);

            if (isPlayerInRange)
            {
                ShowNextMessage();
            }
        }
    }

    private void ShowNextMessage()
    {
        if (messages.Count == 0)
        {
            Debug.LogWarning("No messages configured for fairy speech bubble");
            return;
        }

        FairyMessage messageToShow = null;

        if (randomizeMessages)
        {
            List<FairyMessage> availableMessages = new List<FairyMessage>();

            foreach (var message in messages)
            {
                float lastShownTime = messageLastShownTime[message.tag];
                if (Time.time - lastShownTime >= message.cooldownDuration)
                {
                    availableMessages.Add(message);
                }
            }

            if (availableMessages.Count > 0)
            {
                messageToShow = availableMessages[Random.Range(0, availableMessages.Count)];
            }
        }
        else
        {
            int attempts = 0;
            while (attempts < messages.Count)
            {
                FairyMessage candidateMessage = messages[currentMessageIndex];
                float lastShownTime = messageLastShownTime[candidateMessage.tag];

                if (Time.time - lastShownTime >= candidateMessage.cooldownDuration)
                {
                    messageToShow = candidateMessage;
                    break;
                }

                currentMessageIndex = (currentMessageIndex + 1) % messages.Count;
                attempts++;
            }
        }

        if (messageToShow != null)
        {
            DisplayMessage(messageToShow);
        }
    }

    private void DisplayMessage(FairyMessage message)
    {
        messageText.text = message.message;
        setSpeechBubbleActive(true);
        isBubbleActive = true;

        messageLastShownTime[message.tag] = Time.time;
    }

    private void setSpeechBubbleActive(bool active)
    {
        if (speechBubbleObject != null)
        {
            speechBubbleObject.SetActive(active);
        }

        if (questionMarkObject != null)
        {
            questionMarkObject.SetActive(!active);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}