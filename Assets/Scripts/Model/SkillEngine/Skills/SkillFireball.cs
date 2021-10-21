using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFireball : Skill {

    public SkillFireball(Chr _chrOwner) : base(_chrOwner) {

        sName = "Fireball";
        sDisplayName = "Fireball";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 1, 0, 0), true);

        nCooldownInduced = 6;
        nFatigue = 4;


        lstTargets = new List<Target>() {
            new TarMana(this, manaCost),
            new TarChr(this, TarChr.IsDiffTeam(chrOwner))
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 10;

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Deal {0}*X damage to an Enemy", nBaseDamage);
        }

        public override void ClauseEffect(Selections selections) {

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

    class Clause2 : Clause {

        public SoulBurning soulToCopy;

        public Clause2(Skill _skill) : base(_skill) {

            soulToCopy = new SoulBurning(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Apply Burning(4) to that enemy");
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulBurning(soulToCopy, chrSelected)));

        }

    };

}
