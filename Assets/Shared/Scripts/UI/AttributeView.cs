using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace HyperCasual.Runner
{
    /// <summary>
    /// Represents an asset's attribute
    /// </summary>
    public class AttributeView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Type = null;
        [SerializeField] private TextMeshProUGUI m_Value = null;

        public async void Initialise(AssetAttribute attribute)
        {
            m_Type.text = attribute.trait_type;
            m_Value.text = attribute.value;
        }

    }
}