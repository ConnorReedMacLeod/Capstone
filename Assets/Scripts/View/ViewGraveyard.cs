using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewGraveyard : MonoBehaviour {

    public float fDistBetweenChrs;

    public void cbUpdateGraveyardCharacters(Object target, params object[] args) {

        for(int i=0; i<ContDeaths.Get().lstDeadChrs.Count; i++) {

            Vector3 v3NewChrPosition = new Vector3(this.transform.position.x + i * fDistBetweenChrs, this.transform.position.y, this.transform.position.z);

            ContDeaths.Get().lstDeadChrs[i].view.transform.position = v3NewChrPosition;
            
        }

    }

    private void Start() {
        ContDeaths.Get().subDeadChrsChange.Subscribe(cbUpdateGraveyardCharacters);
    }
}
