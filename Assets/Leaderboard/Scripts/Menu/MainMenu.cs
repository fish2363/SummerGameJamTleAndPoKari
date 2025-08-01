using Leaderboard.Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboard.Scripts.Menu
{
    public class MainMenu : Panel
    {
        [SerializeField] private Button leaderboardsButton = null;
    
        public override void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            leaderboardsButton.onClick.AddListener(Leaderboards);
            base.Initialize();
        }
    
        private void Leaderboards()
        {
            PanelManager.Open("leaderboards");
        }
    }
}