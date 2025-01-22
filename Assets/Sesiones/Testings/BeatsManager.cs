using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class BeatsManager : MonoBehaviour
{
    [SerializeField]  private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;

    private void Update()
    {
        foreach (Intervals interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
            interval.CheckForNewIntervals(sampledTime);
        }
        
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;
    private int _lastInterval;

    public float GetIntervalLength(float bpm) {
        return 60f / (bpm * _steps);
    }

    public void CheckForNewIntervals(float interval)
    {
        if (Mathf.FloorToInt(interval) != _lastInterval) {
            _lastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
        }
    } 
}