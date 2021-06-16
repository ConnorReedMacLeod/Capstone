using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContOptionsOverlay : Singleton<ContOptionsOverlay> {

    public ViewOptionsButton btnPlyr0Human;
    public ViewOptionsButton btnPlyr0AI;
    public ViewOptionsButton btnPlyr1Human;
    public ViewOptionsButton btnPlyr1AI;
    public ViewOptionsButton btnTimerFast;
    public ViewOptionsButton btnTimerMedium;
    public ViewOptionsButton btnTimerInf;

    public ViewOptionsButton btnRestart;

    public Subject subPlayer0SelectedInGroup = new Subject();
    public Subject subPlayer1SelectedInGroup = new Subject();
    public Subject subTimerSelectedInGroup = new Subject();

    public Vector3 v3OnScreen = new Vector3(0f, 0f, -1f);
    public Vector3 v3OffScreen = new Vector3(-100f, -100f, -1f);


    public void cbClickRestart(Object target, params object[] args) {

        //Clear out any static subject lists
        Subject.ResetAllStaticSubjects();

        //Now transition back to this level (reset the scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }


    public void cbClickPlyr0Human(Object target, params object[] args) {

        Match.Get().arPlayers[0].SetInputType(Player.InputType.HUMAN);

        subPlayer0SelectedInGroup.NotifyObs(target);
    }

    public void cbClickPlyr0AI(Object target, params object[] args) {

        Match.Get().arPlayers[0].SetInputType(Player.InputType.AI);

        subPlayer0SelectedInGroup.NotifyObs(target);
    }


    public void cbClickPlyr1Human(Object target, params object[] args) {

        Match.Get().arPlayers[1].SetInputType(Player.InputType.HUMAN);

        subPlayer1SelectedInGroup.NotifyObs(target);
    }

    public void cbClickPlyr1AI(Object target, params object[] args) {

        Match.Get().arPlayers[1].SetInputType(Player.InputType.AI);

        subPlayer1SelectedInGroup.NotifyObs(target);
    }

    public void cbClickTimerFast(Object target, params object[] args) {

        ContSkillSelection.Get().SetMaxSelectionTime(ContSkillSelection.DELAYOPTIONS.FAST);

        subTimerSelectedInGroup.NotifyObs(target);
    }

    public void cbClickTimerMedium(Object target, params object[] args) {

        ContSkillSelection.Get().SetMaxSelectionTime(ContSkillSelection.DELAYOPTIONS.MEDIUM);

        subTimerSelectedInGroup.NotifyObs(target);
    }

    public void cbClickTimerInf(Object target, params object[] args) {

        ContSkillSelection.Get().SetMaxSelectionTime(ContSkillSelection.DELAYOPTIONS.INF);

        subTimerSelectedInGroup.NotifyObs(target);
    }


    public void cbOnEnter(Object target, params object[] args) {

        //Move the overlay onto the screen
        this.transform.position = v3OnScreen;

        ContTime.Get().Pause();

        //Initialize all of the button's action subscription and action groups
        btnPlyr0Human.subClick.Subscribe(cbClickPlyr0Human);
        btnPlyr0AI.subClick.Subscribe(cbClickPlyr0AI);
        btnPlyr1Human.subClick.Subscribe(cbClickPlyr1Human);
        btnPlyr1AI.subClick.Subscribe(cbClickPlyr1AI);
        btnTimerFast.subClick.Subscribe(cbClickTimerFast);
        btnTimerMedium.subClick.Subscribe(cbClickTimerMedium);
        btnTimerInf.subClick.Subscribe(cbClickTimerInf);
        btnRestart.subClick.Subscribe(cbClickRestart);

        //And listen for the open menu shortcut
        KeyBindings.SetBinding(cbOnLeave, KeyCode.Escape);
    }

    public void cbOnLeave(Object target, params object[] args) {

        //Move the overlay off of the screen
        this.transform.position = v3OffScreen;

        ContTime.Get().UnPause();

        //Unsubscribe each button action
        btnPlyr0Human.subClick.UnSubscribe(cbClickPlyr0Human);
        btnPlyr0AI.subClick.UnSubscribe(cbClickPlyr0AI);
        btnPlyr1Human.subClick.UnSubscribe(cbClickPlyr1Human);
        btnPlyr1AI.subClick.UnSubscribe(cbClickPlyr1AI);
        btnTimerFast.subClick.UnSubscribe(cbClickTimerFast);
        btnTimerMedium.subClick.UnSubscribe(cbClickTimerMedium);
        btnTimerInf.subClick.UnSubscribe(cbClickTimerInf);
        btnRestart.subClick.UnSubscribe(cbClickRestart);

        //And listen for the open menu shortcut
        KeyBindings.SetBinding(cbOnEnter, KeyCode.Escape);
    }


    public void InitButtonGroups() {
        //Set up the radio-button selection groups
        subPlayer0SelectedInGroup.Subscribe(btnPlyr0Human.cbSelectedOptionInGroup);
        subPlayer0SelectedInGroup.Subscribe(btnPlyr0AI.cbSelectedOptionInGroup);

        subPlayer1SelectedInGroup.Subscribe(btnPlyr1Human.cbSelectedOptionInGroup);
        subPlayer1SelectedInGroup.Subscribe(btnPlyr1AI.cbSelectedOptionInGroup);

        subTimerSelectedInGroup.Subscribe(btnTimerFast.cbSelectedOptionInGroup);
        subTimerSelectedInGroup.Subscribe(btnTimerMedium.cbSelectedOptionInGroup);
        subTimerSelectedInGroup.Subscribe(btnTimerInf.cbSelectedOptionInGroup);

    }

    public void InitDefaultOptions() {

        //Initially Set the default options
        btnPlyr0Human.bSelected = true;
        subPlayer0SelectedInGroup.NotifyObs(btnPlyr0Human);

        btnPlyr1AI.bSelected = true;
        subPlayer1SelectedInGroup.NotifyObs(btnPlyr1AI);

        ContSkillSelection.Get().SetMaxSelectionTime(ContSkillSelection.DELAYOPTIONS.MEDIUM);
        btnTimerMedium.bSelected = true;
        subTimerSelectedInGroup.NotifyObs(btnTimerMedium);
    }

    public override void Init() {
        //TODO:: Decide what things should persist between scene changes (default options/keybinds)
        InitButtonGroups();
        InitDefaultOptions();

        //Initially hide the menu
        this.transform.position = v3OffScreen;

        //And listen for the open menu shortcut
        KeyBindings.SetBinding(cbOnEnter, KeyCode.Escape);
    }

}
