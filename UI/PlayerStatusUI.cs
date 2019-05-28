// ======================================================================
//    Land Forgotten : Player Status UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("UI Contents")]
    [SerializeField] private Image _HealthMeter;
    [SerializeField] private Image _StatusIcon;
    [SerializeField] private Image _HealthDelayed;
    [Header("Icons")]
    [SerializeField] private Sprite[] _HeroIcons;
    [SerializeField] private Image[] _DeathIcons;
    [Header("Player Location")]
    [SerializeField] private GameObject _ArrowLocation;
    [SerializeField] private bool _IsTrackingPlayer = true;
    [Header("Player Status")]
    [SerializeField] private GameObject _AttackUpStatus;
    [SerializeField] private Image _ArrowStatus;
    [SerializeField] private GameObject _HalfAttackUpParticles;
    [SerializeField] private GameObject _FullAttackUpParticles;
    [SerializeField] private GameObject _PlayerHitParticles;

    [Header("Adjustable Variables")]
    [SerializeField] private float _HealthDelay = 3f;
    [SerializeField] private float _PulseSpeed = 3f;
    [SerializeField] private float _ShakeStrength = 5f;

    private Sprite[] _MyCharacterIcons = new Sprite[2];

    private int _KOCount = 0;
    private float _Percentage;
    private bool _IsPulsing = false;

    [HideInInspector] public float PlayerInitialHealth = 0;
    [HideInInspector] public GameObject MyCurrentCharacter;

    private void Awake()
    {
        PlayerInitialHealth = 0;
        _KOCount = 0;

        if (_IsTrackingPlayer == false)
        {
            _HalfAttackUpParticles.SetActive(false);
            _FullAttackUpParticles.SetActive(false);
            _PlayerHitParticles.SetActive(false);
        }
    }

    private void Update()
    {
        if (_IsTrackingPlayer == true && MyCurrentCharacter != null)
        {
            ShowPlayersLocation(MyCurrentCharacter.transform.localPosition);
        }
        if (_IsTrackingPlayer == false)
        {
            ShowBonusAttackState();
        }
    }

    private void ShowBonusAttackState()
    {
        GameObject player = gameObject.GetComponentInParent<HeroesUI>().MyPlayer;
        if (player == null)
            return;

        int playerNearCount = player.GetComponent<AttackDamage>().PLayerNearCount();
        Image overallColor = _AttackUpStatus.GetComponent<Image>();

        switch (playerNearCount)
        {
            case 1:
                overallColor.color = Color.grey;
                _ArrowStatus.fillAmount = 0.5f;
                _HalfAttackUpParticles.SetActive(true);
                _FullAttackUpParticles.SetActive(false);
                break;
            case 2:
                overallColor.color = Color.white;
                _ArrowStatus.fillAmount = 1f;
                _HalfAttackUpParticles.SetActive(false);
                _FullAttackUpParticles.SetActive(true);
                break;
            default:
                overallColor.color = new Color(0.1764f, 0.0980f, 0.0156f, 1f);
                _ArrowStatus.fillAmount = 0f;
                _HalfAttackUpParticles.SetActive(false);
                _FullAttackUpParticles.SetActive(false);
                break;
        }

    }

    private void ShowPlayersLocation(Vector3 playerPos)
    {
        // Debug.Log(gameObject.name + ": " + playerPos);
        GameObject myPos = gameObject.GetComponentInParent<HeroesUI>().MyPlayer;
        Vector3 direction = (playerPos - myPos.transform.localPosition);
        float angle = Vector3.SignedAngle(direction.normalized, myPos.transform.forward.normalized, Vector3.up);

        // Debug.DrawRay(myPos.transform.position, direction, Color.green);
        // Debug.Log("Angle: " + angle);

        _ArrowLocation.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ResetHealthBars()
    {
        _HealthMeter.fillAmount = 1f;
        _StatusIcon.color = Color.white;
        _HealthMeter.color = Color.green;
    }

    public void SetCharacterIcons(CharacterSelectionType character)
    {
        switch (character)
        {
            case CharacterSelectionType.PEPITO:
                if (_ArrowLocation != null)
                    _ArrowLocation.GetComponentInChildren<Image>().color = Color.green;
                _MyCharacterIcons[0] = _HeroIcons[2];
                _MyCharacterIcons[1] = _HeroIcons[3];
                break;
            case CharacterSelectionType.JUANITO:
                if (_ArrowLocation != null)
                    _ArrowLocation.GetComponentInChildren<Image>().color = Color.blue;
                _MyCharacterIcons[0] = _HeroIcons[4];
                _MyCharacterIcons[1] = _HeroIcons[5];
                break;
            default:
                if (_ArrowLocation != null)
                    _ArrowLocation.GetComponentInChildren<Image>().color = Color.magenta;
                _MyCharacterIcons[0] = _HeroIcons[0];
                _MyCharacterIcons[1] = _HeroIcons[1];
                break;
        }

        _StatusIcon.sprite = _MyCharacterIcons[1];
    }

    public void UpdateHealthMeter(float amount, bool isHealing)
    {
        _Percentage = ((amount * 100) / PlayerInitialHealth) * 0.01f;
        _HealthMeter.fillAmount = _Percentage;

        if (isHealing == false)
        {
            StartCoroutine("ShakeObject");
            StartCoroutine("ShowHitParticles");
        }

        _StatusIcon.color = Color.white;
        _HealthMeter.color = Color.green;

        if (_Percentage >= 0.80f)
        {
            _IsPulsing = false;
            _StatusIcon.sprite = _MyCharacterIcons[1];
            _HealthMeter.color = Color.green;
        }
        if (_Percentage >= 0.50f && _Percentage <= 0.79f)
        {
            _StatusIcon.color = Color.white;
            _HealthMeter.color = Color.yellow;
        }
        else if (_Percentage >= 0.20f && _Percentage <= 0.49)
        {
            _StatusIcon.sprite = _MyCharacterIcons[0];
            _IsPulsing = true;
            // StopCoroutine("PulseColor");
            StartCoroutine("PulseColor");
            _HealthMeter.color = new Color(1.0f, 0.5f, 0.2f);
        }
        else if (_Percentage <= 0)
        {
            _IsPulsing = false;
            _StatusIcon.color = Color.red;
            _HealthMeter.color = Color.red;
        }
        StopCoroutine("UpdateDelayedHealthMeter");
        StartCoroutine("UpdateDelayedHealthMeter", amount);
    }

    private IEnumerator ShakeObject()
    {
        float timer = 0.5f;
        float movement = 0f;

        RectTransform rt = GetComponent<RectTransform>();

        while (timer >= 0)
        {
            movement = Mathf.Sin(Time.deltaTime * _ShakeStrength) * 2f;
            rt.anchoredPosition = new Vector2(movement, -movement);
            timer -= Time.deltaTime;

            yield return null;
        }

        rt.anchoredPosition = Vector2.zero;
        yield return null;
    }

    private IEnumerator UpdateDelayedHealthMeter(float amount)
    {
        float delayedPercentage = _HealthDelayed.fillAmount;
        float timer = _HealthDelay;

        while (timer >= 0f)
        {
            float value = Mathf.Lerp(_Percentage, delayedPercentage, timer);
            _HealthDelayed.fillAmount = value;
            timer -= Time.deltaTime;
            yield return null;

        }

        yield return null;
    }

    private IEnumerator PulseColor()
    {
        float timer = 0f;
        while (_IsPulsing == true)
        {

            _StatusIcon.color = Color.Lerp(Color.white, Color.red, timer);
            timer = Mathf.Clamp(Mathf.Sin(Time.time * _PulseSpeed), 0f, 1f);
            // Debug.Log("Timer value is: " + timer);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator ShowHitParticles()
    {
        _PlayerHitParticles.SetActive(true);
        yield return new WaitForSeconds(1f);
        _PlayerHitParticles.SetActive(false);
    }
}
