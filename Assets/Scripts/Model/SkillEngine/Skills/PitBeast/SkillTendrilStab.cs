using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTendrilStab : Skill {

    public SkillTendrilStab(Chr _chrOwner) : base(_chrOwner) {

        sName = "TendrilStab";
        sDisplayName = "Tendril Stab";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(1, 0, 0, 0, 0));

        nCooldownInduced = 6;
        nFatigue = 3;

        lstTargets = new List<Target>() {
            new TarChr(Target.AND(TarChr.IsDiffTeam(chrOwner), TarChr.IsFrontliner()))
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }


    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 25;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage, true);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} [PIERCING] damage to an enemy frontliner.", dmg.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("PitBeast/sndTendrilStab", 3.067f) },
                sLabel = "Stab, stab, stab"
            });

        }

    };

}
