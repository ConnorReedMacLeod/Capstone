using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMultikill : Skill {

    public SkillMultikill(Chr _chrOwner) : base(_chrOwner) {

        sName = "Multikill";
        sDisplayName = "Multikill";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 1, 0));

        nCooldownInduced = 1;
        nFatigue = 1;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this),
            new Clause2(this),
            new Clause3(this),
            new Clause4(this),
            new Clause5(this),
            new Clause6(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, Target.TRUE);
        TarChr.AddTarget(this, Target.TRUE);
        TarChr.AddTarget(this, Target.TRUE);
        TarChr.AddTarget(this, Target.TRUE);
        TarChr.AddTarget(this, Target.TRUE);
        TarChr.AddTarget(this, Target.TRUE);//Target.AND(TarChr.IsInPlay(), TarChr.IsDiffTeam(chrOwner)));
    }

    class Clause1 : ClauseSkillSelection {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Set an enemy's health to 0");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecSetHealth(skill.chrOwner, chrSelected, 0) {
                sLabel = "Setting to 0 health"
            });

        }

    };

    class Clause2 : ClauseSkillSelection {

        public Clause2(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Set a second enemy's health to 0");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[2];

            ContSkillEngine.PushSingleExecutable(new ExecSetHealth(skill.chrOwner, chrSelected, 0) {
                sLabel = "Setting to 0 health"
            });

        }

    };

    class Clause3 : ClauseSkillSelection {

        public Clause3(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Set a third enemy's health to 0");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[3];

            ContSkillEngine.PushSingleExecutable(new ExecSetHealth(skill.chrOwner, chrSelected, 0) {
                sLabel = "Setting to 0 health"
            });

        }

    };

    class Clause4 : ClauseSkillSelection {

        public Clause4(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Set a fourth enemy's health to 0");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[4];

            ContSkillEngine.PushSingleExecutable(new ExecSetHealth(skill.chrOwner, chrSelected, 0) {
                sLabel = "Setting to 0 health"
            });

        }

    };

    class Clause5 : ClauseSkillSelection {

        public Clause5(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Set a fifth enemy's health to 0");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[5];

            ContSkillEngine.PushSingleExecutable(new ExecSetHealth(skill.chrOwner, chrSelected, 0) {
                sLabel = "Setting to 0 health"
            });

        }

    };

    class Clause6 : ClauseSkillSelection {

        public Clause6(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Set a sixth enemy's health to 0");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[6];

            ContSkillEngine.PushSingleExecutable(new ExecSetHealth(skill.chrOwner, chrSelected, 0) {
                sLabel = "Setting to 0 health"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.KILL;
    }

}
