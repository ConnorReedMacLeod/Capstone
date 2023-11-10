using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFireball : Skill {

    public SkillFireball(Chr _chrOwner) : base(_chrOwner) {

        sName = "Fireball";
        sDisplayName = "Fireball";

        typeUsage = new TypeUsageCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 1, 0, 0), true);

        nCooldownInduced = 6;
        nFatigue = 4;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, Target.AND(TarChr.IsInPlay(), TarChr.IsDiffTeam(chrOwner)));
    }

    class Clause1 : ClauseSkillSelection {

        Damage dmg;
        public int nBaseDamage = 10;

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Deal {0}*X damage to an Enemy", nBaseDamage);
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            //Ask our manacost target how much excess mana was spent on it
            int nX = ((TarMana)skill.lstTargets[0]).manaCostRequired.GetXPaid((Mana)selections.lstSelections[0]);
            Chr chrSelected = (Chr)selections.lstSelections[1];

            Debug.Log("nX was " + nX);

            dmg = new Damage(skill.chrOwner, null, nBaseDamage * nX);

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                sLabel = "Hurling a fireball"
            });

        }

    };

    class Clause2 : ClauseSkillSelection {

        public SoulBurning soulToCopy;

        public Clause2(Skill _skill) : base(_skill) {

            soulToCopy = new SoulBurning(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Apply Burning(4) to that enemy");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulBurning(soulToCopy, chrSelected)));

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.FIREBALL;
    }

}
