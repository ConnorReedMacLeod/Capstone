using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillVenomousBite : Skill {

    public SkillVenomousBite(Chr _chrOwner) : base(_chrOwner) {

        sName = "VenomousBite";
        sDisplayName = "Venomous Bite";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 1, 0));

        nCooldownInduced = 8;
        nFatigue = 3;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this),
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, Target.AND(TarChr.IsDiffTeam(chrOwner), TarChr.IsFrontliner()));
    }

    class Clause1 : ClauseSkillSelection {

        Damage dmg;
        public int nBaseDamage = 5;
        public SoulEnvenomed soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
            soulToCopy = new SoulEnvenomed(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage and apply ENVENOMED ({1}) to the enemy Vanguard.\n" +
                "[ENVENOMED]: At the end of turn, lose {2} health.  Whenever this character takes damage, +1 duration.", dmg.Get(), soulToCopy.pnMaxDuration.Get(),
                soulToCopy.nLifeLoss);
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulEnvenomed(soulToCopy, chrSelected)) {
                sLabel = "Applying poison"
            });

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndVenomousBite1", 2f),
                                                     new SoundEffect("Sophidia/sndVenomousBite2", 2f)},
                sLabel = "Nomnomnom"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.VENEMOUSBITE;
    }

}
