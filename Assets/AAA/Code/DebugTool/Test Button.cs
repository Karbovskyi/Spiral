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

    [Inject]
    public void Construct(SpiralStats spiralStats, BulletStats bulletStats, ShooterStats shooterStats)
    {
        _spiralStats = spiralStats;
        _bulletStats = bulletStats;
        _shooterStats = shooterStats;
        
        UpdateStats(StaticHelper.i);
    }
    private void Awake()
    {
        _text.text = "Test N" + StaticHelper.i;
    }

    public void OnClick()
    {
        if (StaticHelper.i + 1 >= _TestingStats.Count)
            StaticHelper.i = 0;
        else
            StaticHelper.i += 1;
        
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
