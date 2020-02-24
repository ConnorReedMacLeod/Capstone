using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTendrilStab : Action {

    public ActionTendrilStab(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "TendrilStab";
        sDisplayName = "Tendril Stab";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCd = 6;
        nFatigue = 3;
        nActionCost = 1;

        nBaseDamage = 25;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage, true);

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }


    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 25;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrMelee(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });

            dmg = new Damage(action.chrSource, null, nBaseDamage, true);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} [PIERCING] damage to the enemy Vanguard.", dmg.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("PitBeast/sndTendrilStab", 3.067f) },
                sLabel = "Stab, stab, stab"
            });

        }

    };

}
