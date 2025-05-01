using System;
using UnityEngine.Rendering;

namespace Rendering
{
    [Serializable]
    public class CustomVolumeComponent : VolumeComponent
    {
        public BoolParameter enableAberration =
            new BoolParameter(false);
        public ClampedFloatParameter aberration =
            new ClampedFloatParameter(0.0f, 0, 0.01f);
    }
}