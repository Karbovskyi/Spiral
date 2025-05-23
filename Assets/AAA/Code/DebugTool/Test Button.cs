using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

public class TestButton : MonoBehaviour
{
    [FormerlySerializedAs("text")] public TMP_Text _text;

    [Inject] private SpiralStats _spiralStats;
    [Inject] private BulletStats _bulletStats;
    [Inject] private ShooterStats _shooterStats;

    public List<TestingStats> _TestingStats;

    private void Awake()
    {
        _text.text = "Test N" + PlayerPrefs.GetInt("TestButton");
    }

    public void OnClick()
    {
        int i = PlayerPrefs.GetInt("TestButton");

        if (i + 1 >= _TestingStats.Count)
            i = 0;
        else
            i += 1;
        
        PlayerPrefs.SetInt("TestButton", i);
        UpdateStats(i);
        SceneManager.LoadScene("BootScene");
    }

    private void UpdateStats(int i)
    {
        _spiralStats.RadiusStep = _TestingStats[i].RadiusStep;
        _spiralStats.SpiralThickness = _TestingStats[i].SpiralThickness;
        _spiralStats.Clockwise = _TestingStats[i].Clockwise;
        _spiralStats.Center = _TestingStats[i].Center;
        _spiralStats.AngleOffsetDeg = _TestingStats[i].AngleOffsetDeg;
        _spiralStats.AngleOffsetDegChangeSpeed = _TestingStats[i].AngleOffsetDegChangeSpeed;
        _spiralStats.RadiusStart = _TestingStats[i].RadiusStart;
        _shooterStats.ShootingInterval = _TestingStats[i].ShootingInterval;
        _shooterStats.RotatingSpeed = _TestingStats[i].RotatingSpeed;
        _shooterStats.AutoMove = _TestingStats[i].AutoMove;
        _bulletStats.Speed = _TestingStats[i].BulletSpeed;
        _bulletStats.Distance = _TestingStats[i].BulletDistance;
        _bulletStats.Radius = _TestingStats[i].BulletRadius;
    }
}
