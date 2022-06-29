using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class phaseRandomizer : MonoBehaviour
{
    public string[] phases;
    private TextMeshProUGUI t;
    void Start()
    {
        t = GetComponent<TextMeshProUGUI>();
        t.text = phases[Random.Range(0, phases.Length)];
    }
}
