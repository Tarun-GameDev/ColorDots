using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChangeColor : MonoBehaviour
{
    Image image;
    [SerializeField] Color selectedColor;
    [SerializeField] Color deSelectColor;

    void Start()
    {
        if(image == null)
            image = GetComponent<Image>();
    }

    public void ChangeColor(bool _selected)
    {
        if (_selected)
            image.color = selectedColor;
        else
            image.color = deSelectColor;
    }
}
