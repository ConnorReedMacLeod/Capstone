using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFireball : Skill {

    public SkillFireball(Chr _chrOwner) : base(_chrOwner) {

        sName = "Fireball";
        sDisplayName = "Fireball";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 1, 0, 0));

        nCooldownInduced = 6;
        nFatigue = 4;


        lstTargets = new List<Target>() {
            new TarChr(this, TarChr.IsDiffTeam(chrOwner))
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

            return string.Format("Deal {0} damage to an Enemy", dmg.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                sLabel = "Hurling a fireball"
            });

        }

    };

    class Clause2 : Clause {

        public SoulBurning soulToCopy;

        public Clause2(Skill _skill) : base(_skill) {

            soulToCopy = new SoulBurning(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Apply Burning(4) to that enemy");
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulBurning(soulToCopy, chrSelected)));

        }

    };

}
