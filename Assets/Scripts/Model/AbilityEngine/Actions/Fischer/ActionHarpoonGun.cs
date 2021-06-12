using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHarpoonGun : Action {

    public ActionHarpoonGun(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "HarpoonGun";
        sDisplayName = "Harpoon Gun";

        //We don't have any specific effect to take place while channeling, so just leave the
        // soulChannel effect null and let it copy our execution's effect for what it does when the channel completes
        type = new TypeChannel(this, 2, null);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 2 });

        nCooldownInduced = 5;
        nFatigue = 2;

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause2(this)
        };
    }


    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 30;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });


            dmg = new Damage(action.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("After channeling, deal {0} damage to the chosen enemy.", dmg.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            Debug.Log("Executing damaging clause");

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndHarpoonGun", 2.067f) },
                sLabel = "Behold, the power of my stand, Beach Boy!"
            });

        }

    };

    class Clause2 : ClauseChr {

        public Clause2(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });
        }

        public override string GetDescription() {

            return string.Format("That enemy becomes the blocker.");
        }

        public override void ClauseEffect(Chr chrSelected) {

            Debug.Log("Executing become blocker clause");

            ContAbilityEngine.PushSingleExecutable(new ExecBecomeBlocker(action.chrSource, chrSelected) {
                sLabel = "Hey, I caught one!"
            });

        }

    };

}