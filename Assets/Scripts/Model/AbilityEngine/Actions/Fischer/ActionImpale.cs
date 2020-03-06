using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionImpale : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionImpale(Chr _chrOwner) : base(_chrOwner, 0) {//set the dominant clause to 0

        sName = "Impale";
        sDisplayName = "Impale";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCd = 6;
        nFatigue = 2;
        nActionCost = 1;

        nBaseDamage = 20;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 20;
        public SoulImpaled soulToCopy;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrMelee(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });


            dmg = new Damage(action.chrSource, null, nBaseDamage);
            soulToCopy = new SoulImpaled(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the enemy Vanguard and reduce their max health by {1}.", dmg.Get(), soulToCopy.nMaxLifeReduction);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulImpaled(soulToCopy, chrSelected)));

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndImpale", 1.833f) },
                sLabel = "Get Kakyoin'd"
            });

            
        }

    };

}
