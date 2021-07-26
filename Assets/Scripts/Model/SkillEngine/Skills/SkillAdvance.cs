using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAdvance : Skill {

    public SkillAdvance(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Advance";
        sDisplayName = "Advance";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCooldownInduced = 6;
        nFatigue = 4;



        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {


        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSelf(this), //Base Tag always goes first
            });


        }

        public override string GetDescription() {

            return string.Format("Switch to the middle Frontline Position");
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecSwitchChar(skill.chrOwner, chrSelected, ContPositions.Get().GetAlliedFrontlinePositions(chrSelected.plyrOwner)[1]) {
                sLabel = "Booping ya back"
            });

        }

    };


}
