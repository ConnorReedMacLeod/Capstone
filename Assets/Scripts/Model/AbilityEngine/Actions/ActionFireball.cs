using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFireball : Action {

    public ActionFireball(Chr _chrOwner): base(_chrOwner, 0){

		sName = "Fireball";
        sDisplayName = "Fireball";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[]{0,0,1,0,0});

		nCd = 6;
        nFatigue = 4;



        lstClauses = new List<Clause> {
            new Clause1(this),
            new Clause2(this)
        };
	}

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 5;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });
            

            dmg = new Damage(action.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {
            
            return string.Format("Deal {0} damage to an Enemy", dmg.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                sLabel = "Hurling a fireball"
            });

        }

    };

    class Clause2 : ClauseChr {

        public SoulBurning soulToCopy;

        public Clause2(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });

            soulToCopy = new SoulBurning(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Apply Burning(4) to that enemy");
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulBurning(soulToCopy, chrSelected)));

        }

    };

}
