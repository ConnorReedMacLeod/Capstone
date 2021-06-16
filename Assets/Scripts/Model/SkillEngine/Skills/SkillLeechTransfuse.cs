using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLeech : Skill {

    SkillTransfuse skillSwap;

    public SkillLeech(Chr _chrOwner) : base(_chrOwner, 0) {

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

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });


            dmg = new Damage(skill.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to an Enemy", dmg.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrSource, chrSelected, dmg) {
                sLabel = "Gimme yer life-juice"
            });

        }

    };

    class Clause2 : ClauseChr {

        public Clause2(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSelf(this), //Base Tag always goes first
            });

        }

        public override string GetDescription() {

            return string.Format("Transform this skill into [Transfuse]");
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecAdaptSkill(skill.chrSource, this.skill, SkillType.SKILLTYPE.TRANSFUSE));

        }

    };

}



public class SkillTransfuse : Skill {

    public SkillLeech skillSwap;

    public SkillTransfuse(Chr _chrOwner) : base(_chrOwner, 0) {

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

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrAlly(this)
            });


            healing = new Healing(skill.chrSource, null, nHealAmount);
        }

        public override string GetDescription() {

            return string.Format("Heal an Ally for {0}", healing.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecHeal(skill.chrSource, chrSelected, healing) {
                sLabel = "Drink my life-juice"
            });

        }

    };

    class Clause2 : ClauseChr {

        public Clause2(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSelf(this) // base tag always goes first
            });

        }

        public override string GetDescription() {

            return string.Format("Transform this skill into [Leech]");
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecAdaptSkill(skill.chrSource, this.skill, SkillType.SKILLTYPE.LEECH));

        }

    };

}

