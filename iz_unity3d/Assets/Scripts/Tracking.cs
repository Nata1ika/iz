using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Tracking : MonoBehaviour
{
    [SerializeField] TrackElement[] _elements;

    private void Start()
    {
        foreach (var element in _elements)
        {
            element.Init();
        }
    }

    private void OnDestroy()
    {
        foreach (var element in _elements)
        {
            element.Stop();
        }
    }
}

[System.Serializable]
public class TrackElement
{
    [SerializeField] DefaultTrackableEventHandler _target;
    [SerializeField] GameObject _obj;

    public void Init()
    {
        _target.OnTrackingFoundEvent += OnTrackingFound;
        _target.OnTrackingLostEvent += OnTrackingLost;
    }

    public void Stop()
    {
        _target.OnTrackingFoundEvent -= OnTrackingFound;
        _target.OnTrackingLostEvent -= OnTrackingLost;
    }

    private void OnTrackingLost()
    {
        _obj.SetActive(false);
    }

    private void OnTrackingFound()
    {
        _obj.SetActive(true);
    }
}
