public class ActionHuntersQuarry : Action {

    public ActionHuntersQuarry(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); //Choose an enemy character

        sName = "Hunters Quarry";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 3;
        nActionCost = 1;

		sDescription1 = "Apply HUNTED to the chosen character.";
        sDescription2 = "[HUNTED]\n" + "Before " + _chrOwner.sName + " deals damage to this character, they lose 5 DEFENSE until end of turn.";

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Chr tarChr = Chr.GetTargetByIndex(lstTargettingIndices[0]);

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulHunted(_chrSource, _chrTarget);
                    }

                });
            }
        });

    }

}
