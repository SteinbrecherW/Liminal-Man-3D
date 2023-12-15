using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightBehavior : MonoBehaviour
{
    IEnumerator _battery;

    Light _light;

    [SerializeField] float _chargeDuration = 2;
    [SerializeField] int _flickerCount = 5;
    
    [SerializeField] RectTransform _flashlightBar;

    [SerializeField] RawImage _bar;

    [SerializeField] float _barMaxValue = 10;
    float _barValue;
    float _lerpValue = 0;

    bool _toggled = false;
    public bool Toggled
    {
        get => _toggled;
        set
        {
            _toggled = value;
            if (_toggled && _barValue >= _barMaxValue)
            {
                _barValue = 0;
                _flashlightBar.sizeDelta = new Vector2(0, 100);
                _light.enabled = true;
                _bar.color = new Color32(200, 57, 88, 255);
                StartCoroutine(_battery);
            }
            else
            {
                _light.enabled = false;
                StopCoroutine(_battery);
                _battery = RunDownBattery();
                ChargeCount--;
            }
        }
    }

    [SerializeField] int _startingChargeCount = 2;
    [SerializeField] int _maxChargeCount = 3;

    int _chargeCount;
    public int ChargeCount
    {
        get => _chargeCount;
        set
        {
            if (value > _maxChargeCount)
                _chargeCount = _maxChargeCount;
            else if (value < 0)
                _chargeCount = 0;
            else
                _chargeCount = value;
        }
    }

    void Start()
    {
        _battery = RunDownBattery();

        _light = GetComponent<Light>();
        _light.enabled = false;

        _chargeCount = _startingChargeCount;

        _barValue = _barMaxValue;
    }

    private void FixedUpdate()
    {
        if (Toggled)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _light.range))
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                    hit.collider.gameObject.GetComponent<EnemyBehavior>().Flash();
            }
        }
        else if(_barValue < _barMaxValue)
        {
            _barValue += Time.deltaTime;
            _lerpValue = _barValue / _barMaxValue;
            _flashlightBar.sizeDelta = new Vector2(Mathf.Lerp(0, 3.2f, _lerpValue), 100);
            if(_lerpValue >= 1)
                _bar.color = new Color32(57, 62, 229, 255);
        }
    }
    
    IEnumerator RunDownBattery()
    {
        yield return new WaitForSeconds(_chargeDuration / 2);

        float _flickerLength = _chargeDuration / (4 * _flickerCount);
        for(int i = 0; i < _flickerCount; i++)
        {
            _light.enabled = false;
            yield return new WaitForSeconds(_flickerLength);
            _light.enabled = true;
            yield return new WaitForSeconds(_flickerLength);
        }

        Toggled = false;
    }
}
