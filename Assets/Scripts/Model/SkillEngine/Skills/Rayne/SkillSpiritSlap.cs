using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSpiritSlap : Skill {

    public Damage dmg;
    public int nBaseDamage;

    public SkillSpiritSlap(Chr _chrOwner) : base(_chrOwner) {

        sName = "SpiritSlap";
        sDisplayName = "Spirit Slap";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCooldownInduced = 0;
        nFatigue = 2;

        lstTargets = new List<Target>() {
            new TarChr(Target.AND(TarChr.IsFrontliner(), TarChr.IsDiffTeam(chrOwner)))
        };

        lstClauses = new List<Clause>() {
            new Clause1(this),
        };
    }

    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 5;
        public SoulDispirited soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
            soulToCopy = new SoulDispirited(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to an Enemy Frontliner and  apply DISPIRITED (4).\n" +
                "[DISPIRITED]: This character's cantrips cost [O] more.", dmg.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulDispirited(soulToCopy, chrSelected)) {
                sLabel = "The pain is momentary, but the shame lasts..."
            });

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Rayne/sndSpiritSlap", 3f) },
                sLabel = "Got slapped"
            });

        }

    };

}
