Overview:

As a modern take on classic JRPG combat, this game has two players each draft and customize a team of 5 characters to fight against one another in turn-based strategy combat.  The first player to defeat 3 of the opposing player's characters wins.  

Characters can be equipped with 4 skills from a pool of skills that are (mostly) unique to that character and that synergize with each other.  On each character's turn the player may spend mana to activate a skill which will perform some effect, and then fatigue the character for some number of turns until they are next able to act.  

Character skills can have a wide range of possible effects, with just a few common examples being:
- Dealing damage to one or more target characters
- Applying buffs/debuffs to characters
- Stunning characters so their turns are delayed
- Switching which characters are currently active in combat and which are reserved on the bench
- Changing what other skills a character was access to using

One of the goals of design of the game is to have a wide range of possible effects through which to craft interesting and flavourful skills for characters.  The system which processes these effects has been designed to be robust enough to handle complicated interactions between different skills to enable complex and engaging strategic moments for players.  



Technical Features:

Design Patterns:

Implementations for the Observer design pattern and the Singleton design pattern are provided and are heavily used throughout the project where appropriate
(Observer.cs, Subject.cs, Singleton.cs)

Ability to make arbitrary temporary modifications to values implemented through a functional Decorator Pattern. 
(/Model/Property.cs)

Player control abstracted to allow dynamic switching between local-human, scripted, remote-network, and various AI controllers. Loading a save will set both players' controllers to a scripted input (to re-simulate the match up until the save was created), and then dynamically switch the controllers back to their canonical types after the save file is fully loaded.  
(/SkillSelection/LocalInputType.cs, /Model/Player.cs)



Networking:

Starting up the game leads to a temporary lobby-finding scene where players can either do a solo match against AI for testing purposes, find a new match via a Photon matchmaking service, or load and resume a previously saved game.  Players can either progress through the standard drafting phase, or skip directly to a match by directly configuring which characters to use for each player.
(/Networking/NetworkConnectionManager.cs)

All networked events are specified by a sequential index that identifies the order in which those events are intended to be executed.  Incoming networked inputs are sorted into a buffer of inputs that defines the sequence of events that all players agree upon.  Execution of the game will automatically simulate and continue until it reaches a point where player-input is needed.  The buffer will then be checked to see if there has been a network-event that corresponds to the input that the game needs to continue processing.  If that input is not yet there, then execution will pause until the input has been received (and a turn-timer will start to limit how long a Player has to make the decisions for their turn).  This general system is used for all player inputs during a match, as well as for the drafting phase and the loadout-configuration phase.  
(/Networking/NetworkMatchReceiver.cs, /Networking/NetworkMatchSender.cs)

All randomization in the game is based on an agreed-upon seed provided by the host-Player when the match is started.  Since each player simulates the match in parallel based on what actions have been selected by each player, it is very important that any randomization is properly synchronized between all networked parties.  
(/Controller/ContRandomization.cs)



Game Rule Implementations/Back-end:

Characters have access to a number of Skillslots which hold the Skills they can use.  Game-Events can dynamically modify which Skills a Character has access to over the course of a match.  Skills are hidden from the opponent until they are used.
(/Model/SkillEngine/Skills/SkillSlot.cs)

Characters have their own individual cooldowns for when they are able to act, called Fatigue.  Using a Skill (and the effects of various Skills) can change a Characters Fatigue which determines the turn on which they will next act.  A priority list of when Characters will act is maintained to determine the relative order in which characters will act if they act on the same turn. 
(/Controller/ContTurns.cs) 

Character Skills fall into 4 different categories for how they can be used:
- Active Skills are used immediately
- Cantrip Skills are used immediately, and any number can be used without ending a Character's turn
- Passive Skills are not used directly, but instead provide constant benefits or support Triggers/Reactions to other effects
- Channel Skills have their effect delayed to happen a number of turns later and can allow for ongoing effects to happen for a sec duration
(/Model/SkillEngine/Skills/SkilTypes/TypeUsage*.cs)

Atomic Game-Events (Executables) support targetting of any component of the game (characters, players, buffs/debuffs, character positions, etc.)
(/Model/SkillEngine/Executables/Exec*.cs)

Many pre-constructed common Executables are available for quickly constructing new Game-Events
(/Model/SkillEngine/Executables/Exec*/Exec*.cs)

Game-Event Processing supports triggers that replace game actions with new ones if perscribed conditions are met. 
(/Model/SkillEngine/Replacement.cs)

Character Skills are composed of sequential steps that allow for modification and dynamic Triggers. For example, the mana cost for a Skill can be paid, and then Triggers from that mana-payment will be executed before the main effect of that Skill is executed.  
(/Model/SkillEngine/Skills/Skill.cs)

Multi-stage Game-Events can be flexibly constructed to interact with triggers in specifically desired ways through breaking them down into Executables (atomic changes to the gamestate) and Clauses (groups of Executables that will execute in sequence, and then allow any triggered Game-Events to be processed).  
For example, an Action like "Vampire Bite: Deal 10 damage to a Chr, and heal 10.  Then gain 10 Power" could be broken down into:
Clause 1 { Executable { Deal 10 damage }, Executable {Heal 10} },
Clause 2 { Executable { Gain 10 Power } }

This will ensure that the damage and healing events are tied together and will functionally happen at the same time, then allow for any triggers from that damage+healing to resolve, then will apply the 10 Power at the end.  This system allows Character Skills and Triggers to be constructed in whatever way makes the most narrative sense.
(/Model/SkillEngine/Executables/Executable.cs, /Model/SkillEngine/Clause.cs)

Character Skills have a list of Disciplines they require while Characters have a list of Disciplines they support.  This limits which Skills a Character can equip (and these can change dynamically over the course of a match).  For example, a knight Character could have a Sword discipline which allows them to use any Sword-based Skills.  But any Character could equip those same skills if they were given the Sword discipline during the match.  
(/Model/SkillEngine/Skills/SkillType.cs)

Both Characters and Positions can have modifications called Soul effects applied to them. Soul effects typically temporarily modify properties of the Character/Position they are applied to, but can also act as Triggers that cause a game-effect to happen when some preq-requisite occurs.  Characters can have 3 visible Soul effects at once (Positions can have 1), but can have any number of hidden Soul effects. Hidden Soul effects are typically used as sub-modifier components of a larger visible Soul effect, or as facilitators to make other effects possible.  For example:
A Skill could say "Applies [Retaliation](5) to this character.  [Retaliation]: Whenever this character takes damage, the source of that damage loses 5 Power for 2 turns."
Activating this skill would apply a visible Soul effect to the user called Retaliation for 5 turns.  This Soul effect would cause a Trigger to exist for as long as the Soul effect lasts on the character.  The trigger would wait for the character to take damage and then apply a hidden Soul effect to the damager for 2 turns that modifies their Power property for as long as the hidden Soul effect exists. 
(/Model/Soul/Soul.cs, /Model/Soul/SoulContainer.cs)

If a Character ever has a maximal amount of Soul effects (default is 3) and then another Soul effect is applied to them, then the oldest Soul effect on that character is removed before the new Soul effect is added.  Then, the Soulbreak effect is applied which double all damage that Character would both take and deal for 3 turns.  This itself is implemented as a hidden Soul effect.  
(/Model/Soul/SoulSoulbreak.cs, /Model/Soul/SoulContainer.cs)

Instances of Damage are customizable and dynamic, allowing for functional calculation of damage amounts, and dynamically calculated bonuses/reductions depending on the (flat/multiplicative) Power/Defense of the Damage Source/Target. 
(/Model/SkillEnginer/Executables/Damage.cs, Chrs/chr.cs)

Implements Timestamps to track the precise order in which Game-Events occur.  This, for example, allows us to later process Character deaths in the order they occured.  
(/Model/SkillEnginer/Timestamp.cs, /Controller/ContTimestamp.cs)

Supports flexible mana costs for skills (including wildcard costs, X-costs, and temporary cost increases/decreases) 
(/Model/Mana/ManaPool.cs, /Model/Mana/ManaCost.cs)

Periodically-repeating mana-generation and custom events can be added/removed/viewed on the 'Mana Calendar'
(/Model/Mana/ManaCalendar.cs, /Model/Mana/ManaDate.cs)

Dynamically configurably drafting sequence to allow players to ban, pick, and roster characters for their team. 
(/Drafting/DraftController.cs)

The process for requesting input from a player is abstracted to follow the same sequence of steps regardless of what type of input is requested (i.e., choosing character actions, choosing which character to switch-in, etc) 
(/Model/SkillEngine/MatchInputs/MatchInput.cs)

Player Inputs support auto-filling with randomized selections to assist with AI Player implementations.
(/Model/SkillEngine/MatchInputs/MatchInput.cs, /Model/SkillEngine/MatchInputs/InputSkillSelection.cs)



UI/UX/Front-end:

A custom system for detecting various types of mouse-interactions on objects was designed to quickly bind various interactions with objects with the intended outcomes.  Clicking, double-clicking, press-and-holding, and dragging an object can all be bound to different callback functions to make setting up new UIs quick and easy. 
(/View/ViewInteractive.cs)

There's many various panels of information about a match that a player should have access to, but that don't need to see on-screen at all points of a match. These panels are laid out in a physical-space around the scene such that the camera can do small shifts around the scene to display any information as it becomes pertinent.  The camera can be manually controlled by the player, but will also automatically shift to an area if that area is immediately important to how the game is executing (e.g., if the player has chosen to use a Skill that requires targetting a benched character, then the camera will pan over to the appropriate bench so they can make their selection).
(/View/CameraControllers.cs, /View/CameraControllerMatch.cs)

Custom key-bindings are supported through a collection of Keybind/Callback pairs.  There's currently no UI in place to allow players to use this system to create their own keybinds, but it's currently used to quickly create canonical keybinds.
(/Controller/Keybindings.cs)

Text-processing to support embedding special symbols in textboxes through custom fonts. 
(/Library/LibText.cs)

The mana costs of Skills typically contain some amount of specific colours of mana, and then some amount of 'Effort' mana that can be paid using any colour.  Players have a pool of Effort mana that they can dump various coloured mana into that will automatically be used to pay for any incurred Effort mana costs.  By default, 'Q', 'W', 'E', 'R' add Physical, Mental, Energy, and Blood mana to the mana pool while 'A', 'S', 'D', 'F' de-allocate those corresponding mana types.  
(/Model/Mana/ManaPool.cs)

A panel that displays the relative order of characters is provided to quickly tell which character will be acting next
(/View/ViewPriorityList.cs, /View/ViewPriorityHeadshot.cs)

A history panel is viewable to show a sequential ordering of all used Skills and the targets of those Skills
(/View/History/ViewHistoryPanel.cs, /View/History/ViewHistoryItemSkill.cs)

Audio Manager to randomly modify sound effects to prevent stale repetition. 
(/Controller/Audio.cs)



Serialization:

All player-inputs are serialized and recorded in a log file to aid in debugging and to enable loading partially-complete matches to resume them (in the case of a disconnect or for testing purposes)
(/Controller/LogManager.cs)

Supports serializiation/deserialization of custom Types to allow saving/loading and efficient networking communication 
(/Model/SkillEngine/Serializer.cs)

Supports saving and loading of custom skill-loadouts for each character. 
(/Loadouts/LoadoutManager.cs)


