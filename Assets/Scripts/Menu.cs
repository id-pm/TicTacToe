using UnityEngine;
using static SettingClass;

public class Menu : MonoBehaviour
{
    int prevIndex = 0;
    [SerializeField]
    GameObject[] m_Menu;
    GameObject SoundON;
    GameObject SoundOFF;
    private void Start()
    {
        SoundON = GameObject.FindGameObjectWithTag("SoundON");
        SoundOFF = GameObject.FindGameObjectWithTag("SoundOFF");
        SoundOFF.gameObject.SetActive(false);
        SettingClass.switchSound += SwitchSound;
    }
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                BackMenu();
                return;
            }
        }
        
    }
    public void MainMenu()
    {
        ChangeMenu(1);
        prevIndex = 0;
    }
    public void ChangeMenu(int index)
    {
        DisableMenu();
        m_Menu[index].gameObject.SetActive(true);
    }
    public void BackMenu()
    {
        DisableMenu();
        Debug.Log(prevIndex);
        if (prevIndex == 4) prevIndex--;
        if (prevIndex == 0)
        {
            QuitGame();
            return;
        }
        m_Menu[--prevIndex].gameObject.SetActive(true);
    }
    private void DisableMenu()
    {
        for (int i = 0; i < m_Menu.Length; i++)
        {
            if (m_Menu[i].gameObject.activeSelf == true)
            {
                m_Menu[i].gameObject.SetActive(false);
                prevIndex = i;
                return;
            }
        }
    }
    //public void StartGame()
    //{
    //    //Debug.Log("START:" + modeSelection);
    //    //SceneManager.LoadScene(1);
    //    SceneManager.UnloadSceneAsync(0);
    //   // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}
    public void ChangeGameMode(int type)
    {
        if (type != -1)
        {
            modeSelection = (GameMode)type;
        }
    }
    public static void ChooseSide(bool newside)
    {
        side = newside;
    }
    public static void ChangeSide()
    {
        side = !side;
    }
    public void QuitGame()
    {
        Application.Quit();
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
}
