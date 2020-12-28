using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour {

    public int idPlayer;


    public void SubmitSelection() {
        CharacterSelection.Get().SubmitSelection(idPlayer);
    }

}
