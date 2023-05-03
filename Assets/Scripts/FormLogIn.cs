using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UIController;

public class FormLogIn : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputID = null;
    [SerializeField]
    private TMP_InputField inputPW = null;

    private void Start()
    {
        var toggle = GetComponentInChildren<Toggle>();
        toggle.isOn = RememberID;
        if (toggle.isOn) inputID.text = PlayerPrefs.GetString(KEY_SAVEDID, string.Empty);
    }

    public void ClearInputs()
    {
        if (RememberID) inputID.text = PlayerPrefs.GetString(KEY_SAVEDID, string.Empty);
        else inputID.text = string.Empty;
        inputPW.text = string.Empty;
    }

    public void AttemptLogIn()
    {
        if (MenuFreeze) return;

        string id = inputID.text;
        string pw = inputPW.text;
        id = Encryptor.Sanitize(id);
        pw = Encryptor.GetHash(pw);

        StartCoroutine(LogInCoroutine(id, pw));
    }

    private IEnumerator LogInCoroutine(string id, string pw)
    {
        WWWForm form = new();
        form.AddField("loginUser", id);
        form.AddField("loginPass", pw);

        using (UnityWebRequest www =
            UnityWebRequest.Post("" + "http://127.0.0.1/login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            { Debug.LogError(www.error); ShowPopupOK(www.error); }
            else
            {
                switch (www.downloadHandler.text)
                {
                    case "SUCCESS_LOGIN":
                        Instance().UpdateUserID(id);
                        RequestIDRemember(id);
                        Instance().RequestFormSwitch(FormType.UserPage);
                        break;
                    case "ERROR_WRONG_ID":
                        ShowPopupOK($"[{id}] is not registered ID.");
                        break;
                    case "ERROR_WRONG_PW":
                        ShowPopupOK($"Your password is wrong.");
                        break;
                    case "ERROR_REMOVED":
                        ShowPopupOK($"[{id}] is deleted.");
                        break;
                    default:
                        Debug.LogError(www.downloadHandler.text);
                        ShowPopupOK(www.downloadHandler.text);
                        break;
                }
            }
        }
    }

    private const string KEY_REMEMBER = "LOGIN_REMEMBER_ID";
    private const string KEY_SAVEDID = "LOGIN_ID_SAVED";

    private static bool RememberID => PlayerPrefs.GetInt(KEY_REMEMBER, 0) > 0;

    public static void RequestIDRemember(string id)
    {
        if (RememberID) PlayerPrefs.SetString(KEY_SAVEDID, id);
    }

    public void ToggleRememberID(Toggle trigger)
    {
        PlayerPrefs.SetInt(KEY_REMEMBER, trigger.isOn ? 1 : 0);
        if (!trigger.isOn) PlayerPrefs.SetString(KEY_SAVEDID, string.Empty);
    }

    public void SwitchToSignUp()
    {
        if (MenuFreeze) return;
        Instance().RequestFormSwitch(FormType.SignUp);
    }
}
