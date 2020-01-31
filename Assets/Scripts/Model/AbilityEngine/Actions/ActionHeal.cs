using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHeal : Action {

    public Healing heal;
    public int nBaseHealing;

    public ActionHeal(Chr _chrOwner) : base(_chrOwner) {

        sName = "Heal";
        sDisplayName = "Heal";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 3;
        nFatigue = 3;
        nActionCost = 0;

        nBaseHealing = 5;

        lstClauses = new List<Clause> {
            new Clause1(this)
        };

    }

    class Clause1 : ClauseChr {

        Healing heal;

        public Clause1(Action _act) : base(_act) {
            //TODONOW:: fill in
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() { });

            heal = new Healing(action.chrSource, null, ((ActionHeal)action).nBaseHealing);
        }

        public override string GetDescription() {

            //TODO - eventually figure out how I'm gonna dynamically generate the text targets
            return string.Format("Heal {0} to an Ally", heal.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            //Push an executable with this action's owner as the source, the selected character as the target,
            // and we can copy the stored healing instance to apply
            ContAbilityEngine.PushSingleExecutable(new ExecHeal(action.chrSource, chrSelected, heal));

        }

    };
}
