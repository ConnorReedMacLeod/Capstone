using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTantrum : Skill {

    public SkillTantrum(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Tantrum";
        sDisplayName = "Tantrum";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 1, 0 });

        nCooldownInduced = 9;
        nFatigue = 5;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseSpecial {

        Damage dmgEnemy;
        public int nEnemyDamage = 20;

        Damage dmgAlly;
        public int nAllyDamage = 5;

        public Clause1(Skill _skill) : base(_skill) {
            //TODO - add tags as needed

            dmgEnemy = new Damage(skill.chrSource, null, nEnemyDamage);
            dmgAlly = new Damage(skill.chrSource, null, nAllyDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to all enemies and {1} damage to all other allies", dmgEnemy.Get(), dmgAlly.Get());
        }

        public override void ClauseEffect() {

            //First, deal damage to all enemies
            List<Chr> lstChrEnemy = skill.chrSource.plyrOwner.GetEnemyPlayer().GetActiveChrs();

            for(int i = 0; i < lstChrEnemy.Count; i++) {
                //For each enemy, deal our dmgEnemy to them
                ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrSource, lstChrEnemy[i], dmgEnemy) {
                    sLabel = "WAAAAAAAHWAAHWAHHH"
                });
            }

            //Then damage each of our allies
            List<Chr> lstChrAlly = skill.chrSource.plyrOwner.GetActiveChrs();

            for(int i = 0; i < lstChrAlly.Count; i++) {
                //For each ally, deal our dmgAlly to them
                ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrSource, lstChrAlly[i], dmgAlly) {
                    sLabel = "Really, dude?"
                });
            }

        }

    };

}