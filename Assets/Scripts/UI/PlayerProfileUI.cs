using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerProfileUI : MonoBehaviour
{
    [SerializeField] Image picture;
    [SerializeField] TMP_Text nameText;

    public void SetColorAndName(Color _col,string _name)
    {
        picture.color = _col;
        nameText.text = _name;
    }
}
