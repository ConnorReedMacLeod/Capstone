using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionThunderStorm : Action {

    public ActionThunderStorm(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "ThunderStorm";
        sDisplayName = "Thunder Storm";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 1, 0, 0 });

        nCd = 10;
        nFatigue = 5;
        nActionCost = 1;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 15;
        public int nBaseStun = 2;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSweeping(this) //Base Tag always goes first
            });


            dmg = new Damage(action.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage and {1} fatigue to all characters on the target character's team", dmg.Get(), nBaseStun);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecStun(action.chrSource, chrSelected, nBaseStun){
                 sLabel = "Crackle Crackle"
            });

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                sLabel = "Caught in the storm"
            });



        }

    };

}