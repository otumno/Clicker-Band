using UnityEngine; // Добавьте эту строку

public class InstrumentController : MonoBehaviour
{
    [SerializeField] private InstrumentBeat[] beatSounds;
    [SerializeField] private AudioClip firstPressSound; // Звук при первом нажатии
    [SerializeField] private float hitWindow = 0.1f;

    private int currentBeatIndex;
    private bool firstPress = true; // Флаг для отслеживания первого нажатия

    public void OnInstrumentClicked()
    {
        GlobalAudioManager.Instance.HandleFirstClick(); // Обрабатываем первый клик

        if (firstPress)
        {
            PlayFirstPressSound(); // Воспроизводим звук при первом нажатии
            firstPress = false; // Устанавливаем флаг, чтобы не воспроизводить звук повторно
        }
        else
        {
            // Обрабатываем нажатия как обычно после первого
            if (!CheckDependencies()) return;

            bool isPerfectHit = GlobalAudioManager.Instance.IsBeatInWindow(hitWindow);
            PlaySound(isPerfectHit);
            ScoreSystem.Instance?.RegisterHit(isPerfectHit);
        }
    }

    private bool CheckDependencies()
    {
        if (GlobalAudioManager.Instance == null)
        {
            Debug.LogError($"{name}: GlobalAudioManager not found!", this);
            return false;
        }

        if (beatSounds == null || beatSounds.Length == 0)
        {
            Debug.LogError($"{name}: No beat sounds configured!", this);
            return false;
        }

        return true;
    }

    private void PlaySound(bool isPerfectHit)
    {
        currentBeatIndex %= beatSounds.Length;
        var beat = beatSounds[currentBeatIndex];

        if (beat == null) return;

        AudioClip clipToPlay = isPerfectHit ? beat.correctSound : beat.missSound;
        if (clipToPlay == null) return;

        AudioManager.Instance?.PlayInstrumentSound(
            clipToPlay, 
            beat.volume,
            isPerfectHit ? 1f : 0.8f
        );

        currentBeatIndex = (currentBeatIndex + 1) % beatSounds.Length;
    }

    private void PlayFirstPressSound()
    {
        Debug.Log("Звук при первом нажатии воспроизведён");

        if (firstPressSound != null)
        {
            AudioManager.Instance?.PlayInstrumentSound(firstPressSound, 1f, 1f);
        }
        else
        {
            Debug.LogWarning("Первый звуковой клип не назначен!");
        }
    }
}
