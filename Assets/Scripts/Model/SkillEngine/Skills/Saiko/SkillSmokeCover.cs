using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSmokeCover : Skill {

    public SkillSmokeCover(Chr _chrOwner) : base(_chrOwner) {

        sName = "SmokeCover";
        sDisplayName = "Smoke Cover";

        typeUsage = new TypeUsageCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 1, 0, 0, 0));

        nCooldownInduced = 10;
        nFatigue = 2;

        lstTargets = new List<Target>() {
            new TarMana(this, manaCost),
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        public SoulSmokeCover soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            soulToCopy = new SoulSmokeCover(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain SHROUDED (4)\n" +
                "[SHROUDED]: This character is immune to damage.  If this character becomes the Vanguard, dispel this.");
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = skill.chrOwner;

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulSmokeCover(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndSmokeCover", 4.3f) },
                sLabel = "Disappearing into the shadows..."
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.SMOKECOVER;
    }

}
