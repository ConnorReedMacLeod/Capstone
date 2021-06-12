using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionExplosion : Action {

    public ActionExplosion(Chr _chrOwner) : base(_chrOwner, 0) {//number of target arguments

        sName = "Explosion";
        sDisplayName = "Explosion";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 2, 0, 0 });

        nCooldownInduced = 10;
        nFatigue = 6;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 5;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSweeping(this) //Base Tag always goes first
            });


            dmg = new Damage(action.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to all characters on the target character's team", dmg.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                sLabel = "Explodin'"
            });

        }

    };

}
