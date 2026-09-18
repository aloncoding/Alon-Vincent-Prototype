using UnityEngine;
using Spellbound.Lanes;

namespace Spellbound.UI
{
    /// <summary>
    /// Visually highlights whichever lane LaneManager currently has targeted
    /// (a glowing rune, highlighted lane background, etc.).
    /// </summary>
    public class TargetIndicatorUI : MonoBehaviour
    {
        public LaneManager laneManager;
        [Tooltip("One highlight GameObject per lane, in the same order as LaneManager.lanes.")]
        public GameObject[] laneHighlights;

        void OnEnable() => laneManager.OnLaneChanged += SetActiveLane;
        void OnDisable() { if (laneManager != null) laneManager.OnLaneChanged -= SetActiveLane; }

        void Start() => SetActiveLane(laneManager.CurrentLaneIndex);

        void SetActiveLane(int index)
        {
            for (int i = 0; i < laneHighlights.Length; i++)
                laneHighlights[i].SetActive(i == index);
        }
    }
}
