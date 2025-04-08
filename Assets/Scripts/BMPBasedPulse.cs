using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BPMBasedPulse : MonoBehaviour
{
    [Header("Pulse Timing")]
    [Range(40f, 300f)] public float pulseBPM = 120f; // BPM для пульсации
    public bool[] pulsePattern = new bool[10]; // Паттерн пульсации (до 10 выборов)

    [Header("Pulse Settings")]
    [Range(0.1f, 5f)] public float pulseSize = 1.3f;
    [Range(0.01f, 1f)] public float pulseSpeedIncrease = 0.3f; // Скорость увеличения масштаба
    [Range(0.01f, 1f)] public float pulseSpeedDecrease = 0.5f; // Скорость уменьшения масштаба
    [Range(0f, 1f)] public float pulseSmoothness = 0.7f;
    public bool affectX = true;
    public bool affectY = true;
    public bool affectZ = false;

    [Header("Visual Effects")]
    public Color pulseColor = Color.white;
    [Range(0f, 3f)] public float colorIntensity = 1f;
    public bool useColorPulse = false;

    // Системные переменные
    private Vector3 baseScale;
    private Material materialInstance;
    private Color baseColor;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private int currentPatternIndex = 0;
    private bool isPulsing = false;
    private float beatTimer = 0f;

    void Start()
    {
        InitializeComponents();
        StartPulsePattern();
    }

    private void InitializeComponents()
    {
        baseScale = transform.localScale;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materialInstance = renderer.material;
            baseColor = materialInstance.color;
        }
        else
        {
            Debug.LogError("Не найден компонент Renderer!");
        }
    }

    private void StartPulsePattern()
    {
        float pulseInterval = 60f / pulseBPM; // Вычисляем интервал в секундах
        InvokeRepeating(nameof(CheckPulsePattern), 0f, pulseInterval);
    }

    private void CheckPulsePattern()
    {
        if (currentPatternIndex >= pulsePattern.Length)
        {
            currentPatternIndex = 0; // Зацикливаем паттерн
        }

        if (pulsePattern[currentPatternIndex])
        {
            StartPulse();
        }

        currentPatternIndex++;
    }

    private void StartPulse()
    {
        isPulsing = true;
        beatTimer = 0f;
    }

    void Update()
    {
        if (isPulsing)
        {
            UpdatePulse();
        }
    }

    private void UpdatePulse()
    {
        beatTimer += Time.deltaTime / pulseSpeedIncrease; // Используем скорость увеличения
        float pulseValue = Mathf.Sin(beatTimer * Mathf.PI); // Используем синус для плавного увеличения и уменьшения
        pulseValue = Mathf.Clamp01((pulseValue + 1) / 2); // Нормализуем значение от 0 до 1

        ApplyPulseEffect(pulseValue);

        if (beatTimer >= 1f)
        {
            isPulsing = false;
            StartCoroutine(DecreasePulse());
        }
    }

    private System.Collections.IEnumerator DecreasePulse()
    {
        float decreaseTimer = 0f;
        while (decreaseTimer < 1f)
        {
            decreaseTimer += Time.deltaTime / pulseSpeedDecrease; // Используем скорость уменьшения
            float pulseValue = Mathf.Clamp01(1 - decreaseTimer); // Уменьшаем пульсацию
            ApplyPulseEffect(pulseValue);
            yield return null;
        }
    }

    private void ApplyPulseEffect(float pulseValue)
    {
        // Масштаб
        Vector3 newScale = baseScale;
        float scaleMultiplier = 1f + (pulseSize - 1f) * pulseValue;

        if (affectX) newScale.x *= scaleMultiplier;
        if (affectY) newScale.y *= scaleMultiplier;
        if (affectZ) newScale.z *= scaleMultiplier;

        transform.localScale = newScale;

        // Цвет
        if (useColorPulse && materialInstance != null)
        {
            Color targetColor = Color.Lerp(baseColor, pulseColor, pulseValue * colorIntensity);
            materialInstance.color = targetColor;

            if (materialInstance.HasProperty(EmissionColor))
            {
                materialInstance.SetColor(EmissionColor, targetColor * colorIntensity);
            }
        }
    }

    void OnValidate()
    {
        pulseSize = Mathf.Max(0.1f, pulseSize);
    }
}
