using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSerenade : Skill {

    public SkillSerenade(Chr _chrOwner) : base(_chrOwner) {

        sName = "Serenade";
        sDisplayName = "Serenade";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 1, 0, 0));

        nCooldownInduced = 8;
        nFatigue = 4;

        lstTargets = new List<Target>() {
            new TarMana(this, manaCost),
            new TarChr(this, TarChr.IsSameTeam(chrOwner))
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };

    }

    class Clause1 : Clause {

        Healing heal;
        int nBaseHealing = 25;

        public Clause1(Skill _skill) : base(_skill) {

            //Create and store a copy of the intended healing effect so that any information/effects
            // can be updated accurately
            heal = new Healing(skill.chrOwner, null, nBaseHealing);

        }

        public override string GetDescription() {

            //TODO - eventually figure out how I'm gonna dynamically generate the text targets
            return string.Format("Heal {0} life to the chosen Ally", heal.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            //Push an executable with this skill's owner as the source, the selected character as the target,
            // and we can copy the stored healing instance to apply
            ContSkillEngine.PushSingleExecutable(new ExecHeal(skill.chrOwner, chrSelected, heal) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Katarina/sndSerenade", 5.3f) },
                sLabel = "<Darude's Sandstorm on Recorder>"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.SERENADE;
    }

}
