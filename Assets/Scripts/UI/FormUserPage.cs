using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static UIController;

public class FormUserPage : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtUsername = null;

    public void UpdateUsername(string id)
    {
        txtUsername.text = id;
    }

    public void ConfirmDeletion()
    {
        if (MenuFreeze) return;
        Instance().CreatePopup(new PopupController.PopupInfo()
        {
            message = "Are you sure?\nYou cannot register again for 5 minutes!",
            buttons = new[] {"Delete", "Cancel"},
            actions = new UnityAction[] {DeleteAccount, null}
        });
    }

    private void DeleteAccount()
    {
        StartCoroutine(DeleteAccountCoroutine(Instance().UserID));
    }

    private IEnumerator DeleteAccountCoroutine(string id)
    {
        WWWForm form = new();
        form.AddField("loginUser", id);

        using (UnityWebRequest www =
            UnityWebRequest.Post("" + "http://127.0.0.1/deleteuser.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            { Debug.LogError(www.error); ShowPopupOK(www.error); }
            else
            {
                switch (www.downloadHandler.text)
                {
                    case "SUCCESS_DELETE":
                        LogOut();
                        ShowPopupOK($"You are unregistered.\nGood bye, {id}!");
                        break;
                    case "ERROR_WRONG_ID":
                        ShowPopupOK($"There's no user named [{id}].");
                        break;
                    default:
                        Debug.LogError(www.downloadHandler.text);
                        ShowPopupOK(www.downloadHandler.text);
                        break;
                }
            }
        }
    }

    

    public void LogOut()
    {
        if (MenuFreeze) return;
        Instance().UpdateUserID(string.Empty);
        Instance().RequestFormSwitch(FormType.Login);
    }
}
