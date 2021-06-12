using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionLeech : Action {

    ActionTransfuse actSwap;

    public ActionLeech(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Leech";
        sDisplayName = "Leech";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCooldownInduced = 1;
        nFatigue = 1;

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause2(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 10;

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
                sLabel = "Gimme yer life-juice"
            });

        }

    };

    class Clause2 : ClauseChr {

        public Clause2(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSelf(this), //Base Tag always goes first
            });

        }

        public override string GetDescription() {

            return string.Format("Transform this skill into [Transfuse]");
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecAdaptSkill(action.chrSource, this.action, SkillType.SKILLTYPE.TRANSFUSE));

        }

    };

}



public class ActionTransfuse : Action {

    public ActionLeech actSwap;

    public ActionTransfuse(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Transfuse";
        sDisplayName = "Transfuse";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCooldownInduced = 2;
        nFatigue = 2;

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause2(this)
        };
    }

    class Clause1 : ClauseChr {

        Healing healing;
        public int nHealAmount = 20;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrAlly(this)
            });


            healing = new Healing(action.chrSource, null, nHealAmount);
        }

        public override string GetDescription() {

            return string.Format("Heal an Ally for {0}", healing.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecHeal(action.chrSource, chrSelected, healing) {
                sLabel = "Drink my life-juice"
            });

        }

    };

    class Clause2 : ClauseChr {

        public Clause2(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSelf(this) // base tag always goes first
            });

        }

        public override string GetDescription() {

            return string.Format("Transform this skill into [Leech]");
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecAdaptSkill(action.chrSource, this.action, SkillType.SKILLTYPE.LEECH));

        }

    };

}

