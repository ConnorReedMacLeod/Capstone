using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManaBlossom : Skill {

    public SkillManaBlossom(Chr _chrOwner) : base(_chrOwner) {

        sName = "ManaBlossom";
        sDisplayName = "Mana Blossom";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 1, 0, 1));

        nCooldownInduced = 14;
        nFatigue = 6;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
    }

    class Clause1 : ClauseSkillSelection {

        public Mana manaToAdd;

        public Clause1(Skill _skill) : base(_skill) {
            manaToAdd = new Mana(0, 0, 1, 0, 0);
        }

        public override string GetDescription() {

            return string.Format("Add {0} to this date of your Mana Calendar", manaToAdd.ToPrettyString());
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            ManaDate manadateCur = skill.chrOwner.plyrOwner.manacalendar.GetCurrentManaDate();
            Property<Mana>.Modifier modManaToAdd = new Property<Mana>.Modifier((Mana mana) => Mana.AddMana(mana, manaToAdd));

            ContSkillEngine.PushSingleExecutable(new ExecApplyManaDateMod(skill.chrOwner, manadateCur, modManaToAdd) {
                sLabel = "Planting a new seed"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.MANABLOSSOM;
    }

}
