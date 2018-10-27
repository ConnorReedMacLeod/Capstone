using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    bool bStarted;

    [SyncVar (hook = "OnSetID")]
    public int id;

    public static int idLocal;

    public static int MAXCHRS = 3;
    public static int MAXPLAYERS = 2;
    public Chr[] arChr;
    public int nChrs = 3;

    public GameObject pfManaPanel;

    public Mana mana;

    public void OnSetID(int id) {
        //Make sure the match knows that we're the player with this id
        Debug.Log("OnSetID called");
        Match.Get().arPlayers[id] = this;

        //Once we've gotten our id, we can claim our characters and position them
        for (int i = 0; i < MAXCHRS; i++) {
            Match.Get().arChrs[id, i].SetOwner(this);
        }
    }

    public void SetID(int _id) {
        Debug.Log("In SetID and are we the server? " + isServer);
        id = _id;
        Match.Get().arPlayers[id] = this;
        Debug.Log("And our id is now " + id);

        //Once we've gotten our id, we can claim our characters and position them
        for (int i = 0; i < MAXCHRS; i++) {
            Match.Get().arChrs[id, i].SetOwner(this);
        }

    }

    [Command]
    public void CmdInitId() {
        //This will be called by the local player, then executed on the server
        if (!isServer) {
            Debug.Log("CmdInitId was somehow called on a client");
            return;
        }

        Match.Get().RequestPlayerID(this);
    }


    public override void OnStartLocalPlayer() {
        Debug.Log("Going to call CmdInitId since we're the local player");
        CmdInitId();
        idLocal = id;
        Debug.Log("Finished calling CmdInitId");

        

    }

    // Use this for initialization
    public void Start () {

		if (bStarted == false) {
			bStarted = true;

            arChr = new Chr[MAXCHRS];

			GameObject manaPanel = Instantiate(pfManaPanel, Match.Get().transform);
			mana = manaPanel.GetComponent<Mana>();

			mana.SetPlayer (this);

			//TODO: Change this, all this, too work with networking
			if (id == 0) {
				manaPanel.transform.position = new Vector3(12.75f, -5.3f, -0.4f);
			} else {
				//move it offscreen for now
				manaPanel.transform.position = new Vector3(-20.0f, -3.0f, -0.4f);
			}
		}
	}

}
