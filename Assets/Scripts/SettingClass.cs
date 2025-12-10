using UnityEngine;
using UnityEngine.UI;
public class SettingClass
{
    public static string host = "192.168.56.1";
    public static int port = 3210;
    public static GameMode modeSelection = GameMode.Offline;
    public static Text abobych;
    public static bool Music = true;
    public delegate void SwitchSound(bool switcher);
    public static event SwitchSound switchSound;
    public static void SwitchChanged(bool value)
    {
        switchSound.Invoke(value);
        Music = !Music;
    }
    public enum GameMode
    {
        Offline,
        WhithBot,
        Online,
    }
    public static bool side = true;
}
