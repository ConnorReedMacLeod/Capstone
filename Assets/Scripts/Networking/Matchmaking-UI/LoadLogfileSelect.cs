using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LoadLogfileSelect : MonoBehaviour {

    public Dropdown dropdownLogFileSelector;


    // Start is called before the first frame update
    void Start() {

        UpdateDropdownOptions();
    }

    public void UpdateDropdownOptions() {
        LogManager.Get().UpdateRecognizedLogFiles();

        List<string> lstLogFileNames = LogManager.Get().lstLogFiles.ConvertAll(fileinfo => fileinfo.Name);

        LibView.SetDropdownOptions(dropdownLogFileSelector, lstLogFileNames);
    }
    

    public void OnClickLoadLogFile() {

        int iSelected = dropdownLogFileSelector.value;

        Debug.LogFormat("Loading log file {0}: {1}", iSelected, LogManager.Get().lstLogFiles[iSelected].Name);

        LogManager.Get().LoadLoggedMatchSetup(LogManager.Get().lstLogFiles[iSelected]);
    }
}
