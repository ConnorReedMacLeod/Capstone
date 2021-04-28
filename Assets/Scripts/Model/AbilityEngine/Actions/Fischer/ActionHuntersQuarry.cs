using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHuntersQuarry : Action {

    public ActionHuntersQuarry(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "HuntersQuarry";
        sDisplayName = "Hunter's Quarry";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 3;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {
        
        public SoulHunted soulToCopy;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
            });
            
            soulToCopy = new SoulHunted(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Apply HUNTED to the chosen character." +
                "[HUNTED]: Before {0} deals damage to this character, they lose {1} DEFENSE until end of turn.", action.chrSource.sName, soulToCopy.nDefenseLoss);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulHunted(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndHuntersQuarry", 0.867f) },
                sLabel = "I'm gonna get ya"
            });


        }

    };

}
