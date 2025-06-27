using MySql.Data.MySqlClient;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogOn : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Transform loginButton;
    public Transform registerButton;
    public Transform Exit;
    public Transform ExitBack;
    public Transform Exitsure;
    public Transform ExitCancel;
    public Transform SetButton;
    public Transform ExitSetButton;
    public Image img;
    public Text statusText;
    public Toggle AgreeToole;

    private bool Isconnect = true;
    private string authToken;
    private bool Agree = false;

    private string actualText = "";

    private void Awake()
    {
        statusText.gameObject.SetActive(false);
        ExitBack.gameObject.SetActive(false);
        img.gameObject.SetActive(false);
        AgreeToole.onValueChanged.AddListener(OnToggleValueChanged);
        passwordInput.onValueChanged.AddListener(UpText);
        usernameInput.onValueChanged.AddListener(UpdateUserName);
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    }

    private void UpdateUserName(string text)
    {
        if (text.Length > 10)
        {
            usernameInput.text = text.Substring(0, 10);
            StartCoroutine(ShowMessage("�û������Ȳ��ܳ���10λ", 2f));
        }
    }

    private void UpText(string text)
    {
        if (text.Length > 16)
        {
            passwordInput.text = text.Substring(0, 16);
            StartCoroutine(ShowMessage("���볤�Ȳ��ܳ���16λ", 2f));
        }

        if(text.Length> actualText.Length)
        {
            char newChar = text[text.Length - 1];
            actualText += newChar;

            string maskedText = new string('��', actualText.Length);
            passwordInput.text = maskedText;

            passwordInput.caretPosition = maskedText.Length;
        }
        else
        {
            actualText = actualText.Substring(0, text.Length);
        }
    }

    void OnToggleValueChanged(bool isOn)
    {
        UpdateStatus(isOn);
    }

    void UpdateStatus(bool isOn)
    {
        if(isOn)
        {
            Agree = true;
        }
        else
        {
            Agree = false;
        }
    }

    private void Start()
    {
        StartCoroutine(WarmupConnectionPool());

        loginButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(LoginProcess()));
        if(loginButton == null)
        {
            Debug.LogError("Login button is not assigned in the inspector.");
            return;
        }
        registerButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(RegisterProcess()));
        Exit.GetComponent<Button>().onClick.AddListener(exit);
        SetButton.GetComponent<Button>().onClick.AddListener(Set_UP);
        ExitSetButton.GetComponent<Button>().onClick.AddListener(ExitSet);
    }

    private static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // ��������֤�飨������Ч/��ǩ��֤�飩
        return true;
    }

    private void ExitSet()
    {
        if (Isconnect)
        {
            usernameInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            registerButton.gameObject.SetActive(true);
            statusText.gameObject.SetActive(false);
        }
        if(!Isconnect) img.gameObject.SetActive(true);
        Exit.gameObject.SetActive(true);
        SetButton.gameObject.SetActive(true);
    }

    void Set_UP()
    {
        img.gameObject.SetActive(false);
        usernameInput.gameObject.SetActive(false);
        passwordInput.gameObject.SetActive(false);
        loginButton.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        statusText.gameObject.SetActive(false);
        Exit.gameObject.SetActive(false);
    }

    private IEnumerator WarmupConnectionPool()
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{PublicVar.ServerUrl}/ping"))
        {
            // ���ó�ʱʱ��
            request.timeout = 5; // 5�볬ʱ

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                //isConnected = true;
                Debug.Log("Ԥ���ӳɹ�������������");
            }
            else
            {
                Isconnect = false;
                img.gameObject.SetActive(true);
                usernameInput.gameObject.SetActive(false);
                passwordInput.gameObject.SetActive(false);
                loginButton.gameObject.SetActive(false);
                registerButton.gameObject.SetActive(false);
                statusText.gameObject.SetActive(false);
                Debug.LogError("Ԥ����ʧ��");
            }
            
        }
        yield return null;
    }

    private void exit()
    {
        img.gameObject.SetActive(false);
        ExitBack.gameObject.SetActive(true);
        usernameInput.gameObject.SetActive(false);
        passwordInput.gameObject.SetActive(false);
        loginButton.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        statusText.gameObject.SetActive(false);
        Exitsure.GetComponent<Button>().onClick.AddListener(exit_sure);
        ExitCancel.GetComponent<Button>().onClick.AddListener(exit_cancel);
    }

    private void exit_sure()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Debug.Log("�˳���Ϸ");
    }

    private void exit_cancel()
    {
        ExitBack.gameObject.SetActive(false);
        if (Isconnect)
        {
            usernameInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            registerButton.gameObject.SetActive(true);
            statusText.gameObject.SetActive(false);
        }
        else
        {
             img.gameObject.SetActive(true);
        }
    }

    private IEnumerator LoginProcess()
    {
        var username = usernameInput.text.Trim();
        var password = actualText.Trim();

        AuthRequest requestBody = new AuthRequest { username = username, password = password };
        string jsonData = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(ShowMessage("�û������벻��Ϊ��", 2f));
            yield break;
        }

        if(!Agree)
        {
            StartCoroutine(ShowMessage("��ͬ���û�Э��", 2f));
            yield break;
        }

        using (UnityWebRequest request = new UnityWebRequest($"{PublicVar.ServerUrl}/login", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // ����JSON��Ӧ
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                authToken = response.token;
                PublicVar.token = authToken;
                Debug.Log("��֤����: " + authToken);
                // �������Ƶ�PlayerPrefs
                PlayerPrefs.SetString("AuthToken", authToken);

                PublicVar.playerName = username;
                Debug.Log(response.userId);
                // �ڵ�¼�ɹ�������UID
                PublicVar.UID = response.userId;
                Debug.Log("�û�ID: " + PublicVar.UID);
                Debug.Log("�������: " + PublicVar.playerName);
                StartCoroutine(ShowMessage("��¼�ɹ�", 1f));
                SceneManager.LoadScene("MainSet");
            }
            else
            {
                if (request.responseCode == 401)
                {
                    ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                    StartCoroutine(ShowMessage(error.message, 2f));
                }
                else
                {
                    StartCoroutine(ShowMessage("���Ӵ���: " + request.error, 2f));
                }
            }
            //Debug.LogError("���ݿ����");
            //StartCoroutine(ShowMessage("���ӳ�ʱ��������", 2f));
            yield return null;
        }
    }

    private IEnumerator RegisterProcess()
    {
        var username = usernameInput.text.Trim();
        var password = actualText.Trim();

        AuthRequest requestBody = new AuthRequest { username = username, password = password };
        string jsonData = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(ShowMessage("�û������벻��Ϊ��", 2f));
            yield break;
        }

        using (UnityWebRequest request = new UnityWebRequest($"{PublicVar.ServerUrl}/register", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            { 
                StartCoroutine(ShowMessage("ע��ɹ�", 2f));
                usernameInput.text = "";
                passwordInput.text = "";
            }
            else
            {
                // ����״̬����ʾ��ͬ����
                if (request.responseCode == 409)
                {
                    StartCoroutine(ShowMessage("�û����Ѵ���", 2f));
                }
                else if (request.responseCode == 400)
                {
                    StartCoroutine(ShowMessage("�û����������ʽ����", 2f));
                }
                else
                {
                    // ��ӡ���������ص�ԭʼ������Ϣ
                    string errorMessage = request.downloadHandler.text;
                    Debug.LogError($"ע��ʧ��: {errorMessage}");
                    StartCoroutine(ShowMessage("ע��ʧ�ܣ���������", 2f));
                }
                yield break;
            }
            //catch (MySqlException ex)
            //{
            //    Debug.LogError($"ע��ʧ��: {ex.Number} - {ex.Message}");
            //    StartCoroutine(ShowMessage("ע��ʧ�ܣ�������", 2f));
            //}
            yield return null;
        }
    }

    private IEnumerator ShowMessage(string message, float duration = 3f)
    {
        statusText.gameObject.SetActive(true);
        statusText.text = message;
        yield return new WaitForSeconds(duration);
        statusText.gameObject.SetActive(false);
    }
    [System.Serializable]
    private class LoginResponse
    {
        public string token;
        public int userId; // �����ֶ�
    }
    [System.Serializable]
    private class ErrorResponse
    {
        public string message;
    }

    [System.Serializable]
    private class AuthRequest
    {
        public string username;
        public string password;
    }
}
