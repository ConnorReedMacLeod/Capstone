using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerMatch : CameraController {


    //Note - I don't like how coupled this is - would be nice to find a way to 'automate' this process
    //   without having to ensure the name written here matches the one given in the inspector
    public void cbSetLocationHome(Object tar, params object[] args) {
        SetTargetLocation("Home");
    }

    public void cbSetLocationBenchLeft(Object tar, params object[] args) {
        SetTargetLocation("BenchLeft");
    }

    public void cbSetLocationBenchRight(Object tar, params object[] args) {
        SetTargetLocation("BenchRight");
    }

    public void cbSetLocationZoomedOut(Object tar, params object[] args) {
        SetTargetLocation("ZoomedOut");
    }

    public void cbSetLocationManaCalendarLeft(Object tar, params object[] args) {
        SetTargetLocation("ManaCalendarLeft");
    }

    public void cbSetLocationManaCalendarRight(Object tar, params object[] args) {
        SetTargetLocation("ManaCalendarRight");
    }


    public override void Start() {
        base.Start();

        //Save each hotkey one-by-one - I don't love this, but this should be fine to extend once we have a
        //   more refined saved-hotkeys utility
        KeyBindings.SetBinding(cbSetLocationHome, dictSavedCameraHotkeys["Home"]);
        KeyBindings.SetBinding(cbSetLocationBenchLeft, dictSavedCameraHotkeys["BenchLeft"]);
        KeyBindings.SetBinding(cbSetLocationBenchRight, dictSavedCameraHotkeys["BenchRight"]);
        KeyBindings.SetBinding(cbSetLocationZoomedOut, dictSavedCameraHotkeys["ZoomedOut"]);
        KeyBindings.SetBinding(cbSetLocationManaCalendarLeft, dictSavedCameraHotkeys["ManaCalendarLeft"]);
        KeyBindings.SetBinding(cbSetLocationManaCalendarRight, dictSavedCameraHotkeys["ManaCalendarRight"]);

    }
}
