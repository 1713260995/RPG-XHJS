using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipWindow : MonoBehaviour
{
    [Header("手动赋值")]
    public Text content;
    public Button confirmBtn;
    public Button closeBtn;
    [Header("自动赋值")]
    public Action confirmAction;
    public Action closeAction;

    // Start is called before the first frame update
    void Start()
    {
        confirmBtn.onClick.AddListener(ConfirmEvent);
        closeBtn.onClick.AddListener(closeEvent);
    }

    public void OpenTips(string text, Action confirm = null, Action close = null)
    {
        gameObject.SetActive(true);
        content.text = text;
    }


    private void closeEvent()
    {
        if (closeAction == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            closeAction();
            closeAction = null;
        }
    }

    private void ConfirmEvent()
    {
        if (confirmAction == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            confirmAction();
            confirmAction = null;
        }
    }
}
