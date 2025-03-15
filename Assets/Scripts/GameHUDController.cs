using TMPro;
using UnityEngine;

public class GameHUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _waveAnnounceText;
    [SerializeField] private TextMeshProUGUI _abilityDisplayText;

    private int _currentScore = 0;

    void Start()
    {
        UpdateScore(_currentScore);
        ClearAbilityDisplay();
    }


    void Update()
    {

    }

    public void UpdateScore(int score)
    {
        _currentScore = score;
        if (_scoreText != null)
        {
            _scoreText.text = "Score " + _currentScore.ToString();
        }
    }

    public void AnnounceWave(string waveText)
    {
        if (_waveAnnounceText != null)
        {
            _waveAnnounceText.text = waveText;
        }
    }

    public void UpdateAbilityDisplay(string abilityName, int abilityCound)
    {
        if (_abilityDisplayText != null)
        {
            _abilityDisplayText.text = $"{abilityCound} X {abilityName}";
        }
    }

    public void ClearAbilityDisplay()
    {
        if (_abilityDisplayText != null)
        {
            _abilityDisplayText.text = "No Ability";
        }
    }
}
