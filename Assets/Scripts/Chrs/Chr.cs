using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ViewChr))]
public class Chr : MonoBehaviour {

    bool bStarted;

    public enum STATESELECT {
        SELECTED,                   //Selected a character (to see status effects, skills)
        TARGGETING,                 //Targetting of character skills
        IDLE                        //Default character state
    };

    public CharType.CHARTYPE chartype; //The type of character this is acting as (e.g., Fischer)

    public string sName;            //The name of the character
    public Player plyrOwner;        //The player who controls the character

    public int id;                  //The character's unique identifier (across all characters)
    public int nFatigue;            //Number of turns a character must wait before their next skill
    public StateReadiness curStateReadiness; //A reference to the current state of readiness

    public const int nMaxSkillUsesPerActivation = 1;     //The total maximum number of skills a character can use in a turn (usually 1, cantrips cost 0)

    public int nCurHealth;          //The character's current health
    public Property<int> pnMaxHealth;          //The character's max health

    public List<Discipline.DISCIPLINE> lstDisciplines; //The disciplines the character has access to

    public Timestamp timestampDeath;           //Stores the timestamp at which this character died (or null, if they aren't dead)
    public bool bDead;                         //If the character is dead or not

    public Property<int> pnPower;              //The character's current power
    public Property<int> pnDefense;            //The character's current defense

    public Property<int> pnArmour;          //The character's current armour
    public int nAbsorbedArmour;             //The amount of damage currently taken by armour

    public Property<bool> pbCanSwapIn;       //Can this character swap in from the bench

    public SkillSlot[] arSkillSlots;      //The slots for the character's currently usable skills - these keep track of the cooldowns of those skills
    public const int nEquippedCharacterSkills = 4; //Number of non-generic (non-rest) skills currently active on the character
    public const int nBenchCharacterSkills = 4; //Number of benched skills the character could adapt into
    public const int nTotalCharacterSkills = nEquippedCharacterSkills + nBenchCharacterSkills; // Total pool of available skills for this character
    public const int nUsableSkills = nEquippedCharacterSkills + 1; //Number of all skills (including generics)
    public SkillRest skillRest;  //The standard reference to the rest skill the character can use
    public const int nRestSlotId = nEquippedCharacterSkills; //Id of the skillslot containing the rest skill

    //If we need extra modifiers for if we can or cannot be selected for a given skill's target, then 
    //  we can decorate them in this property.  By default we don't do any overrides and just listen to what the TarChr says
    public delegate bool CanBeSelectedBy(TarChr tar, InputSkillSelection selectionsSoFar, bool bCanDefaultSelect);
    public Property<CanBeSelectedBy> pOverrideCanBeSelectedBy;

    public Position position;       //A reference to the position the character is on

    public SoulContainerChr soulContainer; //A reference to the character's list of soul effects

    public ViewChr view;

    public STATESELECT stateSelect; //The character's state

    public Subject subStartSelect = new Subject();
    public static Subject subAllStartSelect = new Subject(Subject.SubType.ALL);
    public Subject subStartTargetting = new Subject();
    public static Subject subAllStartTargetting = new Subject(Subject.SubType.ALL);
    public Subject subStartIdle = new Subject();
    public static Subject subAllStartIdle = new Subject(Subject.SubType.ALL);

    public Subject subBecomesActiveForHumans = new Subject(); // When this character's turn for selecting skills begins
    public Subject subEndsActiveForHumans = new Subject(); // When this character's turn for selecting skills ends

    public Subject subBecomesTargettable = new Subject(); // When a skill that is choosing targets can target this character
    public Subject subEndsTargettable = new Subject(); // When the skill that could target this character stops its targetting process

    public Subject subBeforeActivatingSkill = new Subject();
    public static Subject subAllBeforeActivatingSkill = new Subject(Subject.SubType.ALL);
    public Subject subPreExecuteSkill = new Subject();
    public static Subject subAllPreExecuteSkill = new Subject(Subject.SubType.ALL);
    public Subject subPostExecuteSkill = new Subject();
    public static Subject subAllPostExecuteSkill = new Subject(Subject.SubType.ALL);

    public Subject subLifeChange = new Subject();
    public Subject subArmourCleared = new Subject();
    public Subject subFatigueChange = new Subject();
    public static Subject subAllFatigueChange = new Subject(Subject.SubType.ALL);
    public Subject subSwitchingInChange = new Subject();
    public Subject subChannelTimeChange = new Subject();

    public Subject subLeftAnyPosition = new Subject();
    public Subject subEnteredAnyPosition = new Subject();

    public Subject subLeftInPlayPosition = new Subject();
    public Subject subEnteredInPlayPosition = new Subject();

    public Subject subLeftBench = new Subject();
    public Subject subEnteredBench = new Subject();

    public Subject subStatusChange = new Subject();
    public static Subject subAllStatusChange = new Subject(Subject.SubType.ALL);

    public Subject subSoulApplied = new Subject();
    public Subject subSoulRemoved = new Subject();

    public Subject subStunApplied = new Subject();

    public Subject subDeath = new Subject();
    public static Subject subAllDeath = new Subject(Subject.SubType.ALL);

    public void SetStateReadiness(StateReadiness newState) {

        if(curStateReadiness != null) {
            curStateReadiness.OnLeave();
        }

        curStateReadiness = newState;

        if(curStateReadiness != null) {
            curStateReadiness.OnEnter();
        }
    }

    public override string ToString() {
        return "Chr(" + sName + ")";
    }

    public Skill GetRandomActiveSkill() {
        return arSkillSlots[Random.Range(0, nEquippedCharacterSkills)].skill;
    }

    public Skill GetRandomSkill() {
        //Sometimes throw in random selections of resting with weighted changes
        int nRand = Random.Range(0, 100);

        if(nRand < 25) {
            return skillRest;
        } else {
            return GetRandomActiveSkill();
        }
    }


    public void ChangeChanneltime(int _nChange) {
        //Just let our readiness state deal with this
        curStateReadiness.ChangeChanneltime(_nChange);

        subChannelTimeChange.NotifyObs();
    }

    public bool IsDying() {
        if(nCurHealth <= 0) {
            return true;
        }

        //TODO - put some insta-death trigger in

        return false;
    }

    //After a character has been flagged for death, we can use this method to actually execute
    //  the killing of the character - shouldn't be called directly, instead flag the character for death
    //  via ContDeaths.AddDyingChr(chr);
    public void KillFlaggedCharacter() {
        if(bDead) {
            Debug.LogError("Trying to kill a character thast's already dead");
            return;
        }

        //Mark ourselves as being properly dead (for the purposes of target validation, etc.)
        bDead = true;

        //Add ourselves to the collection of dead characters
        ContDeaths.Get().lstDeadChrs.Add(this);

        //interrupt any channel that we may be using 
        curStateReadiness.InterruptChannel();

        //Create a new death state to let our character transition to
        StateDead newState = new StateDead(this);

        //Transition to the new state
        SetStateReadiness(newState);

        //Remove all soul effects that are present on this chr
        soulContainer.RemoveAllSoul();

        Debug.Log("Remember to dispell any soul effects on other characters that require targetting this character - should be done " +
                "on a per-soul effect basis when notified of a subDeath");


        //Remove ourselves from the turn-priority queue since we'll no longer be acting
        ContTurns.Get().RemoveChrFromPriorityList(this);

        //Save a reference to our position before we clear ourselves out of it
        Position posVacated = position;

        ContPositions.Get().DeleteChrFromPosition(this);

        //Flag our position as now being emptied, so it may need to be filled by a new character
        ContSkillEngine.Get().NotifyOfNewEmptyPosition(posVacated);

        //Notify anyone that we have died
        subDeath.NotifyObs(this);
        Chr.subAllDeath.NotifyObs(this);
    }


    // Apply this amount of fatigue to the character
    public void ChangeFatigue(int _nChange, bool bGlobalFatigueChange = false) {
        if(_nChange + nFatigue < 0) {
            nFatigue = 0;
        } else {
            nFatigue += _nChange;
        }

        subFatigueChange.NotifyObs(this);
        subAllFatigueChange.NotifyObs(this);

        //TODO:: Probably delete this bGlobalFatigueChange flag once I get a nice solution for priority handling
        if(bGlobalFatigueChange == false) {
            //Then this is an individual fatigue change for a single character that may change their priority
            ContTurns.Get().FixSortedPriority(this);
            //So make sure we're in the right place in the priority list
        }
    }

    public int GetPriority() {

        //Just ask our readiness state what our priority is
        return curStateReadiness.GetPriority();
    }

    public void RechargeSkills() {

        //Only bother recharging the active skills since those will be the only ones that can be on cooldown
        for(int i = 0; i < Chr.nEquippedCharacterSkills; i++) {

            //Only reduce the cooldown if it is not currently off cooldown
            if(arSkillSlots[i].nCooldown > 0) {
                ContSkillEngine.Get().AddExec(new ExecChangeCooldown(null, arSkillSlots[i], -1) {

                    fDelay = ContTime.fDelayMinorSkill
                });
            }
        }

    }

    //If we have lowered our armour (either by taking damage, or by
    // having an armour buff expire), then check if we have no armour left
    public void CheckNoArmour() {

        if(pnArmour.Get() < nAbsorbedArmour) {
            //Debug.LogError("ERROR - " + sName + "'s armour is at " + pnArmour.Get() + " but we've absorbed " + nAbsorbedArmour);
        } else if(pnArmour.Get() == 0) {
            //Then we have used up all of our armour

            //Reset the armour absorbed amount to 0
            nAbsorbedArmour = 0;

            //So clear out the armour modifiers and reset armour to 0
            pnArmour = new Property<int>(0);

            //Notify all Armour buffs on this character that they should be dispelled
            subArmourCleared.NotifyObs();
        }

    }

    public void AddArmour(int nChange) {

        //TODO:: Just make a nAbsorbedByArmour value that keeps track of damage sustained by armour
        //       If this ever goes negative, or reachs 0, clear the pnArmour modifiers
        //       If a Armour Modifier ever expires, then it should also decrease the nAbsorbedByArmour
        //       by the same amount

        pnArmour.AddModifier((nBelow) => nBelow += nChange);

    }

    public void DamageArmour(int nDamage) {

        //Increase the current amount of damage absorbed by armour value
        nAbsorbedArmour += nDamage;

        //Then check if we've destroyed all of the armour currently on the character
        CheckNoArmour();

    }



    public void TakeDamage(Damage dmgToTake) {

        //Fetch the amount of damage we're going to take
        int nDamageToTake = dmgToTake.Get();

        //If the damage isn't piercing, then reduce it by the defense amount
        if(dmgToTake.bPiercing == false) {
            //Reduce the damage to take by our defense (but ensure it doesn't go below 0)
            nDamageToTake = Mathf.Max(0, nDamageToTake - pnDefense.Get());
        }

        int nArmouredDamage = 0;

        if(dmgToTake.bPiercing == false) {
            //Deal as much damage as we can (but not more than how much armour we have)
            nArmouredDamage = Mathf.Min(nDamageToTake, pnArmour.Get());

            //If there's actually damage that needs to be dealt to armour
            if(nArmouredDamage > 0) {
                DamageArmour(nArmouredDamage);
            }
        }

        //Calculate how much damage still needs to be done after armour
        int nAfterArmourDamage = nDamageToTake - nArmouredDamage;

        //If there's damage to be done, then deal it to health
        if(nAfterArmourDamage > 0) {
            ChangeHealth(-nAfterArmourDamage);
        }

        //Maybe notify that we're taking damage
    }

    public void TakeHealing(Healing healToTake) {

        //Fetch the amount of healing we're going to take
        int nHealingToTake = healToTake.Get();

        //If there's healing to be done, then apply it to our health
        if(nHealingToTake > 0) {
            ChangeHealth(nHealingToTake);
        }

        //maybe notify people that we've been healed
    }


    public void ChangeHealth(int nChange) {
        SetHealth(nCurHealth + nChange);
    }

    public void SetHealth(int nNewHealth) {

        bool bAliveBefore = nCurHealth > 0;
        int nHealthBefore = nCurHealth;

        if(nNewHealth > pnMaxHealth.Get()) {
            //Set the character's life to maximum if they would go above that
            nCurHealth = pnMaxHealth.Get();
        } else {
            nCurHealth = nNewHealth;
        }

        bool bAliveAfter = nCurHealth > 0;

        //Check if this health change causes us to go from living to dying
        if(bAliveBefore == true && bAliveAfter == false) {
            //Then we just died - flag ourselves for death
            ContDeaths.Get().AddDyingChr(this);
        }

        subLifeChange.NotifyObs(this, nCurHealth - nHealthBefore);
    }

    public void SetPosition(Position _position) {
        if(position == _position) return;

        position = _position;
    }

    //Counts down the character's recharge time
    public void TimeTick() {
        ChangeFatigue(-1, true);
    }

    public void ChangeState(STATESELECT _stateSelect) {
        stateSelect = _stateSelect;

        subStatusChange.NotifyObs(this);
        subAllStatusChange.NotifyObs(this);
    }

    //Sets character state to selected
    public void Select() {
        ChangeState(STATESELECT.SELECTED);

        subStartSelect.NotifyObs(this);
        subAllStartSelect.NotifyObs(this);
    }

    //Sets character state to targetting
    public void Targetting() {
        ChangeState(STATESELECT.TARGGETING);

        subStartTargetting.NotifyObs(this);
        subAllStartTargetting.NotifyObs(this);
    }

    //Set character state to unselected
    public void Idle() {
        Debug.Log("Remember to get rid of this Idle() system");
        ChangeState(STATESELECT.IDLE);

        subStartIdle.NotifyObs(this);
        subAllStartIdle.NotifyObs(this);
    }



    // Used to initiallize information fields of the Chr
    // Call this after creating to set information
    public void InitChr(CharType.CHARTYPE _chartype, Player _plyrOwner, LoadoutManager.Loadout loadout) {
        chartype = _chartype;
        sName = CharType.GetChrName(chartype);
        plyrOwner = _plyrOwner;

        ChrCollection.Get().AddChr(this);

        //Initialize this character's disciplines based on their chartype
        InitDisciplines();

        //Initialize any loadout-specific qualities of the character
        InitFromLoadout(loadout);

        view.Init();
    }

    public void InitGenericSkills() {

    }

    public void InitSkillSlots() {

        arSkillSlots = new SkillSlot[nUsableSkills];

        for(int i = 0; i < nUsableSkills; i++) {
            arSkillSlots[i] = new SkillSlot(this, i);
        }


        skillRest = new SkillRest(this);

        arSkillSlots[nRestSlotId].SetSkill(skillRest);
    }

    public bool HasSkillEquipped(SkillType.SKILLTYPE skilltype) {
        //Loop through our skill slots and check if one of them has the desired skilltype
        for(int i = 0; i < nEquippedCharacterSkills; i++) {
            if(arSkillSlots[i].skill.GetSkillType() == skilltype) {
                return true;
            }
        }

        return false;
    }

    public void InitDisciplines() {
        //Should this be done as a copy?
        lstDisciplines = CharType.GetDisciplines(chartype);
    }

    public void InitFromLoadout(LoadoutManager.Loadout loadout) {

        //Load in all the equipped skills
        for(int i = 0; i < Chr.nEquippedCharacterSkills; i++) {
            arSkillSlots[i].SetSkill(loadout.lstChosenSkills[i]);
        }


        //TODO - store all the benched skills as well

    }

    public bool IsLocallyOwned() {
        return NetworkMatchSetup.IsLocallyOwned(plyrOwner.id);
    }

    // Sets up fundamental class connections for the Chr
    public void Start() {
        if(bStarted == false) {
            bStarted = true;

            InitSkillSlots();

            stateSelect = STATESELECT.IDLE;

            pnMaxHealth = new Property<int>(100);
            nCurHealth = pnMaxHealth.Get();
            pnArmour = new Property<int>(0);

            pnPower = new Property<int>(0);
            pnDefense = new Property<int>(0);

            //By default, we don't override any targetting - just listen to the base response of if the target can select us
            pOverrideCanBeSelectedBy = new Property<CanBeSelectedBy>((tar, selectionsSoFar, bCanSelectSoFar) => bCanSelectSoFar);
            pbCanSwapIn = new Property<bool>(() => position.positiontype == Position.POSITIONTYPE.BENCH);

            SetStateReadiness(new StateFatigued(this));

            view = GetComponent<ViewChr>();
            view.Start();

        }

    }

}



//Add a max health initializer in each instance of a character - add an 
// initializer in the base chr that sets curhealth to max health