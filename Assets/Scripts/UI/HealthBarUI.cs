using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] Image profile;
    [SerializeField] TMP_Text nameTxt;

    public void SetColorAndName(Color _col,string _name)
    {
        profile.color = _col;
        nameTxt.text = _name;
    }
    public void SetHeathBar(float health)
    {
        healthBar.fillAmount = health;
        if (health < .5f)
        {
            healthBar.color = Color.red;
        }
        else
            healthBar.color = Color.green;

    }
}
