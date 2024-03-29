﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBunker : Skill {

    public SkillBunker(Chr _chrOwner) : base(_chrOwner) {

        sName = "Bunker";
        sDisplayName = "Bunker";

        typeUsage = new TypeUsageCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 1, 0, 0));

        nCooldownInduced = 4;
        nFatigue = 6;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
    }

    class Clause1 : ClauseSkillSelection {

        public SoulPositionBunker soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            soulToCopy = new SoulPositionBunker(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("The Position under this character gives +{0} DEFENSE to the character on it for {1} turns.", soulToCopy.nDefenseBuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulPosition(skill.chrOwner, skill.chrOwner.position, new SoulPositionBunker(soulToCopy, skill.chrOwner.position)) {

                sLabel = "Hunker down"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.BUNKER;
    }

}