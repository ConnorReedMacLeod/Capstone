using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStickyBomb : Skill {

    public Damage dmg;
    public int nBaseDamage;

    public SkillStickyBomb(Chr _chrOwner) : base(_chrOwner) {

        sName = "StickyBomb";
        sDisplayName = "Sticky Bomb";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 1, 0, 0, 1));

        nCooldownInduced = 6;
        nFatigue = 3;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this),
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, TarChr.IsInPlay());
    }

    class Clause1 : ClauseSkillSelection {

        Damage dmg;
        public int nBaseDamage = 5;
        public SoulStickyBomb soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
            soulToCopy = new SoulStickyBomb(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the chosen character, and another {1} at the end of turn", dmg.Get(), soulToCopy.nDetonationDamage);
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulStickyBomb(soulToCopy, chrSelected)) {
                sLabel = "A bomb stuck"
            });

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndStickyBombToss", 2.133f) },
                sLabel = "Threw a bomb"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.STICKYBOMB;
    }

}