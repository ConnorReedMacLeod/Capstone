using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targetter <T> {

    public BaseTargetter<T> baseTargetter;
    public List<TargetterTag<T>> lstTargetterTags;
     
    //TODONOW - for TargetterSpecial, include a delegate function that can 
    //          be filled in dynamically for custom targetting rules, and 
    //          the GetTargets method can be overriden to just call this 
    //          stored delegate


    //Gets the final targets based on the tag filters on the Targetter
    public virtual List<T> GetTargets() {

        List<T> lstTargets = baseTargetter.GetBaseTargets();

        for(int i=0; i<lstTargetterTags.Count; i++) {
            lstTargets = lstTargetterTags[i].ApplyTagFiltering(lstTargets);
        }

        //TODONOW:: Consider what will need to be done specially for the Ally+Enemy combination
        //          Since those wouldn't filter stuff out, but actually increase the targetting pool

        return lstTargets;
    }


}
