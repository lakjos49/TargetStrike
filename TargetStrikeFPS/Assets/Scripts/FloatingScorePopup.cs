using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingScorePopup : MonoBehaviour
{
    [Header("Settings")]
    public float floatSpeed = 2f;
    public float lifetime = 0.8f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    public Color normalColor = Color.white;
    public Color bonusColor = Color.yellow;

    private TextMeshPro _text;
    private Camera _cam;

    void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _cam = Camera.main;
    }

    public void Initialize(string value, bool isBonus = false)
    {
        if (_text) { _text.text = value; _text.color = isBonus ? bonusColor : normalColor; }
        StartCoroutine(AnimateRoutine());
    }

    IEnumerator AnimateRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;

            transform.position = startPos + Vector3.up * (floatSpeed * t);
            if (_cam) transform.forward = _cam.transform.forward;
            if (_text) _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, fadeCurve.Evaluate(t));

            yield return null;
        }

        Destroy(gameObject);
    }
}
