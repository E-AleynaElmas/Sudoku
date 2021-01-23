using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Text time_text;

    public void DisPlayTime()
    {
        time_text.text = Clock.instance.GetCurrentTimeText().text;
    }
}
