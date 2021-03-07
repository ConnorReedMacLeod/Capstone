using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRest : Action {

    public ActionRest(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Rest";
        sDisplayName = "Rest";

        type = new TypeCantrip(this);

        chrSource = _chrOwner;

        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 0;
        nFatigue = 0;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseSpecial {

        public int nRestFatigue;

        public Clause1(Action _act) : base(_act) {
            //TODO add in tags for base action, and rest

            nRestFatigue = 3;
        }

        public override string GetDescription() {

            return string.Format("Finish this character's turn");
        }

        public override void ClauseEffect() {

            //Check if the character has any fatigue already
            if(action.chrSource.nFatigue == 0) {
                //If not, then give them the rest fatigue
                ContAbilityEngine.Get().AddExec(new ExecChangeFatigue(action.chrSource, action.chrSource, nRestFatigue, false) {
                    sLabel = "Resting"
                });
            }

            action.chrSource.SetStateReadiness(new StateFatigued(action.chrSource));

        }

    };

}
