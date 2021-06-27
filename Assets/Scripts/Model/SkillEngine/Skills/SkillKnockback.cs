using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKnockback : Skill {

    public SkillKnockback(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Knockback";
        sDisplayName = "Knockback";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCooldownInduced = 6;
        nFatigue = 4;



        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause2(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 5;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });


            dmg = new Damage(skill.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to an Enemy", dmg.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrSource, chrSelected, dmg) {
                sLabel = "Booping ya back"
            });

        }

    };

    class Clause2 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 5;

        public Clause2(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });

            dmg = new Damage(skill.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Move that Enemy to the Position behind them.");
        }

        public override void ClauseEffect(Chr chrSelected) {

            //TODO - maybe add some sort of additional function that can be called exactly when the executable resolves to trigger additional effects
            //    e.g., here it could be a structure called Tracking where you call Tracking.BeforeEffect() to track the gamestate before the executable
            //          evaluates (this can store information, and then you call Tracking.AfterEffect() to
            ContSkillEngine.PushSingleExecutable(new ExecMoveChar(skill.chrSource, chrSelected, (chrTarget) => ContPositions.Get().GetBehindPosition(chrTarget.position)));

        }

    };

}
