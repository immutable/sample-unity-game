using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using HyperCasual.Runner;

namespace HyperCasual.Gameplay
{
    /// <summary>
    /// The event is triggered when the player clicks on an asset in the inventory
    /// </summary>
    [CreateAssetMenu(fileName = nameof(AssetItemClickedEvent),
        menuName = "Runner/" + nameof(AssetItemClickedEvent))]
    public class AssetItemClickedEvent : GameEvent<AssetModel>
    {
    }
}