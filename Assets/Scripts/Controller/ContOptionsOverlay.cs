using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContOptionsOverlay : Singleton<ContOptionsOverlay> {
    
    public Dropdown dropdownPlayer1Input;
    public Dropdown dropdownPlayer2Input;
    public Dropdown dropdownGameSpeed;
    public Toggle toggleFastForward;

    public Vector3 v3OnScreen = new Vector3(0f, 0f, 0f);
    public Vector3 v3OffScreen = new Vector3(-1000f, -1000f, 0f);


    public void cbClickRestart() {

        //Clear out any static subject lists
        Subject.ResetAllStaticSubjects();

        //Now transition back to this level (reset the scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }


    public void UpdatePlyr1Input(int nInputType) {

        Match.Get().arPlayers[0].SetInputType((LocalInputType.InputType)dropdownPlayer1Input.value);

    }

    public void UpdatePlyr2Input(int nInputType) {

        Match.Get().arPlayers[1].SetInputType((LocalInputType.InputType)dropdownPlayer2Input.value);

    }

    public void UpdateGameSpeed(int nGameSpeed) {

        ContTime.Get().SetMaxSelectionTime((ContTime.DELAYOPTIONS)dropdownGameSpeed.value);

    }

    public void OnToggleFastForward() {
        ContTime.Get().SetManualFastForward(toggleFastForward.isOn);
    }

    public void cbOpenOptionsOverlay(Object target, params object[] args) {

        //Move the overlay onto the screen
        this.transform.localPosition = v3OnScreen;

        ContTime.Get().Pause();

        //And listen for the open menu shortcut
        KeyBindings.SetBinding(cbOnLeave, KeyCode.Escape);
    }

    public void cbOnLeave(Object target, params object[] args) {

        //Move the overlay off of the screen
        this.transform.localPosition = v3OffScreen;

        ContTime.Get().UnPause();

        //And listen for the open menu shortcut
        KeyBindings.SetBinding(cbOpenOptionsOverlay, KeyCode.Escape);
    }


    public void InitDefaultOptions() {

        //Load in any preset or pre-configured option settings that we cover
        dropdownPlayer1Input.SetValueWithoutNotify((int)NetworkMatchSetup.GetInputType(0) - 1);
        dropdownPlayer2Input.SetValueWithoutNotify((int)NetworkMatchSetup.GetInputType(1) - 1);

        dropdownGameSpeed.value = (int)ContTime.DELAYOPTIONS.MEDIUM;
    }

    public override void Init() {
        //TODO:: Decide what things should persist between scene changes (default options/keybinds)
        InitDefaultOptions();

        //Initially hide the menu
        this.transform.localPosition = v3OffScreen;

        //And listen for the open menu shortcut
        KeyBindings.SetBinding(cbOpenOptionsOverlay, KeyCode.Escape);
    }

}
