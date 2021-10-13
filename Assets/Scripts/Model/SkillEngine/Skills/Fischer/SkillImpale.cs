using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillImpale : Skill {

    public Damage dmg;
    public int nBaseDamage;

    public SkillImpale(Chr _chrOwner) : base(_chrOwner) {

        sName = "Impale";
        sDisplayName = "Impale";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(1, 0, 0, 0, 0));

        nCooldownInduced = 6;
        nFatigue = 2;

        nBaseDamage = 20;
        //Create a base Damage object that this skill will apply
        dmg = new Damage(this.chrOwner, null, nBaseDamage);

        lstTargets = new List<Target>() {
            new TarMana(this, manaCost),
            new TarChr(this, Target.AND(TarChr.IsDiffTeam(chrOwner), TarChr.IsFrontliner()))
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 20;
        public SoulImpaled soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {

            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
            soulToCopy = new SoulImpaled(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the enemy Vanguard and reduce their max health by {1}.", dmg.Get(), soulToCopy.nMaxLifeReduction);
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulImpaled(soulToCopy, chrSelected)));

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndImpale", 1.833f) },
                sLabel = "Get Kakyoin'd"
            });


        }

    };

}
