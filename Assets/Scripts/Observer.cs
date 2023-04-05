using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class that can track a set of subjects that are being observed, and can cancel
//  all of these observations when the set of observations is no longer needed
public class Observer {

    public struct ObservingInfo {
        public Subject subbedTo;
        public Subject.FnCallback fnCallback;

        public ObservingInfo(Subject _subbedTo, Subject.FnCallback _fnCallback) {
            subbedTo = _subbedTo;
            fnCallback = _fnCallback;
        }

        public bool IsEqual(Subject _subbedTo, Subject.FnCallback _fnCallback) {
            return subbedTo == _subbedTo && fnCallback == _fnCallback;
        }

        public void EndObservation() {
            subbedTo.UnSubscribe(fnCallback);
        }
    }

    public List<ObservingInfo> lstObserving;

    public Observer() {
        lstObserving = new List<ObservingInfo>();
    }


    public void Observe(Subject sub, Subject.FnCallback fnCallback) {
        sub.Subscribe(fnCallback);
        lstObserving.Add(new ObservingInfo(sub, fnCallback));
    }

    public void StopObserving(Subject sub, Subject.FnCallback fnCallback) {

        for(int i = 0; i < lstObserving.Count; i++) {
            if(lstObserving[i].IsEqual(sub, fnCallback)) {
                sub.UnSubscribe(fnCallback);
                lstObserving.RemoveAt(i);
                return;
            }
        }

        Debug.LogError("Error! Tried to stop observing " + sub + " with callback " + fnCallback + " but " + this + " wasn't subscribed to it");

    }

    public void EndAllObservations() {
        for(int i = 0; i < lstObserving.Count; i++) {
            lstObserving[i].EndObservation();
        }
        lstObserving.Clear();
    }

}
