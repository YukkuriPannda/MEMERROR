using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System.Drawing;
using UnityEngine.UI;
public class StartController : MonoBehaviour
{
    // Assign the Animator that controls ;
    public Animator play_the_game_animator;
    public Animator settings_animator;
    public Animator how_to_play_animator;
    public GameObject settingCanvas;
    public Animator globalVolumeAnimator;
    public Color32 active_color = new Color32(255, 255, 255, 255);
    public Color32 deactive_color = new Color32(0, 0, 0, 255);

    public void Start()
    {
        SaveDataManager.Load();
    }
    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("Game");
    }
    public void ActivePlayButton()
    {
        play_the_game_animator.SetTrigger("active");
        play_the_game_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "01.>play_the_game";
        play_the_game_animator.gameObject.transform.GetChild(0).transform.localScale = new Vector3(0.9f, 0.9f, 1);
    }
    public void DeactivePlayButton()
    {
        play_the_game_animator.SetTrigger("deactive");
        play_the_game_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "01._play_the_game";
        play_the_game_animator.gameObject.transform.GetChild(0).transform.localScale = new Vector3(1f, 1f, 1);
    }

    public void OpenSettings()
    {
        if (settingCanvas != null)
        {
            settingCanvas.SetActive(true);
            globalVolumeAnimator.SetTrigger("open");
        }
    }
    public void CloseSettings()
    {
        if (settingCanvas != null)
        {

            settingCanvas.SetActive(false);
            globalVolumeAnimator.SetTrigger("close");
        }
    }
    public void ActiveSettingsButton()
    {
        settings_animator.SetTrigger("active");
        settings_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "02.>settings";
        settings_animator.gameObject.transform.GetChild(0).transform.localScale = new Vector3(0.9f, 0.9f, 1);
    }
    public void DeactiveSettingsButton()
    {
        settings_animator.SetTrigger("deactive");
        settings_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "02._settings";
        settings_animator.gameObject.transform.GetChild(0).transform.localScale = new Vector3(1f, 1f, 1);
    }
    public void ActiveHowToPlayButton()
    {
        how_to_play_animator.SetTrigger("active");
        how_to_play_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "03.>how_to_play";
        how_to_play_animator.gameObject.transform.GetChild(0).transform.localScale = new Vector3(0.9f, 0.9f, 1);
    }
    public void DeactiveHowToPlayButton()
    {
        how_to_play_animator.SetTrigger("deactive");

        how_to_play_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "03._how_to_play";
        how_to_play_animator.gameObject.transform.GetChild(0).transform.localScale = new Vector3(1f, 1f, 1);
    }

}