using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownInputSelect : MonoBehaviour {

    public PlayerSelector plyrselectorParent;

    public void OnInputSelectChange(int nInputSelect) {

        CharacterSelection.Get().arInputTypes[plyrselectorParent.idPlayer] = (Player.InputType)nInputSelect;

    }
}
