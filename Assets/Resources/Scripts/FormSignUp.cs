using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static UIController;

public class FormSignUp : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputID = null;
    [SerializeField]
    private TMP_InputField inputPW = null;
    [SerializeField]
    private TMP_InputField inputPW2 = null;

    public void ClearInputs()
    {
        inputID.text = string.Empty;
        inputPW.text = string.Empty;
        inputPW2.text = string.Empty;
    }

    public void AttemptSignUp()
    {
        if (MenuFreeze) return;

        string id = inputID.text;
        string pw = inputPW.text;
        string pw2 = inputPW2.text;

        if (pw != pw2)
        {
            ShowPopupOK("Two passwords do not match.");
            return;
        }
        if (!ValidationCheck(id, pw)) return;

        id = Encryptor.Sanitize(id);
        pw = Encryptor.GetHash(pw);
        //Debug.Log($"({pw.Length}){pw}");
        StartCoroutine(SignUpCoroutine(id, pw));
    }

    private static bool ValidationCheck(string id, string pw)
    {
        if (id.Length < 8)
        {
            ShowPopupOK("ID is too short. It must be longer than 7 characters.");
            return false;
        }
        if (pw.Length < 8)
        {
            ShowPopupOK("Password is too short. It must be longer than 7 characters.");
            return false;
        }
        int flags = (1 << 4) - 1;
        foreach (var c in pw)
        {
            if (char.IsWhiteSpace(c))
            {
                ShowPopupOK("Password should not contain white space.");
                return false;
            }
            else if (char.IsUpper(c)) flags &= ~(1 << 3);
            else if (char.IsLower(c)) flags &= ~(1 << 2);
            else if (char.IsDigit(c)) flags &= ~(1 << 1);
            else if (char.IsSymbol(c)) flags &= ~(1 << 0);
        }
        if (flags > 0)
        {
            ShowPopupOK("Password must contain capital letters, small letters, digits, and symbols.");
            return false;
        }

        return true;
    }

    private IEnumerator SignUpCoroutine(string id, string pw)
    {
        WWWForm form = new();
        form.AddField("loginUser", id);
        form.AddField("loginPass", pw);

        using (UnityWebRequest www =
            UnityWebRequest.Post("" + "http://127.0.0.1/signup.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            { Debug.LogError(www.error); ShowPopupOK(www.error); }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                switch (www.downloadHandler.text)
                {
                    case "SUCCESS_SIGNUP":
                        Instance().UpdateUserID(id);
                        Instance().RequestFormSwitch(FormType.UserPage);
                        FormLogIn.RequestIDRemember(id);
                        ShowPopupOK($"You are now registered!\nWelcome, {id}!");
                        break;
                    case "ERROR_ID_DUPE":
                        ShowPopupOK($"There's already a user named [{id}].");
                        break;
                    case "ERROR_RECENT":
                        ShowPopupOK($"[{id}] is deleted recently.\nTry later.");
                        break;
                    default:
                        Debug.LogError(www.downloadHandler.text);
                        ShowPopupOK(www.downloadHandler.text);
                        break;
                }
            }
        }
    }

    public void SwitchToLogIn()
    {
        if (MenuFreeze) return;
        Instance().RequestFormSwitch(FormType.Login);
    }
}
