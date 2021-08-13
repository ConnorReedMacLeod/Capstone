using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTwinSnakes : Skill {

    public SkillTwinSnakes(Chr _chrOwner) : base(_chrOwner) {

        sName = "TwinSnakes";
        sDisplayName = "Twin Snakes";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 1 });

        nCooldownInduced = 8;
        nFatigue = 4;


        lstTargets = new List<Target>() {
            new TarChr(TarChr.IsDiffTeam(chrOwner))
        };

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 20;
        public int nLifeloss = 5;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the chosen character.  Lose {1} health", dmg.Get(), nLifeloss);
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecLoseLife(skill.chrOwner, skill.chrOwner, nLifeloss) {
                sLabel = "Owie"
            });

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndTwinSnakes", 2f) },
                sLabel = "Snakey, no!"
            });

        }

    };

}