using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
public class StartController : MonoBehaviour
{
    // Assign the Animator that controls ;
    public Animator play_the_game_animator;
    public Animator settings_animator;
    public Animator how_to_play_animator;

    public void ChangeToGameScene(){
        SceneManager.LoadScene("Game");
    }
    public void ActivePlayButton()
    {
        play_the_game_animator.SetTrigger("active");
        play_the_game_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "01.>play_the_game";
    }
    public void DeactivePlayButton()
    {
        play_the_game_animator.SetTrigger("deactive");
        play_the_game_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "01._play_the_game";
    }

    public void ChangeToSettingsScene(){
        SceneManager.LoadScene("Settings");
    }
    public void ActiveSettingsButton()
    {
        settings_animator.SetTrigger("active");
        settings_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "02.>settings";
    }
    public void DeactiveSettingsButton()
    {
        settings_animator.SetTrigger("deactive");
        settings_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "02._settings";
    }
    public void ActiveHowToPlayButton()
    {
        how_to_play_animator.SetTrigger("active");
        how_to_play_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "03.>how_to_play";
    }
    public void DeactiveHowToPlayButton()
    {
        how_to_play_animator.SetTrigger("deactive");

        how_to_play_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "03._how_to_play";

    }
}