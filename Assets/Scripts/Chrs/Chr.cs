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

    public int nSwitchingInTime;    //Number of turns a character must wait before being able to act after switching in from the bench

    public StateReadiness curStateReadiness; //A reference to the current state of readiness
    public Timestamp timestampLastActed; //A Timestamp of when last acted or last entered play

    public const int nMaxSkillUsesPerActivation = 1;     //The total maximum number of skills a character can use in a turn (usually 1, cantrips cost 0)

    public int nCurHealth;          //The character's current health
    public Property<int> pnMaxHealth;          //The character's max health

    public List<Discipline.DISCIPLINE> lstDisciplines; //The disciplines the character has access to

    public Timestamp timestampDeath;           //Stores the timestamp at which this character died (or null, if they aren't dead)
    public bool bDead;                         //If the character is dead or not

    public Property<int> pnPower;              //The character's current power
    public Property<int> pnDefense;            //The character's current defense
    public Property<int> pnPowerMult;          //The character's current additional multiplicative power
    public Property<int> pnDefenseMult;          //The character's current additional  multiplicative defense

    public Property<int> pnArmour;          //The character's current armour
    public int nAbsorbedArmour;             //The amount of damage currently taken by armour

    public Property<bool> pbCanSwapIn;       //Can this character swap in from the bench

    public SkillSlot[] arSkillSlots;      //The slots for the character's currently usable skills - these keep track of the cooldowns of those skills
    public int nEquippedChosenSkills;  //The current number of non-generic (non-rest) skills active on the character
    public const int nMaxEquippedChosenSkills = 4; //Maximum number of non-generic (non-rest) skills currently active on the character 
    public int nBenchChosenSkills;     //The current number of benched skills the character could adapt into
    public const int nMaxBenchChosenSkills = 4; //Maximum number of benched skills the character could adapt into
    public const int nMaxTotalChosenSkills = nMaxEquippedChosenSkills + nMaxBenchChosenSkills; // Maximum total pool of available skills for this character
    public const int nFixedGenericSkills = 1; //The number if pre-defined skills that all Chrs share 
    public const int iRestSkill = nMaxTotalChosenSkills + 0;       //The index where the generic rest skill is stored
    public const int nTotalSkills = nMaxTotalChosenSkills + nFixedGenericSkills; //The total number of skills available to a character 
    public SkillRest skillRest;  //The standard reference to the rest skill the character can use

    //If we need extra modifiers for if we can or cannot be selected for a given skill's target, then 
    //  we can decorate them in this property.  By default we don't do any overrides and just listen to what the TarChr says
    public delegate bool CanBeSelectedBy(TarChr tar, InputSkillSelection selectionsSoFar, bool bCanDefaultSelect);
    public Property<CanBeSelectedBy> pOverrideCanBeSelectedBy;

    public Position position;       //A reference to the position the character is on
    
    public SoulContainerChr soulContainer; //A reference to the character's list of soul effects
    public SoulSoulBreak soulSoulBreak;

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

    public Subject subSoulbreakChanged = new Subject();

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
        //Generate a random offset to choose the skill, then shift that index to start after all our generic skills
        Debug.LogFormat("Getting random skill in index range 0, {0}", nEquippedChosenSkills);
        return arSkillSlots[Random.Range(0, nEquippedChosenSkills)].skill;

    }

    public Skill GetRandomSkill() {
        //Sometimes throw in random selections of resting with weighted changes
        int nRand = Random.Range(0, 100);

        Debug.LogFormat("Getting random skill for {0} with nRand={1}", sName, nRand);

        if(nRand < 25) {
            Debug.LogFormat("Returning rest {0}", skillRest);
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
        ContDeaths.Get().AddDeadChr(this);

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
    public void ChangeFatigue(int _nChange) {

        if(_nChange + nFatigue < 0) {
            nFatigue = 0;
        } else {
            nFatigue += _nChange;
        }

        subFatigueChange.NotifyObs(this);
        subAllFatigueChange.NotifyObs(this);
        
        //Make sure we're in the right place in the priority list
        ContTurns.Get().FixSortedPriority(this);

    }

    // Apply this amount of switch-in time to the character
    public void ChangeSwitchInTime(int _nChange) {
        if (_nChange + nSwitchingInTime < 0) {
            nSwitchingInTime = 0;
        } else {
            nSwitchingInTime += _nChange;
        }

        subSwitchingInChange.NotifyObs(this);

        //If we have no time remaining on switching in, then we can transition to a fatigued state
        if(nSwitchingInTime == 0) {
            Debug.LogFormat("{0} has finished switching in", this);
            SetStateReadiness(new StateFatigued(this));
        }

        //Make sure we're in the right place in the priority list
        ContTurns.Get().FixSortedPriority(this);

    }

    public int GetPriority() {

        //Just ask our readiness state what our priority is
        return curStateReadiness.GetPriority();
    }

    public void RechargeSkills() {

        //Only bother recharging the active skills since those will be the only ones that can be on cooldown
        for(int i = 0; i < nEquippedChosenSkills; i++) {

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
            ContDeaths.Get().FlagDyingChr(this);
        }

        subLifeChange.NotifyObs(this, nCurHealth - nHealthBefore);
    }

    public void SetPosition(Position _position) {
        if(position == _position) return;

        position = _position;
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
    public void InitChr(CharType.CHARTYPE _chartype, Player _plyrOwner, LoadoutManager.Loadout loadout, int nStartingFatigue, Position posStart) {

        chartype = _chartype;
        sName = CharType.GetChrName(chartype);
        plyrOwner = _plyrOwner;

        ChrCollection.Get().AddChr(this);

        //Initialize this character's disciplines based on their chartype
        InitDisciplines();

        if (posStart.IsActivePosition()) {
            ContTurns.Get().AddChrToPriorityList(this);
        }

        //Set the starting fatigue
        ChangeFatigue(nStartingFatigue);

        //Initialize any loadout-specific qualities of the character
        InitFromLoadout(loadout);
        
        view.Init();

        ContPositions.Get().InitChrToPosition(this, posStart);
    }

    public void InitGenericSkills() {

    }

    public bool HasSkillEquipped(SkillType.SKILLTYPE skilltype) {
        //Loop through our skill slots and check if one of them has the desired skilltype
        for(int i = 0; i < nEquippedChosenSkills; i++) {
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
        
        arSkillSlots = new SkillSlot[nTotalSkills];

        nEquippedChosenSkills = loadout.NumEquippedSkills();
        nBenchChosenSkills = loadout.NumBenchedSkills();

        //First add all of the chosen skills as defined by our loadout
        for (int i = 0; i < Chr.nMaxTotalChosenSkills; i++) {

            //If there's not supposed to be a skill in this slot, then we can leave it blank and flag it as unused
            if(loadout.lstChosenSkills[i] == SkillType.SKILLTYPE.NULL) {
                Debug.Log("TODO - flag skillslots as unused in the UI");
                arSkillSlots[i] = null;
            } else {
                arSkillSlots[i] = new SkillSlot(this, i);
                arSkillSlots[i].SetSkill(loadout.lstChosenSkills[i]);
            }
        }
        
        //Then add in any fixed generic skills
        arSkillSlots[iRestSkill] = new SkillSlot(this, iRestSkill);
        skillRest = new SkillRest(this);
        arSkillSlots[iRestSkill].SetSkill(skillRest);

    }

    public bool IsLocallyOwned() {
        return NetworkMatchSetup.IsLocallyOwned(plyrOwner.id);
    }

    // Sets up fundamental class connections for the Chr
    public void Start() {
        if(bStarted == false) {
            bStarted = true;
            
            stateSelect = STATESELECT.IDLE;

            pnMaxHealth = new Property<int>(100);
            nCurHealth = pnMaxHealth.Get();
            pnArmour = new Property<int>(0);

            pnPower = new Property<int>(0);
            pnDefense = new Property<int>(0);

            pnPowerMult = new Property<int>(0);
            pnDefenseMult = new Property<int>(0);

            //By default, we don't override any targetting - just listen to the base response of if the target can select us
            pOverrideCanBeSelectedBy = new Property<CanBeSelectedBy>((tar, selectionsSoFar, bCanSelectSoFar) => bCanSelectSoFar);
            pbCanSwapIn = new Property<bool>(() => position.positiontype == Position.POSITIONTYPE.BENCH);

            SetStateReadiness(new StateFatigued(this));

            soulContainer.Start();

            view = GetComponent<ViewChr>();
            view.Start();

        }

    }

}



//Add a max health initializer in each instance of a character - add an 
// initializer in the base chr that sets curhealth to max health