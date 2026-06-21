using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;
    public Slider healthSlider;
    public Slider armorSlider;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI headshotsText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI ammoText;
    public Image crosshair;
    public GameObject hudRoot;

    [Header("Notifications")]
    public TextMeshProUGUI hitMarkerText;
    public TextMeshProUGUI headshotBannerText;
    public TextMeshProUGUI killStreakText;
    public TextMeshProUGUI pickupText;
    public TextMeshProUGUI reloadText;
    public Image damageOverlay;
    public GameObject waveCompletePanel;
    public TextMeshProUGUI waveCompleteText;

    [Header("Screens")]
    public GameObject startScreen;
    public GameObject pauseMenu;
    public GameObject gameOverScreen;

    [Header("Game Over")]
    public TextMeshProUGUI goScore, goKills, goHeadshots, goAccuracy, goWave, goTime, goHighScore;

    [Header("Minimap")]
    public RawImage minimapRender;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowStartScreen()   { SetScreens(false); if (startScreen) startScreen.SetActive(true); }
    public void ShowHUD()           { SetScreens(false); if (hudRoot) hudRoot.SetActive(true); }
    public void ShowPauseMenu()     { if (pauseMenu) pauseMenu.SetActive(true); }
    public void HidePauseMenu()     { if (pauseMenu) pauseMenu.SetActive(false); }

    public void ShowGameOver(int score, int kills, int hs, float acc, int wave, int timeSec, int highScore)
    {
        SetScreens(false);
        if (gameOverScreen) gameOverScreen.SetActive(true);
        Set(goScore, score.ToString("N0"));
        Set(goKills, kills.ToString());
        Set(goHeadshots, hs.ToString());
        Set(goAccuracy, acc.ToString("F1") + "%");
        Set(goWave, wave.ToString());
        Set(goTime, $"{timeSec / 60}:{timeSec % 60:00}");
        Set(goHighScore, highScore.ToString("N0"));
    }

    public void UpdateHealthUI(int hp, int maxHp, int armor, int maxArmor)
    {
        Set(healthText, hp.ToString());
        Set(armorText, armor.ToString());
        if (healthSlider) healthSlider.value = (float)hp / maxHp;
        if (armorSlider)  armorSlider.value  = (float)armor / maxArmor;
    }

    public void UpdateAmmoUI(int mag, int reserve)
    {
        if (ammoText) ammoText.text = (mag >= 0 ? mag : 0) + " / " + reserve;
    }

    public void UpdateScoreUI(int score, int kills, int hs)
    {
        Set(scoreText, score.ToString("N0"));
        Set(killsText, kills.ToString());
        Set(headshotsText, hs.ToString());
    }

    public void UpdateWaveUI(int wave) => Set(waveText, "WAVE " + wave);

    public void ShowDamageIndicator()
    {
        if (!damageOverlay) return;
        StopCoroutine("FadeDamage");
        StartCoroutine("FadeDamage");
    }

    IEnumerator FadeDamage()
    {
        if (!damageOverlay) yield break;
        damageOverlay.color = new Color(1, 0, 0, 0.45f);
        yield return new WaitForSeconds(0.1f);
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            damageOverlay.color = new Color(1, 0, 0, Mathf.Lerp(0.45f, 0f, t / 0.4f));
            yield return null;
        }
    }

    public void ShowHitMarker(bool headshot)
    {
        if (!hitMarkerText) return;
        hitMarkerText.text = headshot ? "X" : "+";
        hitMarkerText.color = headshot ? Color.yellow : Color.white;
        StopCoroutine("FadeHitMarker");
        StartCoroutine("FadeHitMarker");
    }

    IEnumerator FadeHitMarker()
    {
        if (!hitMarkerText) yield break;
        hitMarkerText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        hitMarkerText.gameObject.SetActive(false);
    }

    public void ShowHeadshotNotification() => StartCoroutine(ShowBanner(headshotBannerText, "HEADSHOT!", Color.yellow, 1.2f));
    public void ShowKillStreakNotification(int streak, int bonus) => StartCoroutine(ShowBanner(killStreakText, $"{streak} KILL STREAK +{bonus}", Color.red, 1.5f));
    public void ShowPickupNotification(string msg, Color col) => StartCoroutine(ShowBanner(pickupText, msg, col, 1.5f));

    IEnumerator ShowBanner(TextMeshProUGUI tmp, string msg, Color col, float dur)
    {
        if (!tmp) yield break;
        tmp.text = msg; tmp.color = col;
        tmp.gameObject.SetActive(true);
        yield return new WaitForSeconds(dur);
        tmp.gameObject.SetActive(false);
    }

    public void ShowWaveComplete(int wave)
    {
        if (!waveCompletePanel) return;
        if (waveCompleteText) waveCompleteText.text = $"WAVE {wave} COMPLETE";
        waveCompletePanel.SetActive(true);
        StartCoroutine(HideAfter(waveCompletePanel, 3f));
    }

    IEnumerator HideAfter(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (go) go.SetActive(false);
    }

    public void ShowReloadIndicator(bool show) { if (reloadText) reloadText.gameObject.SetActive(show); }
    public void HidePowerupIndicator(string type) { }

    public void ShowFloatingDamage(Vector3 worldPos, int damage, bool headshot)
    {
        // Instantiate a world-space TextMeshPro popup above enemy (prefab-based)
    }

    void SetScreens(bool v)
    {
        if (startScreen)    startScreen.SetActive(v);
        if (pauseMenu)      pauseMenu.SetActive(v);
        if (gameOverScreen) gameOverScreen.SetActive(v);
        if (hudRoot)        hudRoot.SetActive(v);
    }

    void Set(TextMeshProUGUI tmp, string txt) { if (tmp) tmp.text = txt; }

    public void OnStartButton()   => GameManager.Instance.StartGame();
    public void OnResumeButton()  => GameManager.Instance.ResumeGame();
    public void OnRestartButton() => GameManager.Instance.RestartGame();
    public void OnMenuButton()    => GameManager.Instance.QuitToMenu();
    public void OnQuitButton()    => GameManager.Instance.QuitGame();
}
