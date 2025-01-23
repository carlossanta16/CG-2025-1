using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseToBeat : MonoBehaviour
{
    [SerializeField] private float _pulseSize = 1.15f;
    [SerializeField] private float _returnSpeed = 5f;
    private Vector3 _startSize;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _startSize, Time.deltaTime * _returnSpeed);
    }

    public void Pulse()
    {
        transform.localScale = _startSize * _pulseSize;
    }
}
