using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTranquilize : Skill {

    public SkillTranquilize(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "Tranquilize";
        sDisplayName = "Tranquilize";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCooldownInduced = 11;
        nFatigue = 3;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public int nStunAmount;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrMelee(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });

            nStunAmount = 4;
        }

        public override string GetDescription() {

            return string.Format("Deal {0} fatigue to the enemy Vanguard.", nStunAmount);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecStun(skill.chrOwner, chrSelected, nStunAmount) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndTranquilize", 1.4f) },
                sLabel = "Shhh... Look at my daughter."
            });

        }

    };

}