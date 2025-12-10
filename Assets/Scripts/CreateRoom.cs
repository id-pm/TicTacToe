using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    [SerializeField]
    Text textBoxCode;
    [SerializeField]
    Text name;
    [SerializeField]
    Text GetCode;
    [SerializeField]
    GameObject gamePlane;
    [SerializeField]
    GameObject menu;
    [SerializeField]
    GameObject canvas;
    private Client onlineClient;

    public void CreateNewRoom()
    {
        Debug.Log("create");
        onlineClient = Client.GetClient();
        if (name.text.Length < 2)
            name.text = "Player1";
        string result = onlineClient.StartOnline(name.text);
        if (result.Contains("CODE") || true)
        {
            GetCode.text = result;
            WaitOpponent();
        }
    }
    public void EnterTheRoom()
    {
        Debug.Log("enter");
        onlineClient = Client.GetClient();
        if (name.text.Length < 2)
            name.text = "Player1";
        if (textBoxCode.text.Length != 5)
        {
            Debug.Log("םו גטירכמ");
            return;
        }
        string result = onlineClient.StartOnline(name.text, textBoxCode.text);
        Debug.Log(result);
        if (result.Contains("OK"))
        {
            StartOnline(result);
        }
    }
    private async void WaitOpponent()
    {
        string result = await onlineClient.ReceiveMessageAsync();
        if (result.Contains("OK"))
        {
            StartOnline(result);
        }

    }
    private async void StartOnline(string result)
    {
        Debug.Log("Start");
        MainScript main = gamePlane.transform.GetComponent<MainScript>();
        main.playerSideIsTic = result.Contains("true");
        main.RestartGame(false);
        menu.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
    }
}
