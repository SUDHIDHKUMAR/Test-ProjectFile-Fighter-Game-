using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FighterGame.UI
{
    public class UIScreen : MonoBehaviour
    {
        public int screenIndex;
        public virtual void InitializeScreen()
        {
            gameObject.SetActive(true);
        }
        public virtual void DeinitializeScreen()
        {
            gameObject.SetActive(false);
        }
    }
}