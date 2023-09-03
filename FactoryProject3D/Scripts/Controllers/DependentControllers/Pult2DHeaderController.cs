using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pult2DHeaderController : MonoBehaviour
{
    private TextMeshProUGUI header;

    private void Awake()
    {
        header = GetComponent<TextMeshProUGUI>();
        if (header)
        {
            header.text = string.Empty;
        }
    }

    private void Update()
    {
        if (header && string.IsNullOrEmpty(header.text))
        {
            var trenObject = transform.parent.GetComponentInChildren<TrenObject>();
            if (trenObject && trenObject.IsRegistrated)
            {
                header.text = trenObject.TrenName;
            }
        }
    }
}
