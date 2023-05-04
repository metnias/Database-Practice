using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance() => _instance;
    private static UIController _instance;

    public static bool MenuFreeze { get; private set; }

    [SerializeField]
    private JammoControl player;

    private void Awake()
    {
        if (!_instance)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    private void Start()
    {
        popupCtrler = GetComponentInChildren<PopupController>();
        formLogIn = GetComponentInChildren<FormLogIn>();
        formSignUp = GetComponentInChildren<FormSignUp>();
        formUserPage = GetComponentInChildren<FormUserPage>();

        popupCtrler.gameObject.SetActive(false);
        formSignUp.gameObject.SetActive(false);
        formUserPage.gameObject.SetActive(false);
        formLogIn.gameObject.SetActive(true);
    }

    #region Popup

    private PopupController popupCtrler = null;

    public void CreatePopup(PopupController.PopupInfo info)
    {
        MenuFreeze = true;
        popupCtrler.UpdatePopup(info);
        popupCtrler.Activate();
    }

    internal static void ShowPopupOK(string message)
    {
        Instance().CreatePopup(
            new PopupController.PopupInfo()
            {
                message = message,
                buttons = new string[] { "OK" }
            });
    }

    public void OnPopupClose()
    {
        MenuFreeze = false;
    }

    #endregion

    #region Forms

    public enum FormType
    {
        Login, SignUp, UserPage
    }

    private FormType currentForm = FormType.Login;

    private FormLogIn formLogIn = null;
    private FormSignUp formSignUp = null;
    private FormUserPage formUserPage = null;

    public void RequestFormSwitch(FormType newForm)
    {
        if (currentForm == newForm) return;

        switch (currentForm)
        {
            case FormType.Login: formLogIn.gameObject.SetActive(false); formLogIn.ClearInputs(); break;
            case FormType.SignUp: formSignUp.gameObject.SetActive(false); formSignUp.ClearInputs(); break;
            case FormType.UserPage: formUserPage.gameObject.SetActive(false); break;
        }
        currentForm = newForm;
        switch (currentForm)
        {
            case FormType.Login:
                formLogIn.gameObject.SetActive(true);
                break;
            case FormType.SignUp:
                formSignUp.gameObject.SetActive(true);
                break;
            case FormType.UserPage:
                formUserPage.gameObject.SetActive(true);
                formUserPage.UpdateUsername(UserID);
                player.ReadyPlayerOne(UserID);
                break;
        }
        CameraController.Instance().SetGoal(currentForm);
    }

    #endregion

    public string UserID { get; private set; } = string.Empty;

    public void UpdateUserID(string newID) => UserID = newID;

}
