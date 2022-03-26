using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManaCalendar : MonoBehaviour {

    public ViewManaDate[] arViewManaDate;
    public Text txtTitle;

    public ManaCalendar modManaCalendar;

    public bool bShown;

    public static readonly Vector3 v3ShowPosition = new Vector3(0.75f, -2.33f, -0.5f);
    public static readonly Vector3 v3HidePosition = new Vector3(100f, 100f, -0.5f);

    // Start is called before the first frame update
    public void Start() {
        
        txtTitle.text = string.Format("Mana Calendar for Player {0}", modManaCalendar.plyrOwner.id);

        //Initially hide the panel
        HidePanel();

        //Set the hotkey for opening/closing the mana calendar dependent on which player this calendar is for
        if(modManaCalendar.plyrOwner.id == 0) {
            KeyBindings.SetBinding(cbToggleShown, KeyCode.LeftBracket);
        } else {
            KeyBindings.SetBinding(cbToggleShown, KeyCode.RightBracket);
        }

    }

    public void cbToggleShown(Object tar, params object[] args) {
        if (bShown) {
            HidePanel();
        } else {
            ShowPanel();
        }

        bShown = !bShown;
    }

    public void HidePanel() {

        this.gameObject.transform.position = v3HidePosition;
    }

    public void ShowPanel() {

        this.gameObject.transform.position = v3ShowPosition;
    }


}
