using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTemplateUI : MonoBehaviour
{
    [SerializeField]
    private Image icon;

    public Image GetIcon()
    {
        return icon;
    }

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}
