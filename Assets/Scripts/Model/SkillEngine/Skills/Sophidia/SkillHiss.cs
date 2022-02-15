using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHiss : Skill {


    public SkillHiss(Chr _chrOwner) : base(_chrOwner) {

        sName = "Hiss";
        sDisplayName = "Hiss";

        typeUsage = new TypeUsageCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 1));

        nCooldownInduced = 10;
        nFatigue = 1;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
    }

    class Clause1 : ClauseSkillSelection {

        public SoulSpooked soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            soulToCopy = new SoulSpooked(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("All enemies lose {0} POWER for {1} turns.", soulToCopy.nPowerDebuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(InputSkillSelection selections) {


            List<Chr> lstEnemyChrs = skill.chrOwner.plyrOwner.GetEnemyPlayer().GetActiveChrs();

            for(int i = 0; i < lstEnemyChrs.Count; i++) {
                ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, lstEnemyChrs[i], new SoulSpooked(soulToCopy, lstEnemyChrs[i])) {
                    arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndHiss1", 2f),
                                                     new SoundEffect("Sophidia/sndHiss2", 2f),
                                                     new SoundEffect("Sophidia/sndHiss3", 2f)},
                    sLabel = "Ah, so spook!"
                });
            }
        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.HISS;
    }

}