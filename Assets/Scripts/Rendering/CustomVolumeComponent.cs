using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering
{
    [Serializable]
    public class CustomVolumeComponent : VolumeComponent
    {
        public BoolParameter enableInEditor =
            new BoolParameter(false);
        
        public BoolParameter enableAberration =
            new BoolParameter(false);
        public ClampedFloatParameter aberration =
            new ClampedFloatParameter(0.0f, 0, 0.01f);

        public BoolParameter enableOutline =
            new BoolParameter(true);
        public ClampedFloatParameter outline =
            new ClampedFloatParameter(0.0f, 0, 1.0f);
        public ColorParameter outlineColor =
            new ColorParameter(Color.white);
        public BoolParameter includeObject =
            new BoolParameter(true);
    }
}