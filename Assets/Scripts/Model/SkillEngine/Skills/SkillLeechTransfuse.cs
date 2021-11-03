using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLeech : Skill {

    SkillTransfuse skillSwap;

    public SkillLeech(Chr _chrOwner) : base(_chrOwner) {

        sName = "Leech";
        sDisplayName = "Leech";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 1, 0));

        nCooldownInduced = 1;
        nFatigue = 1;

        InitTargets();

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause2(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, TarChr.IsDiffTeam(chrOwner));
    }

    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 10;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to an Enemy", dmg.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                sLabel = "Gimme yer life-juice"
            });

        }

    };

    class Clause2 : Clause {

        public Clause2(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Transform this skill into [Transfuse]");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecAdaptSkill(skill.chrOwner, this.skill.skillslot, SkillType.SKILLTYPE.TRANSFUSE));

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.LEECH;
    }

}



public class SkillTransfuse : Skill {

    public SkillLeech skillSwap;

    public SkillTransfuse(Chr _chrOwner) : base(_chrOwner) {

        sName = "Transfuse";
        sDisplayName = "Transfuse";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 1, 0));

        nCooldownInduced = 2;
        nFatigue = 2;

        InitTargets();

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause2(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, TarChr.IsSameTeam(chrOwner));
    }

    class Clause1 : Clause {

        Healing healing;
        public int nHealAmount = 20;

        public Clause1(Skill _skill) : base(_skill) {

            healing = new Healing(skill.chrOwner, null, nHealAmount);
        }

        public override string GetDescription() {

            return string.Format("Heal an Ally for {0}", healing.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecHeal(skill.chrOwner, chrSelected, healing) {
                sLabel = "Drink my life-juice"
            });

        }

    };

    class Clause2 : Clause {

        public Clause2(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Transform this skill into [Leech]");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecAdaptSkill(skill.chrOwner, this.skill.skillslot, SkillType.SKILLTYPE.LEECH));

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.TRANSFUSE;
    }

}

