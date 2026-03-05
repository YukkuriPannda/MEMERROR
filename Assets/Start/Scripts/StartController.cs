using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class StartController : MonoBehaviour
{
    // Assign the Animator that controls ;
    public Animator play_the_game_animator;
    public Animator settings_animator;
    public Animator how_to_play_animator;

    // Name of the trigger parameter in the Animator. Create this trigger in the Animator Controller.

    // Optional: call this from a Button's OnClick if you prefer OnClick over pointer events.

    public void Start()
    {

    }

    public void ActivePlayButton()
    {
        play_the_game_animator.SetTrigger("active");
        play_the_game_animator.gameObject.get_child(0).GetComponent<TextMeshProUGUI>().text = "01.>play_the_game";
    }
    public void DeactivePlayButton()
    {
        play_the_game_animator.SetTrigger("deactive");
        play_the_game_animator.gameObject.get_child(0).GetComponent<TextMeshProUGUI>().text = "01._play_the_game";
    }
    public void ActiveSettingsButton()
    {
        settings_animator.SetTrigger("active");
        settings_animator.gameObject.get_child(0).GetComponent<TextMeshProUGUI>().text = "02.>settings";
    }
    public void DeactiveSettingsButton()
    {
        settings_animator.SetTrigger("deactive");
        settings_animator.gameObject.get_child(0).GetComponent<TextMeshProUGUI>().text = "02._settings";
    }
    public void ActiveHowToPlayButton()
    {
        how_to_play_animator.SetTrigger("active");
        how_to_play_animator.gameObject.transform.get_child(0).GetComponent<TextMeshProUGUI>().text = "03.>how_to_play";
    }
    public void DeactiveHowToPlayButton()
    {
        how_to_play_animator.SetTrigger("deactive");

        how_to_play_animator.gameObject.transform.get_child(0).GetComponent<TextMeshProUGUI>().text = "03._how_to_play";

    }
}