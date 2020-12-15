// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Screen code
    /// </summary>
    [AddComponentMenu("Magic Leap/Raycast/Combo Raycast Head")]
    public class screenScript : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("The camera attached to the screen.")]
        private Camera screenCamera;

        // Note: Generated mesh may include noise (bumps). This bias is meant to cover
        // the possible deltas between that and the perception stack results.
        private const float _bias = 0.04f;
        #endregion
    }
}
