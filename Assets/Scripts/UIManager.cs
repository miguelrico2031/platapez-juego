using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Money")] [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private int _digits = 18;
    [SerializeField] private float _moneySpeed;

    [Header("Metamorphose")] [SerializeField]
    private Button _metamorphoseButton;

    [SerializeField] private Slider _metDurationSlider;
    [SerializeField] private float _metCooldownButtonDuration = 6f;

    [Header("GameOver")] [SerializeField] private Image _fadeOut;
    [SerializeField] private AudioClip _gameOverSound;

    private ulong _money = 0;
    private ulong _displayedMoney = 0;
    private float _metButtonCooldownTimer;
    private float _metDurationTimer;
    private bool _isMet;
    private float _moneyDelayPeriod;
    private float _moneyTimer;


    private void Awake()
    {
        Instance = this;


        _moneyDelayPeriod = 1f / _moneySpeed;
        _moneyTimer = _moneyDelayPeriod;
    }

    private void Update()
    {
        if (_isMet)
        {
            _metDurationTimer = Mathf.Max(0f, _metDurationTimer - Time.deltaTime);
            _metDurationSlider.value = _metDurationTimer;
            _metButtonCooldownTimer -= Time.deltaTime;
            // _metamorphoseButton.GetComponent<Image>().fillAmount = 
            //     Mathf.Clamp01(Mathf.Lerp(1f, 0f, _metButtonCooldownTimer / _metCooldownButtonDuration));
            if (_metButtonCooldownTimer > 0f) return;
            _metamorphoseButton.enabled = true;
            _isMet = false;
        }

        if (_money > _displayedMoney)
        {
            _moneyTimer -= Time.deltaTime;
            if (_moneyTimer <= 0f)
            {
                _moneyTimer = _moneyDelayPeriod;
                _displayedMoney += 100;
                UpdateMoney();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && _metamorphoseButton.enabled)
            PressMetamorphoseButton();
    }

    private void Start()
    {
        _metamorphoseButton.GetComponent<Animator>().SetFloat("Speed", 1f / _metCooldownButtonDuration);
        UpdateMoney();
        _metDurationSlider.value = 0f;
    }

    public void AddMoney(ulong money)
    {
        _money += money;
    }


    private void UpdateMoney()
    {
        string paddedMoney = _displayedMoney.ToString($"D{_digits}");

        _moneyText.text = FormatWithSpaces(paddedMoney);
    }

    public void PressMetamorphoseButton()
    {
        var pm = FindObjectOfType<PlayerMetamorphose>();
        pm.Metamorphose();
        _metamorphoseButton.enabled = false;
        // _metamorphoseButton.GetComponent<Image>().fillAmount = 0f;
        _isMet = true;
        _metamorphoseButton.GetComponent<Animator>().Play("Cooldown");
        _metButtonCooldownTimer = _metCooldownButtonDuration;
        _metDurationSlider.maxValue = pm.MetamorphoseDuration;
        _metDurationTimer = pm.MetamorphoseDuration;
        _metDurationSlider.value = pm.MetamorphoseDuration;
    }


    private static string FormatWithSpaces(string input)
    {
        // Comienza desde el final e inserta un espacio cada 3 dígitos.
        for (int i = input.Length - 3; i > 0; i -= 3)
        {
            input = input.Insert(i, " ");
        }

        return input;
    }


    public void EndGame()
    {
        PlayerPrefs.SetString("Score", _displayedMoney.ToString());
        _fadeOut.gameObject.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance.StopAllSounds();
        AudioManager.Instance.PlaySound(_gameOverSound);
        foreach (var drag in FindObjectsByType<DraggableUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Destroy(drag.gameObject);
    }
}