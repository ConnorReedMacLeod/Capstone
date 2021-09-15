using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillExplosion : Skill {

    public SkillExplosion(Chr _chrOwner) : base(_chrOwner) {

        sName = "Explosion";
        sDisplayName = "Explosion";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 2, 0, 0));

        nCooldownInduced = 10;
        nFatigue = 6;

        lstTargets = new List<Target>() {
            new TarChr(Target.TRUE)
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 5;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to all characters on the target character's team", dmg.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            List<Chr> lstChrsOnTeam = chrSelected.plyrOwner.GetActiveChrs();

            for(int i = 0; i < lstChrsOnTeam.Count; i++) {
                ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, lstChrsOnTeam[i], dmg) {
                    sLabel = "Explodin'"
                });
            }

        }

    };

}
