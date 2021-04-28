using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSadism : Action {

    public SoulSadism soulPassive;

    public ActionSadism(Chr _chrOwner) : base(_chrOwner, 0) {// set the dominant clause to 0

        sName = "Sadism";
        sDisplayName = "Sadism";

        type = new TypePassive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 0;
        nFatigue = 0;


        soulPassive = new SoulSadism(this.chrSource, this.chrSource, this);

        lstClausesOnEquip = new List<Clause>() {
            new ClauseEquip(this)
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };

        lstClausesOnUnequip = new List<Clause>() {
            new ClauseUnequip(this)
        };
    }

    class ClauseEquip : ClauseSpecial {

        public ClauseEquip(Action _act) : base(_act) {
            // Eventually add superficial tags here
        }

        public override string GetDescription() {

            return string.Format("Initially applying Sadism on equip");
        }

        public override void ClauseEffect() {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, action.chrSource, ((ActionSadism)action).soulPassive) {
                sLabel = "applying sadism"
            });

        }

    };

    class ClauseUnequip : ClauseSpecial {

        public ClauseUnequip(Action _act) : base(_act) {
            // Eventually add superficial tags here
        }

        public override string GetDescription() {

            return string.Format("Removing Sadism on unequip");
        }

        public override void ClauseEffect() {

            ContAbilityEngine.PushSingleExecutable(new ExecRemoveSoul(action.chrSource, ((ActionSadism)action).soulPassive) {
                sLabel = "removing sadism"
            });

        }

    };

    class Clause1 : ClauseSpecial {

        public Clause1(Action _act) : base(_act) {
            // Eventually add superficial tags here
        }

        public override string GetDescription() {

            return string.Format("When {0} would deal damage to a character with greater health, heal {1}.", action.chrSource.sName, ((ActionSadism)action).soulPassive.heal.Get());
        }

        public override void ClauseEffect() {

            Debug.LogError("Shouldn't be executing a passive");

        }

    };
}
