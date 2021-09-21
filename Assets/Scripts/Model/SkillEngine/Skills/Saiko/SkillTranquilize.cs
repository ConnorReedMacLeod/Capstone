using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTranquilize : Skill {

    public SkillTranquilize(Chr _chrOwner) : base(_chrOwner) {

        sName = "Tranquilize";
        sDisplayName = "Tranquilize";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 1, 0, 0, 0));

        nCooldownInduced = 11;
        nFatigue = 3;

        lstTargets = new List<Target>() {
            new TarChr(this, Target.AND(TarChr.IsDiffTeam(chrOwner), TarChr.IsFrontliner()))
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        public int nStunAmount;

        public Clause1(Skill _skill) : base(_skill) {

            nStunAmount = 4;
        }

        public override string GetDescription() {

            return string.Format("Deal {0} fatigue to an enemy frontliner.", nStunAmount);
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecStun(skill.chrOwner, chrSelected, nStunAmount) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndTranquilize", 1.4f) },
                sLabel = "Shhh... Look at my daughter."
            });

        }

    };

}