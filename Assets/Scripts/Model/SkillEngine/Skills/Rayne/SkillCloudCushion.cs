﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCloudCushion : Skill {

    public SkillCloudCushion(Chr _chrOwner) : base(_chrOwner) {

        sName = "CloudCushion";
        sDisplayName = "Cloud Cushion";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 1, 0, 0));

        nCooldownInduced = 7;
        nFatigue = 1;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };


    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, Target.AND(TarChr.IsInPlay(), TarChr.IsSameTeam(chrOwner)));
    }

    class Clause1 : ClauseSkillSelection {

        public SoulCloudCushion soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            soulToCopy = new SoulCloudCushion(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Target ally gains {0} DEFENSE for {1} turns.", soulToCopy.nDefenseBuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulCloudCushion(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Rayne/sndCloudCushion", 3.467f) },
                sLabel = "Ooh, so soft"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.CLOUDCUSHION;
    }

}