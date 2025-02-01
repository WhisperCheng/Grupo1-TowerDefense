using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crown Data", menuName = "Crown Data")]
public class CrownsData : ScriptableObject
{
    [SerializeField] private GameObject crownLevelOne;
    [SerializeField] private GameObject crownLevelTwo;

    public GameObject CrownLevelOne { get { return crownLevelOne; } }
    public GameObject CrownLevelTwo { get { return crownLevelTwo; } }
}
