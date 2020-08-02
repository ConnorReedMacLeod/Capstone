using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputAbilitySelection : MonoBehaviour {

    public Player plyrOwner;

    //The stored information for what targetting information we'll give.  We can build this up and construct an appropriate
    //  final SelectionInfo once we're done choosing ability targets
    public SelectionSerializer.SelectionInfo infoSelection;

    public void ResetTargets() {
        infoSelection = null;
    }

    public abstract void StartSelection();

    public abstract void GaveInvalidTarget();

    public void SetOwner(Player _plyrOwner) {
        plyrOwner = _plyrOwner;
    }


}
