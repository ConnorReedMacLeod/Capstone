using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBunker : Skill {

    public SkillBunker(Chr _chrOwner) : base(_chrOwner, 0) {//set the dominant clause

        sName = "Bunker";
        sDisplayName = "Bunker";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 1, 0, 0 });

        nCooldownInduced = 4;
        nFatigue = 6;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public SoulPositionBunker soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSelf(this), //Base Tag always goes first
            });

            soulToCopy = new SoulPositionBunker(skill.chrSource, null, skill);
        }

        public override string GetDescription() {

            return string.Format("The Position under this character gives +{0} DEFENSE to the character on it for {1} turns.", soulToCopy.nDefenseBuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulPosition(skill.chrSource, skill.chrSource.position, new SoulPositionBunker(soulToCopy, skill.chrSource.position)) {

                sLabel = "Hunker down"
            });

        }

    };

}