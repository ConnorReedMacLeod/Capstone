using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBucklerParry : Action {

    public ActionBucklerParry(Chr _chrOwner) : base(_chrOwner, 0) {// 0 is the dominant clause

        sName = "BucklerParry";
        sDisplayName = "Buckler Parry";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 2;
        nActionCost = 0;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public SoulParry soulToCopy;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

            soulToCopy = new SoulParry(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Gain 15 DEFENSE and PARRY (4).\n" +
                "[PARRY]: When an enemy would deal damage to {0}, deal {1} damage to them and dispel.", action.chrSource.sName, soulToCopy.dmgCounterAttack.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulParry(soulToCopy, chrSelected)));

        }

    };

}