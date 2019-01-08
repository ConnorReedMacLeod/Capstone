using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Replacement {

    public bool bHasReplaced; //Raise this trigger to ensure that we don't encounter cycles of fullreplacements
    public static List<Replacement> lstAllReplacements = new List<Replacement>();

    public List<Replacement> lstExecReplacements; //a reference to our executable's list of Replacements (either full or modifiers)

    public delegate bool ReplacePred(Executable exec);
    public delegate Executable ReplaceNewExec(Executable exec);

    public ReplacePred shouldReplace;
    public ReplaceNewExec execReplace;


    public Replacement() {
        bHasReplaced = false;
    }

    //Call when initializing
    public static void Register(Replacement rep) {
        //Add ourselves to the list of all replacement effects, so we get our flag reset properly
        lstAllReplacements.Add(rep);

        //Then add ourselves to our Executable's list of replacement effects
        rep.lstExecReplacements.Add(rep);
    }

    //Call when the replacement effect is ending
    public static void Unregister(Replacement rep) {
        lstAllReplacements.Remove(rep);

        rep.lstExecReplacements.Remove(rep);
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
