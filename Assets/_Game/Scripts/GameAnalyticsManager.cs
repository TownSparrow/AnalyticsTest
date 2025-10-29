using UnityEngine;
using GameAnalyticsSDK;
using System.Collections.Generic;

public class GameAnalyticsManager : MonoBehaviour, IGameAnalyticsATTListener
{
    public static GameAnalyticsManager Instance { get; private set; }

    // Counters for button taps (red/green)
    private readonly Dictionary<string, int> _tapCounters = new Dictionary<string, int>(4);
    private bool _initialized;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        GameAnalytics.RequestTrackingAuthorization(this);
        #else
        InitializeGA();
        #endif
    }


    private void InitializeGA()
    {
        // Logs in development builds:
        // NOTE: GameAnalytics.SetEnabledInfoLog and SetEnabledVerboseLog have been removed or renamed in newer SDKs.
        // To enable logging, use GameAnalytics.SetEnabledEventSubmission or review new SDK docs as needed.
        // The following lines are commented out to prevent compile errors:
        /*
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        GameAnalytics.SetEnabledInfoLog(true);
        GameAnalytics.SetEnabledVerboseLog(true);
#endif
        */
        GameAnalytics.Initialize();
        _initialized = true;
    }

    // ATT callbacks (iOS)
    public void GameAnalyticsATTListenerNotDetermined() => InitializeGA();
    public void GameAnalyticsATTListenerRestricted()    => InitializeGA();
    public void GameAnalyticsATTListenerDenied()        => InitializeGA();
    public void GameAnalyticsATTListenerAuthorized()    => InitializeGA();

    // Real-time: sending an event about a tap
    public void TrackTapRealtime(string label)
    {
        if (!_initialized) return; // avoid sending before initialization
        GameAnalytics.NewDesignEvent($"ui:button:tap:{label}");
    }

    // Increment counter for aggregated sending
    public void AccumulateTap(string label)
    {
        if (!_tapCounters.TryGetValue(label, out var count))
            count = 0;
        _tapCounters[label] = count + 1;
    }

    // Sending aggregated counters when exiting/collapsing
    private void OnApplicationQuit() => FlushTapCounters();
    private void OnApplicationPause(bool paused)
    {
        if (paused) FlushTapCounters();
    }

    private void FlushTapCounters()
    {
        if (!_initialized) return;
        foreach (var kv in _tapCounters)
        {
            // value = number of taps on the button of this color during the session
            GameAnalytics.NewDesignEvent($"ui:button:tap:{kv.Key}", kv.Value);
        }
        _tapCounters.Clear();
    }
}
