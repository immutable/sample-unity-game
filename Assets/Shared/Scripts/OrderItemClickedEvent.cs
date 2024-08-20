using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using HyperCasual.Runner;

namespace HyperCasual.Gameplay
{
    /// <summary>
    /// The event is triggered when the player clicks on an order in the marketplace
    /// </summary>
    [CreateAssetMenu(fileName = nameof(OrderItemClickedEvent),
        menuName = "Runner/" + nameof(OrderItemClickedEvent))]
    public class OrderItemClickedEvent : GameEvent<OrderModel>
    {
    }
}