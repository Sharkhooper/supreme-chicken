using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrowOverTime : MonoBehaviour
{
    private TextMeshProUGUI tmpro;
    public float multiplier;

    private void Start() {
        tmpro = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        tmpro.fontSize += Time.deltaTime * multiplier;
    }
}
