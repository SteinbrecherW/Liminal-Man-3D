using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldReader : MonoBehaviour
{
    public void SubmitName(string arg0)
    {
        ScoreManager.Instance.PlayerName = arg0;
        gameObject.SetActive(false);
        ScoreManager.Instance.Save();
        Debug.Log(arg0);
    }
}
