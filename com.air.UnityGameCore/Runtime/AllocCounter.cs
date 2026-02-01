using System;
using UnityEngine.Profiling;

// See UnityEngine.TestTools.Constraints.AllocatingGCMemoryConstraint
// and https://maingauche.games/devlog/20230504-counting-allocations-in-unity/
public class AllocCounter {
    private Recorder _rec;

    public AllocCounter() {
        _rec = Recorder.Get("GC.Alloc");
        _rec.enabled = false;

#if !UNITY_WEBGL
        _rec.FilterToCurrentThread();
#endif

        _rec.enabled = true;
    }

    public int Stop() {
        if (_rec == null) throw new InvalidOperationException("AllocCounter was not started.");

        _rec.enabled = false;

#if !UNITY_WEBGL
        _rec.CollectFromAllThreads();
#endif

        int result = _rec.sampleBlockCount;
        _rec = null;
        return result;
    }

    public static int Instrument(Action action) {
        var counter = new AllocCounter();
        int allocs;

        try {
            action();
        }
        finally {
            allocs = counter.Stop();
        }

        return allocs;
    }
}