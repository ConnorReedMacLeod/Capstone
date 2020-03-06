using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpiritSlap : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionSpiritSlap(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "SpiritSlap";
        sDisplayName = "Spirit Slap";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCd = 0;
        nFatigue = 2;
        nActionCost = 1;
        
        lstClauses = new List<Clause>() {
            new Clause1(this),
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 5;
        public SoulDispirited soulToCopy;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrMelee(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });
            
            dmg = new Damage(action.chrSource, null, nBaseDamage);
            soulToCopy = new SoulDispirited(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the enemy blocker and  apply DISPIRITED (4).\n" +
                "[DISPIRITED]: This character's cantrips cost [O] more.", dmg.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulDispirited(soulToCopy, chrSelected)) {
                sLabel = "The pain is momentary, but the shame lasts..."
            });

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Rayne/sndSpiritSlap", 3f) },
                sLabel = "Got slapped"
            });

        }

    };

}
