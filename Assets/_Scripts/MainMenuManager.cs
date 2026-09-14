using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip goodbye;
    private AudioChannelSettings channelSettings;

    public AudioSource bgm;

    // Start is called before the first frame update
    void Start()
    {
        this.channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 0.5f, "SFX");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButtonClicked()
    {
        SceneLoader.instance.LoadScene("JaredScene");        
    }

    public void ExitButtonClicked()
    {
        StartCoroutine(this.SayGoodbyeAndQuit());
    }

    private IEnumerator SayGoodbyeAndQuit()
    {
        this.bgm.Stop();    
        AudioManager.instance.Play(this.goodbye, this.channelSettings);

        yield return new WaitForSeconds(2.0f);

        SceneLoader.instance.QuitGame();
    }
}
