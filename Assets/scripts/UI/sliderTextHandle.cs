using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class sliderTextHandle : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        updateText();
    }
    public void updateText() {
        text.text = GetComponent<Slider>().value.ToString();
    }
}
