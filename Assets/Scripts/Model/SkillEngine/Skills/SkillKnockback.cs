using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKnockback : Skill {

    public SkillKnockback(Chr _chrOwner) : base(_chrOwner) {

        sName = "Knockback";
        sDisplayName = "Knockback";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(1, 0, 0, 0, 0));

        nCooldownInduced = 6;
        nFatigue = 4;

        lstTargets = new List<Target>() {
            new TarMana(this, manaCost),
            new TarChr(this, TarChr.IsDiffTeam(chrOwner))
        };

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause2(this)
        };
    }

    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 5;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to an Enemy", dmg.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                sLabel = "Booping ya back"
            });

        }

    };

    class Clause2 : Clause {

        Damage dmg;
        public int nBaseDamage = 5;

        public Clause2(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Move that Enemy to the Position behind them.");
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            //TODO - maybe add some sort of additional function that can be called exactly when the executable resolves to trigger additional effects
            //    e.g., here it could be a structure called Tracking where you call Tracking.BeforeEffect() to track the gamestate before the executable
            //          evaluates (this can store information, and then you call Tracking.AfterEffect() to
            ContSkillEngine.PushSingleExecutable(new ExecMoveChar(skill.chrOwner, chrSelected, (chrTarget) => ContPositions.Get().GetBehindPosition(chrTarget.position)));

        }

    };

}
