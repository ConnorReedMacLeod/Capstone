using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTantrum : Skill {

    public SkillTantrum(Chr _chrOwner) : base(_chrOwner) {

        sName = "Tantrum";
        sDisplayName = "Tantrum";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(1, 0, 0, 1, 0));

        nCooldownInduced = 9;
        nFatigue = 5;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
    }

    class Clause1 : ClauseSkillSelection {

        Damage dmgEnemy;
        public int nEnemyDamage = 20;

        Damage dmgAlly;
        public int nAllyDamage = 5;

        public Clause1(Skill _skill) : base(_skill) {

            dmgEnemy = new Damage(skill.chrOwner, null, nEnemyDamage);
            dmgAlly = new Damage(skill.chrOwner, null, nAllyDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to all enemies and {1} damage to all other allies", dmgEnemy.Get(), dmgAlly.Get());
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            //First, deal damage to all enemies
            List<Chr> lstChrEnemy = skill.chrOwner.plyrOwner.GetEnemyPlayer().GetActiveChrs();

            for(int i = 0; i < lstChrEnemy.Count; i++) {
                //For each enemy, deal our dmgEnemy to them
                ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, lstChrEnemy[i], dmgEnemy) {
                    sLabel = "WAAAAAAAHWAAHWAHHH"
                });
            }

            //Then damage each of our allies
            List<Chr> lstChrAlly = skill.chrOwner.plyrOwner.GetActiveChrs();

            for(int i = 0; i < lstChrAlly.Count; i++) {
                //For each ally, deal our dmgAlly to them
                ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, lstChrAlly[i], dmgAlly) {
                    sLabel = "Really, dude?"
                });
            }

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.TANTRUM;
    }

}