using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    Text notificationText;
    [SerializeField]
    Text buttonChangeSide;
    [SerializeField]
    GameObject ticPlayer;
    [SerializeField]
    GameObject tacPlayer;
    [SerializeField]
    GameObject winLine;
    [SerializeField]
    GameObject[] restartMenu;
    [SerializeField]
    AudioClip DrowingTic;
    [SerializeField]
    AudioClip DrowingTac;
    [SerializeField]
    AudioSource source;
    GameObject winLineTemp;
    [SerializeField]
    GameObject SoundON;
    [SerializeField]
    GameObject SoundOFF;

    delegate void EventMove(string gameCellName);
    event EventMove clickedOnACell;

    Client onlineClient = null;
    public bool playerSideIsTic = false;
    int emptyCell = 9;
    string[] cellTable;
    private Vector2 startPos;
    public static bool disableTouch = true;


    private void Start()
    {
        SettingClass.switchSound += SwitchSound;
        SwitchSound(SettingClass.Music);
        DrawGamePlane.IsDrawnEvent += ModeSetup;
        cellTable = new[] { "cell:0.0", "cell:0.1", "cell:0.2", "cell:1.0", "cell:1.1", "cell:1.2", "cell:2.0", "cell:2.1", "cell:2.2" };
        notificationText.text = "Оберіть місце для\nігрового поля";
    }
    public void SwitchSound(bool sound)
    {
        SoundON.gameObject.SetActive(!sound);
        SoundOFF.gameObject.SetActive(sound);
    }
    public void PressedTheSwitch(bool sound)
    {
        SettingClass.SwitchChanged(sound);
    }
    
    private void ModeSetup()
    {
        switch (SettingClass.modeSelection)
        {
            case SettingClass.GameMode.Online:
                {
                    onlineClient = Client.GetClient();
                    buttonChangeSide.gameObject.SetActive(false);
                    clickedOnACell += WaitPlayerMove;
                    disableTouch = true;
                    if (playerSideIsTic)
                    {
                        notificationText.text = "Ваш хід!";
                        playerSideIsTic = true;
                        disableTouch = false;
                    }
                    else WaitOpponentMove();
                    break;
                }
            case SettingClass.GameMode.Offline:
                {
                    notificationText.text = "Хiд хрестикiв";
                    playerSideIsTic = true;
                    disableTouch = false;
                    buttonChangeSide.gameObject.SetActive(false);
                    clickedOnACell += OfflineGame;
                    break;
                }
            case SettingClass.GameMode.WhithBot:
                {
                    notificationText.text = "";
                    playerSideIsTic = SettingClass.side;
                    if (!playerSideIsTic)
                    {
                        disableTouch = true;
                        SelectRandomCell(ticPlayer);
                        emptyCell--;
                        disableTouch = false;
                    }
                    else disableTouch = false;
                    notificationText.text = "Ваш хід!";
                    buttonChangeSide.gameObject.SetActive(true);
                    clickedOnACell += GameWithBot;
                    break;
                }
        }
    }
    void Update()
    {
        if (disableTouch) return;
        //foreach (Touch touch in Input.touches)
        foreach (Touch touch in InputHelper.GetTouches())
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, 1000) && touch.position.x - startPos.x < 10)
                {
                    if (hit.transform.tag != "gameCell" || hit.transform.childCount > 0) return;
                    clickedOnACell.Invoke(hit.transform.name);
                    CheckWin();
                }
            }
        }
    }
    private void SetParent(string cellName, GameObject turn)
    {
        GameObject cell = GameObject.Find(cellName);
        GameObject dd = Instantiate(turn, new Vector3(0, 0, 0), Quaternion.identity);
        dd.transform.SetParent(cell.transform, false);
        if (turn.tag == ticPlayer.tag) StartCoroutine(PlayDrawingTicAudio());
        else source.PlayOneShot(DrowingTac);
    }
    IEnumerator PlayDrawingTicAudio()
    {
        source.PlayOneShot(DrowingTic);
        yield return new WaitForSeconds(1.2f);
        source.PlayOneShot(DrowingTic);
    }
    private void PlayWinLine(int whatWin)
    {
        float rotation = 90f;
        float position = 0.33f;
        float z = 1;
        winLineTemp = Instantiate(winLine, new Vector3(0, 0, 0), Quaternion.identity);
        winLineTemp.transform.SetParent(gameObject.transform, false);
        if (whatWin > 5)
        {
            rotation = 45f;
            if (whatWin == 6)
                rotation *= -1;
            z += 0.22f;
        }
        else if (whatWin > 2)
        {
            rotation = 0f;
        }
        winLineTemp.transform.DOScaleZ(z, 2);
        winLineTemp.transform.Rotate(0f, rotation, 0f);
        int furfur = 0;
        bool df = true;
        do
        {
            winLineTemp.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            furfur++;
            if (winLineTemp.transform.position.y.ToString("0.00") == "0.00") df = false;
            if (furfur > 100) df = false;
        } while (df);
        //dd.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        //textWhoseMove.text = $"{dd.transform.position.x},{dd.transform.position.y},{dd.transform.position.x}";
        switch (whatWin)
        {
            case 0:
                winLineTemp.transform.position = new Vector3(0.0f, position, 0.0f);
                break;
            case 2:
                winLineTemp.transform.position = new Vector3(0.0f, -position, 0.0f);
                break;
            case 3:
                winLineTemp.transform.position = new Vector3(-position, 0.0f, 0.0f);
                break;
            case 5:
                winLineTemp.transform.position = new Vector3(position, 0.0f, 0.0f);
                break;
        }
    }
    private void GameWithBot(string gameCell)
    {
        disableTouch = true;
        SetParent(gameCell, playerSideIsTic ? ticPlayer : tacPlayer);
        CheckWin();
        notificationText.text = "";
        emptyCell--;
        if (emptyCell == 0) return;
        StartCoroutine(BotMove());
        
        //textWhoseMove.text = "ВАШ ХІД!";
        
    }
    IEnumerator BotMove()
    {
        yield return new WaitForSeconds(1.5f);
        if (FindWinCombination(playerSideIsTic ? tacPlayer : ticPlayer, playerSideIsTic ? ticPlayer : tacPlayer))
        {
            disableTouch = false;
        }
        else
        {
            SelectRandomCell(playerSideIsTic ? tacPlayer : ticPlayer);
            disableTouch = false;
        }
        yield return new WaitForSeconds(1.5f);
        CheckWin();
        notificationText.text = "Ваш хід!";
    }
    private bool SelectRandomCell(GameObject gameSide)
    {
        do
        {
            var rand = new System.Random();
            GameObject cell = GameObject.Find(cellTable[rand.Next(9)]);
            if (cell.transform.childCount == 0)
            {
                SetParent(cell.name, gameSide);
                emptyCell--;
                return true;
            }
        } while (true);
    }
    public void RestartGame(bool changeSide)
    {
        disableTouch = true;
        emptyCell = 9;
        if (changeSide)
        {
            SettingClass.side = !SettingClass.side;
        }
        GameObject[] gameCells = GameObject.FindGameObjectsWithTag("gameCell");
        foreach (GameObject gameCell in gameCells)
        {
            if (gameCell.transform.childCount != 0)
            {
                Destroy(gameCell.transform.GetChild(0).gameObject);
            }
        }
        foreach (GameObject restartMenu in restartMenu)
        {
            restartMenu.SetActive(false);
        }
        Destroy(winLineTemp);
        if (DrawGamePlane.isDrawn)
        {
            ModeSetup();
        }
        else
        {
            notificationText.text = "Оберіть місце для\nігрового поля";
        }
        
    }
    private bool FindWinCombination(GameObject botTag,GameObject enemyTag = null)
    {
        int[,] Wins = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
        GameObject[] gameCells = GameObject.FindGameObjectsWithTag("gameCell");
        for (int i = 0; i < 9; i++)
        {
            if (gameCells[i].transform.childCount > 0)
            {
                continue;
            }
            string[] tagCell = new string[9];
            for (int j = 0; j < 9; j++)
            {
                if (gameCells[j].transform.childCount != 0)
                    tagCell[j] = gameCells[j].transform.GetChild(0).transform.tag;
            }
            tagCell[i] = botTag.gameObject.tag;
            for (int g = 0; g < 9; g++)
            {
                try
                {
                    string temptag = tagCell[Wins[g, 0]];
                    if (string.IsNullOrEmpty(temptag)) continue;
                    if (temptag == tagCell[Wins[g, 1]] && temptag == tagCell[Wins[g, 2]])
                    {
                        SetParent(cellTable[i], playerSideIsTic ? tacPlayer : ticPlayer);
                        emptyCell--;
                        if (enemyTag == null)
                        {
                            Debug.Log("Нашел вражескую победу");
                        }
                        else
                        {
                            Debug.Log("Нашел победную");
                        }
                        return true;
                    }
                }
                catch
                {
                }

            }
        }
        if (enemyTag == null) return false;
        return FindWinCombination(enemyTag);
    }
    private void OfflineGame(string gameCellName)
    {
        disableTouch = true;
        SetParent(gameCellName, playerSideIsTic ? ticPlayer : tacPlayer);
        emptyCell--;
        StartCoroutine(WhoseMove());
    }
    IEnumerator WhoseMove()
    {
        yield return new WaitForSeconds(1.3f);
        notificationText.text = playerSideIsTic ? "Хід нуликів" : "Хід хрестиків";
        Debug.Log("ну типу");
        playerSideIsTic = !playerSideIsTic;
        disableTouch = false;
    }
    #region Online
    private void WaitPlayerMove(string gameCellName)
    {
        if (disableTouch) return;
        disableTouch = true;
        SetParent(gameCellName, playerSideIsTic ? ticPlayer : tacPlayer);
        emptyCell--;
        onlineClient.SendMessage(gameCellName);
        //playerTurn = false;
        WaitOpponentMove();
    }
    private async void WaitOpponentMove()
    {
        Debug.Log("wait opponent");
        notificationText.text = "Очікуємо на хід опонента...";
        string cellName = await onlineClient.ReceiveMessageAsync();
        if (!cellName.Contains("cell"))
        {
            notificationText.text = "З’єднання втрачено";
            return;
        }
        SetParent(cellName, playerSideIsTic ? tacPlayer : ticPlayer);
        emptyCell--;
        CheckWin();
        if(emptyCell > 0)
        {
            disableTouch = false;
            notificationText.text = "Ваш хід!";
        }        
    }
    #endregion
    #region CheckWin
    public void CheckWin()
    {
        GameObject[] gameObject = GameObject.FindGameObjectsWithTag("gameCell");
        int[,] Wins = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
        for (int i = 0; i < 8; i++)
        {
            try
            {
                string tag = gameObject[Wins[i, 0]].transform.GetChild(0)?.transform.tag;
                if (tag == gameObject[Wins[i, 1]].transform.GetChild(0)?.transform.tag && tag == gameObject[Wins[i, 2]].transform.GetChild(0)?.transform.tag)
                {
                    StartCoroutine(Win(i, tag == ticPlayer.tag));
                    return;
                }
            }
            catch
            {

            }

        }
        if (emptyCell == 0)
        {
            Debug.Log("ETO NICHIYA");
            StartCoroutine(Win(-1));
        }
    }
    IEnumerator Win(int whatWin,bool winIsTic = false)
    {
        clickedOnACell -= WaitPlayerMove;
        clickedOnACell -= GameWithBot;
        clickedOnACell -= OfflineGame;
        disableTouch = true;
        if (whatWin != -1) PlayWinLine(whatWin);
        yield return new WaitForSeconds(2f);
        foreach (GameObject restartMenu in restartMenu)
        {
            restartMenu.SetActive(true);
        }
        if (whatWin == -1)
        {
            notificationText.text = "Нічия!";
        }
        else
        {
            string winner = winIsTic ? "хрестики" : "нулики";
            notificationText.text = $"Гра закінчена!\nПеремогли {winner}";
        }
    }
    #endregion
}
