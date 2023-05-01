using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSummonSlime : Skill {

    public SkillSummonSlime(Chr _chrOwner) : base(_chrOwner) {

        sName = "SummonSlim";
        sDisplayName = "Summon Slime";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 1, 0));

        nCooldownInduced = 3;
        nFatigue = 1;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarPosition.AddTarget(this, Target.AND(TarPosition.IsSameTeam(chrOwner), TarPosition.IsEmptyPosition()));
    }

    class Clause1 : ClauseSkillSelection {


        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Summon a Slime on an Unoccupied Allied Position");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Position posToSummonTo = (Position)selections.lstSelections[1];
            LoadoutManager.Loadout loadout = LoadoutManager.GetDefaultLoadoutForChar(CharType.CHARTYPE.SLIME);

            ContSkillEngine.PushSingleExecutable(new ExecSummonChrToPosition(skill.chrOwner, posToSummonTo, CharType.CHARTYPE.SLIME, skill.chrOwner.plyrOwner, loadout) {
                sLabel = "Sliming it up"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.SUMMONSLIME;
    }


}
