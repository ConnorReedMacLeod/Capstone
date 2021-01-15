using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTwinSnakes : Action {

    public ActionTwinSnakes(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "TwinSnakes";
        sDisplayName = "Twin Snakes";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 1 });

        nCd = 8;
        nFatigue = 4;

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 20;
        public int nLifeloss = 5;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this) //Base Tag always goes first
            });


            dmg = new Damage(action.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the chosen character.  Lose {1} health", dmg.Get(), nLifeloss);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndTwinSnakes", 2f) },
                sLabel = "Snakey, no!"
            });

        }

    };

}