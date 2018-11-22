using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSoulContainer : MonoBehaviour {

    public SoulContainer mod;
    
    public ViewSoul[] arViewSoul;



    //Update all display information for each soul icon for visible soul
    public void cbUpdateVisibleSoul(Object target, params object[] args) {

        List<Soul> lstVisibleSoul = mod.GetVisibleSoul();
       

        for (int i = 0; i < arViewSoul.Length; i++) {

            if (lstVisibleSoul.Count - 1 < i) {
                //Then there's not actually a Soul in this slot
                arViewSoul[i].UpdateSoul(null);

            } else {
                //Then pass along this visible soul to be displayed
                arViewSoul[i].UpdateSoul(lstVisibleSoul[i]);

            }
        }

    }

    public void SetNumSoulSlots(int _nNumSoulSlots) {
        //TODO:: Account for having a variable number of soul slots

    }


    void Start () {

        SetNumSoulSlots(3);

        mod.subVisibleSoulUpdate.Subscribe(cbUpdateVisibleSoul);

        cbUpdateVisibleSoul(null);//Initially update the soul to just be blanks

	}
	

}
