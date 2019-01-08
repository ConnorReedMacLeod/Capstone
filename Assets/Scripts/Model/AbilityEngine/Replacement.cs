using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replacement {

    public System.Type type; //A substitute for templating - we can cast to this type if necessary
                             // alternatively, just make sure you're typesafe when when calling the predicate/replacement effect

    public bool bHasReplaced; //Raise this trigger to ensure that we don't encounter cycles of fullreplacements
    public static List<Replacement> lstAllReplacements = new List<Replacement>();

    public delegate bool ReplacePred(Executable exec);
    public delegate Executable ReplaceNewExec(Executable exec);

    public ReplacePred shouldReplace;
    public ReplaceNewExec execReplace;


    //Call when initializing
    public static void Register(Replacement rep) {
        lstAllReplacements.Add(rep);
    }

    //Call when the replacement effect is ending
    public static void Unregister(Replacement rep) {
        lstAllReplacements.Remove(rep);
    }

    public Executable ApplyReplacement(Executable toReplace) {
        bHasReplaced = true;

        return execReplace(toReplace);
    }

    //Call this at the beginning of each opportunity for replacement effects
    // we'll prepare all replacement effects to be enacted
    public static void ResetReplacedFlags() {
        foreach (Replacement rep in lstAllReplacements) {
            rep.bHasReplaced = false;
        }
    }
}
