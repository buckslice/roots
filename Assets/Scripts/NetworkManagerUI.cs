using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour {

    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMPro.TMP_InputField ipField;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject background;

    void Awake() {
        startButton.gameObject.SetActive(false);

        hostButton.onClick.AddListener(OnHostClick);
        joinButton.onClick.AddListener(OnJoinClick);
        startButton.onClick.AddListener(OnStartClick);

        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += SceneChanged;
    }

    void SceneChanged(Scene arg0, Scene arg1) {
        if (arg1.name != "MainMenu") {
            hostButton.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
            ipField.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
        }
    }

    void OnHostClick() {
        NetworkManager.Singleton.StartHost();
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        ipField.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
    }

    void OnJoinClick() {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (ipField.text.Length > 0) {
            var splits = ipField.text.Split(":");
            var address = splits[0];
            var port = splits.Length > 1 ? int.Parse(splits[1]) : 42420;
            transport.SetConnectionData(address, (ushort)port);
            Debug.Log($"setting connection data, address: {address}, port: {port}");
        } else {
            transport.SetConnectionData("127.0.0.1", 42420);
        }
        NetworkManager.Singleton.StartClient();
    }

    void OnStartClick() {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

}
