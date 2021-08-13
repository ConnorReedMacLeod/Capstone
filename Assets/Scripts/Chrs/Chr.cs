using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ViewChr))]
public class Chr : MonoBehaviour {

    bool bStarted;

    public enum CHARTYPE {          //CHARTYPE's possible values include all characters in the game
        FISCHER, KATARINA, PITBEAST, RAYNE, SAIKO, SOHPIDIA, LENGTH
    };
    public static readonly string[] ARSCHRNAMES = { "Fischer", "Katarina", "PitBeast", "Rayne", "Saiko", "Sophidia", "None" };

    public enum STATESELECT {
        SELECTED,                   //Selected a character (to see status effects, skills)
        TARGGETING,                 //Targetting of character skills
        IDLE                        //Default character state
    };

    public enum SIZE {
        SMALL,
        MEDIUM,
        LARGE,
        GIANT
    };

    public string sName;            //The name of the character
    public Player plyrOwner;        //The player who controls the character

    public static List<Chr> lstChrInPlay; //A static list of the characters in play (not on the bench)
    public static List<Chr> lstAllChrs;  //A static list of all characters

    public int globalid;            //The character's unique identifier across all characters
    public int id;                  //The character's unique identifier for this team
    public int nFatigue;            //Number of turns a character must wait before their next skill
    public StateReadiness curStateReadiness; //A reference to the current state of readiness

    public int nMaxSkillsLeft;     //The total maximum number of skills a character can use in a turn (usually 1, cantrips cost 0)

    public int nCurHealth;          //The character's current health
    public Property<int> pnMaxHealth;          //The character's max health

    public List<Discipline.DISCIPLINE> lstDisciplines; //The disciplines the character has access to

    public bool bDead;                         //If the character is dead or not

    public Property<int> pnPower;              //The character's current power
    public Property<int> pnDefense;            //The character's current defense

    public Property<int> pnArmour;          //The character's current armour
    public int nAbsorbedArmour;             //The amount of damage currently taken by armour

    public SkillSlot[] arSkillSlots;      //The slots for the character's currently usable skills - these keep track of the cooldowns of those skills
    public const int nStandardCharacterSkills = 4; //Number of non-generic (non-rest) skills currently active on the character
    public const int nTotalSkills = nStandardCharacterSkills + 2; //Number of all skills (including generics)
    public SkillRest skillRest;  //The standard reference to the rest skill the character can use
    public const int nRestSlotId = nStandardCharacterSkills; //Id of the skillslot containing the rest skill
    public SkillType.SKILLTYPE[] arSkillTypesOpeningLoadout;  //Holds the initially selected loadout of skills for the character - may shift this to some loadout manager

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
    public Subject subChannelTimeChange = new Subject();

    public Subject subLeftPosition = new Subject();
    public Subject subEnteredPosition = new Subject();

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

    public int GetTargettingId() {
        return globalid;
    }

    public static Chr GetTargetByIndex(int ind) {
        return lstAllChrs[ind];
    }

    public static Chr GetRandomChr() {
        return lstChrInPlay[Random.Range(0, lstChrInPlay.Count)];
    }

    public Skill GetRandomActiveSkill() {
        return arSkillSlots[Random.Range(0, nStandardCharacterSkills)].skill;
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

    public static void RegisterChr(Chr chr) {
        if(lstAllChrs == null) {
            lstAllChrs = new List<Chr>(Player.MAXPLAYERS * Player.MAXCHRS);
        }

        lstAllChrs.Insert(chr.globalid, chr);

        //TODO:: do something more sophisticated for this once the bench is added
        if(lstChrInPlay == null) {
            lstChrInPlay = new List<Chr>(Player.MAXPLAYERS * Player.MAXCHRS);
        }

        lstChrInPlay.Add(chr);


    }


    public void ChangeChanneltime(int _nChange) {
        //Just let our readiness state deal with this
        curStateReadiness.ChangeChanneltime(_nChange);

        subChannelTimeChange.NotifyObs();
    }


    public void KillCharacter() {
        if(bDead) {
            Debug.Log("Trying to kill a character thast's already dead");
            return;
        }

        //interrupt any channel that  we may be using 
        curStateReadiness.InterruptChannel();

        //Create a new death state to let our character transition to
        StateDead newState = new StateDead(this);

        //Transition to the new state
        SetStateReadiness(newState);
    }


    // Apply this amount of fatigue to the character
    public void ChangeFatigue(int _nChange, bool bBeginningTurn = false) {
        if(_nChange + nFatigue < 0) {
            nFatigue = 0;
        } else {
            nFatigue += _nChange;
        }

        subFatigueChange.NotifyObs(this);
        subAllFatigueChange.NotifyObs(this);

        //TODO:: Probably delete this bBeginningTurn flag once I get a nice solution for priority handling
        if(!bBeginningTurn) {
            //Then this is a stun or an skills used
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
        for(int i = 0; i < Chr.nStandardCharacterSkills; i++) {

            //Only reduce the cooldown if it is not currently off cooldown
            if(arSkillSlots[i].nCooldown > 0) {
                ContSkillEngine.Get().AddExec(new ExecChangeCooldown(null, arSkillSlots[i].skill, -1) {

                    fDelay = ContTurns.fDelayMinorSkill
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
        if(nCurHealth + nChange > pnMaxHealth.Get()) {
            nCurHealth = pnMaxHealth.Get();
        } else if(nCurHealth + nChange <= 0) {
            nCurHealth = 0;

            KillCharacter();
        } else {
            nCurHealth += nChange;
        }

        subLifeChange.NotifyObs(this, nChange);
    }

    public void SetPosition(Position _position) {
        if(position == _position) return;

        position = _position;
    }

    //Counts down the character's recharge with the timeline
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
        ChangeState(STATESELECT.IDLE);

        subStartIdle.NotifyObs(this);
        subAllStartIdle.NotifyObs(this);
    }

    //Performs the consumed skill 
    public void ExecuteSkill(Selections selections) {

        if(selections.IsGoodEnoughToExecute() == false) {
            Debug.LogError("ERROR! This skill was targetted, but is no longer valid to be executed");
            selections.ResetToRestSelection();
        }

        //Notify everyone that we're about to use this skill
        subBeforeActivatingSkill.NotifyObs(this, selections);
        subAllBeforeActivatingSkill.NotifyObs(this, selections);

        //Actually use the skill
        selections.skillSelected.UseSkill(selections);

    }


    // Used to initiallize information fields of the Chr
    // Call this after creating to set information
    public void InitChr(Player _plyrOwner, int _id, BaseChr baseChr) {
        plyrOwner = _plyrOwner;
        id = _id;
        globalid = id + plyrOwner.id * Player.MAXCHRS;

        RegisterChr(this);

        baseChr.Init();

        view.Init();
    }

    public void InitGenericSkills() {

    }

    public void InitSkillSlots() {

        arSkillSlots = new SkillSlot[nTotalSkills];

        for(int i = 0; i < nTotalSkills; i++) {
            arSkillSlots[i] = new SkillSlot(this, i);
        }


        skillRest = new SkillRest(this);

        arSkillSlots[nRestSlotId].SetSkill(skillRest);
    }

    // Sets up fundamental class connections for the Chr
    public void Start() {
        if(bStarted == false) {
            bStarted = true;

            nMaxSkillsLeft = 1;

            InitSkillSlots();

            stateSelect = STATESELECT.IDLE;

            pnMaxHealth = new Property<int>(100);
            pnArmour = new Property<int>(0);

            pnPower = new Property<int>(0);
            pnDefense = new Property<int>(0);

            SetStateReadiness(new StateFatigued(this));

            view = GetComponent<ViewChr>();
            view.Start();

        }

    }

}



//Add a max health initializer in each instance of a character - add an 
// initializer in the base chr that sets curhealth to max health