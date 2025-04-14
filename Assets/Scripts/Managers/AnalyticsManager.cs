using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Managers
{
    public class AnalyticsManager : MonoBehaviour
    {
        private void Start()
        {
            UnityServices.Instance.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }
    }
}
