using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class KeyboardControl : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Selectable self;

    private void Awake()
    {
        self = GetComponent<Selectable>();
    }

    [SerializeField]
    private UnityEvent onEnterPressed = null;

    private bool selected = false;

    private void Update()
    {
        if (!selected) return;

        Selectable next = null;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                next = self.FindSelectableOnUp();
            else
                next = self.FindSelectableOnDown();
        }
        if (next) { next.Select(); return; }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            onEnterPressed?.Invoke();
    }

    public void OnSelect(BaseEventData eventData) => selected = true;

    public void OnDeselect(BaseEventData eventData) => selected = false;
}
