using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAdvance : Skill {

    public SkillAdvance(Chr _chrOwner) : base(_chrOwner) {

        sName = "Advance";
        sDisplayName = "Advance";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(1, 0, 0, 0, 0));

        nCooldownInduced = 6;
        nFatigue = 4;

        lstTargets = new List<Target>() {

        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {


        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Switch to the middle Frontline Position");
        }

        public override void ClauseEffect(Selections selections) {
            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecSwitchChar(skill.chrOwner, chrSelected, ContPositions.Get().GetAlliedFrontlinePositions(chrSelected.plyrOwner)[1]) {
                sLabel = "Booping ya back"
            });

        }

    };


}
