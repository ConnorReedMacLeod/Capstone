using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChrCollection : Singleton<ChrCollection> {

    public List<Chr> lstChrs;

    public override void Init() {

        lstChrs = new List<Chr>();
    }


    public void AddChr(Chr chr) {

        chr.id = lstChrs.Count;
        lstChrs.Add(chr);

    }
    //Note there is no method to remove characters from this global list
    // Killed characters should just be flagged as such - some other affect may need them,
    // and we want to maintain ids


    // Common Query methods

    public Chr GetChr(int id) {
        return lstChrs[id];
    }

    public List<Chr> GetAllLiveAndDeadChrs() {
        return lstChrs;
    }

    public List<Chr> GetAllLiveChrs() {
        return GetChrs((Chr c) => c.bDead == false);
    }

    public List<Chr> GetAllDeadChrs() {
        return GetChrs((Chr c) => c.bDead == true);
    }

    // General Positional Queries



    public List<Chr> GetAllActiveChrs() {
        return GetChrs((Chr c) => (c.bDead == false) && (c.position.positiontype != Position.POSITIONTYPE.BENCH));
    }

    public List<Chr> GetAllFrontlineChrs() {
        return GetChrs((Chr c) => (c.bDead == false) && (c.position.positiontype == Position.POSITIONTYPE.FRONTLINE));
    }
    public List<Chr> GetAllBacklineChrs() {
        return GetChrs((Chr c) => (c.bDead == false) && (c.position.positiontype == Position.POSITIONTYPE.BACKLINE));
    }

    public List<Chr> GetAllBenchChrs() {
        return GetChrs((Chr c) => (c.bDead == false) && (c.position.positiontype == Position.POSITIONTYPE.BENCH));
    }


    // Querying for specific player-owned characters
    public List<Chr> GetAllChrsOwnedBy(Player plyr) {
        return GetChrs((Chr c) => plyr == c.plyrOwner);
    }

    public List<Chr> GetLiveChrsOwnedBy(Player plyr) {
        return GetChrs((Chr c) => (plyr == c.plyrOwner) && (c.bDead == false));
    }

    public List<Chr> GetDeadChrsOwnedBy(Player plyr) {
        return GetChrs((Chr c) => (plyr == c.plyrOwner) && (c.bDead == true));
    }

    public List<Chr> GetActiveChrsOwnedBy(Player plyr) {
        return GetChrs((Chr c) => (plyr == c.plyrOwner) && (c.bDead == false) && (c.position.positiontype != Position.POSITIONTYPE.BENCH));
    }

    public List<Chr> GetFrontlineChrsOwnedBy(Player plyr) {
        return GetChrs((Chr c) => (plyr == c.plyrOwner) && (c.bDead == false) && (c.position.positiontype == Position.POSITIONTYPE.FRONTLINE));
    }
    public List<Chr> GetBacklineChrsOwnedBy(Player plyr) {
        return GetChrs((Chr c) => (plyr == c.plyrOwner) && (c.bDead == false) && (c.position.positiontype == Position.POSITIONTYPE.BACKLINE));
    }

    public List<Chr> GetBenchChrsOwnedBy(Player plyr) {
        return GetChrs((Chr c) => (plyr == c.plyrOwner) && (c.bDead == false) && (c.position.positiontype == Position.POSITIONTYPE.BENCH));
    }


    // Base Query 
    public List<Chr> GetChrs(System.Func<Chr, bool> keepChr) {
        return lstChrs.Where(keepChr).ToList();
    }
}
