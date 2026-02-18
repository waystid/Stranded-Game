using UnityEngine;
using MoreMountains.Tools;

namespace CosmicColony
{
    [AddComponentMenu("Cosmic Colony/UI/Portal Button")]
    public class PortalButton : MonoBehaviour
    {
        [Tooltip("The exact scene name to load when this button is pressed")]
        public string SceneName;

        public void GoToScene()
        {
            if (string.IsNullOrEmpty(SceneName))
            {
                Debug.LogWarning("[PortalButton] SceneName is empty!");
                return;
            }
            MMSceneLoadingManager.LoadScene(SceneName);
        }
    }
}
