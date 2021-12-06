using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillThunderStorm : Skill {

    public SkillThunderStorm(Chr _chrOwner) : base(_chrOwner) {

        sName = "ThunderStorm";
        sDisplayName = "Thunder Storm";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 1, 1, 0, 0));

        nCooldownInduced = 10;
        nFatigue = 5;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, Target.TRUE);
    }

    class Clause1 : ClauseSkillSelection {

        Damage dmg;
        public int nBaseDamage = 15;
        public int nBaseStun = 2;

        public Clause1(Skill _skill) : base(_skill) {
            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage and {1} fatigue to all characters on the target character's team", dmg.Get(), nBaseStun);
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            List<Chr> lstChrsOnTeam = chrSelected.plyrOwner.GetActiveChrs();

            for(int i = 0; i < lstChrsOnTeam.Count; i++) {

                ContSkillEngine.PushSingleExecutable(new ExecStun(skill.chrOwner, lstChrsOnTeam[i], nBaseStun) {
                    sLabel = "Crackle Crackle"
                });

                ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, lstChrsOnTeam[i], dmg) {
                    sLabel = "Caught in the storm"
                });



            }

        }

    }

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.THUNDERSTORM;
    }

}