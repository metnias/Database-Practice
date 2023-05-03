using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public struct PopupInfo
    {
        public string message;
        public string[] buttons;
        public UnityAction[] actions;
    }

    [SerializeField]
    private TMP_Text txtMessage = null;

    [SerializeField]
    private GameObject btnPrefab = null;

    private readonly GameObject[] buttons = new GameObject[3];

    public void UpdatePopup(PopupInfo info)
    {
        txtMessage.text = info.message;

        info.buttons ??= new string[] { "OK" };
        info.actions ??= new UnityAction[info.buttons.Length];

        for (int i = 0; i < buttons.Length; ++i)
            if (buttons[i] != null) buttons[i].SetActive(false);

        for (int i = 0; i < info.buttons.Length && i < buttons.Length; ++i)
        {
            if (buttons[i] == null)
                buttons[i] = Instantiate(btnPrefab, gameObject.transform);
            buttons[i].SetActive(true);

            float posX = (Mathf.Min(info.buttons.Length, buttons.Length) - 1) * -80f;
            posX += 160f * i;
            (buttons[i].transform as RectTransform).anchoredPosition =
                new Vector2(posX, 20f);

            var text = buttons[i].GetComponentInChildren<TMP_Text>();
            if (i < info.buttons?.Length && !string.IsNullOrEmpty(info.buttons[i])) text.text = info.buttons[i];

            var btn = buttons[i].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(Deactivate);
            if (i < info.actions?.Length && info.actions[i] != null)
                btn.onClick.AddListener(info.actions[i]);
        }

    }

    public void Activate()
    {
        gameObject.SetActive(true);
        for (int i = buttons.Length - 1; i >= 0; --i)
            if (buttons[i] != null)
            { buttons[i].GetComponent<Button>().Select(); break; }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        UIController.Instance().OnPopupClose();
    }
    
}
