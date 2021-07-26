using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHuntersQuarry : Skill {

    public SkillHuntersQuarry(Chr _chrOwner) : base(_chrOwner) {

        sName = "HuntersQuarry";
        sDisplayName = "Hunter's Quarry";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCooldownInduced = 8;
        nFatigue = 3;

        lstTargets.Add(new TarChr((c, lstArgs) => (Chr)c != chrOwner));

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        public SoulHunted soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            soulToCopy = new SoulHunted(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Apply HUNTED to another chosen character." +
                "[HUNTED]: Before {0} deals damage to this character, they lose {1} DEFENSE until end of turn.", skill.chrOwner.sName, soulToCopy.nDefenseLoss);
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulHunted(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndHuntersQuarry", 0.867f) },
                sLabel = "I'm gonna get ya"
            });


        }

    };

}
