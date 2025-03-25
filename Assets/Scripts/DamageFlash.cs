using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.25f;
    [SerializeField] private AnimationCurve _flashSpeedCurve;

    private Material _material;

    private Coroutine _damageFlashCoroutine;

    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    public void CallDamageFlash(Color color)
    {
        if (_damageFlashCoroutine != null)
        {
            StopCoroutine(_damageFlashCoroutine);
        }

        _damageFlashCoroutine = StartCoroutine(DamageFlasher(color));
    }

    private IEnumerator DamageFlasher(Color color)
    {

        SetFlashColor(color);

        float currentFlashAmount = 0f;
        float elapstedTime = 0f;

        while (elapstedTime < _flashTime)
        {
            elapstedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapstedTime), (elapstedTime / _flashTime));
            SetFlashAmount(currentFlashAmount);
            yield return null;
        }

    }

    private void SetFlashColor(Color color)
    {
        if (color == null)
        {
            color = _flashColor;
        }

        _material.SetColor("_FlashColor", color);
    }

    private void SetFlashAmount(float amount)
    {
        _material.SetFloat("_FlashAmount", amount);
    }


}
