using TMPro;
using UnityEngine;
using Spellbound.Core;

namespace Spellbound.UI
{
    /// <summary>
    /// Shows the "3, 2, 1, GO!" countdown text driven by GameManager.OnCountdownTick.
    /// </summary>
    public class CountdownUI : MonoBehaviour
    {
        public GameManager gameManager;
        public TMP_Text countdownText;

        void OnEnable() => gameManager.OnCountdownTick += HandleTick;
        void OnDisable() { if (gameManager != null) gameManager.OnCountdownTick -= HandleTick; }

        void HandleTick(int value)
        {
            countdownText.text = value > 0 ? value.ToString() : "GO!";
        }
    }
}