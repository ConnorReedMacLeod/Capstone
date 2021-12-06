using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFortissimo : Skill {

    public SkillFortissimo(Chr _chrOwner) : base(_chrOwner) {

        sName = "Fortissimo";
        sDisplayName = "Fortissimo";

        typeUsage = new TypeUsageCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 0));

        nCooldownInduced = 8;
        nFatigue = 0;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this),
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
    }

    class Clause1 : ClauseSkillSelection {

        public SoulFortissimo soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            soulToCopy = new SoulFortissimo(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain {0} POWER and {1} DEFENSE for {2} turns.", soulToCopy.nPowerBuff, soulToCopy.nDefenseBuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = skill.chrOwner;

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulFortissimo(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Katarina/sndFortissimo", 6.2f) },
                sLabel = "Let's do it louder this time"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.FORTISSIMO;
    }
}
