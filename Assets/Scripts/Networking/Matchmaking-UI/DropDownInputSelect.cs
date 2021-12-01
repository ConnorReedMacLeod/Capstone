using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownInputSelect : MonoBehaviour {

    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;


    public void Start() {
        //Start the match by reacting to 'selecting' whatever's defaultedly set here
        OnInputSelectChange();
    }

    public void OnInputSelectChange() {
        
        NetworkMatchSetup.SetInputType(plyrselectorParent.idPlayer, (Player.InputType)dropdown.value); 

    }
}
