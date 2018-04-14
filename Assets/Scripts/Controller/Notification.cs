using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Provides standard types of events to be sent from
//View objects to controller
public class Notification {

	public const string Default = "default";

	//Character/Action Events
	public const string ClickChr = "click.chr";
	public const string ClickAct = "click.act";
	public const string ClickArena = "click.arena";

	//Mana Events
	public const string ManaAddPhysical = "mana.addphys";
	public const string ManaAddAllPhysical = "mana.addallphys";
	public const string ManaRemovePhysical = "mana.remphys";
	public const string ManaRemoveAllPhysical = "mana.remallphys";

	public const string ManaAddMental = "mana.addmen";
	public const string ManaAddAllMental = "mana.addallmen";
	public const string ManaRemoveMental = "mana.remmen";
	public const string ManaRemoveAllMental = "mana.remallmen";

	public const string ManaAddEnergy = "mana.addenergy";
	public const string ManaAddAllEnergy = "mana.addallenergy";
	public const string ManaRemoveEnergy = "mana.remenergy";
	public const string ManaRemoveAllEnergy = "mana.remallenergy";

	public const string ManaAddBlood = "mana.addbld";
	public const string ManaAddAllBlood = "mana.addallbld";
	public const string ManaRemoveBlood = "mana.rembld";
	public const string ManaRemoveAllBlood = "mana.remallbld";

	public const string ManaAddAll = "mana.addall";
	public const string ManaRemoveAll = "mana.remall";

	public const string ManaChange = "mana.change";
	public const string ManaPoolChange = "mana.poolchange";

	//TimelineEvents
	public const string EventFinish = "event.finish";
	public const string EventMoved = "event.moved";
	public const string EventChangedState = "event.changedstate";

	public const string EventSetMana = "event.setmana";

	public const string ExecuteEvent = "event.execute";

}
