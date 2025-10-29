using UnityEngine;

public class TapTarget : MonoBehaviour
{
    [SerializeField] private string label = "red";       // "red" or "green"
    [SerializeField] private bool sendPerTap = true;     // Enable event on each tap
    [SerializeField] private bool accumulatePerSession = true; // Count for aggregated sending

    public void OnTapped()
    {
        Debug.Log($"Tapped on {label}");

        // Real-time: event on each tap
        if (sendPerTap)
            GameAnalyticsManager.Instance?.TrackTapRealtime(label);

        // Counter for aggregated sending
        if (accumulatePerSession)
            GameAnalyticsManager.Instance?.AccumulateTap(label);
    }
}
