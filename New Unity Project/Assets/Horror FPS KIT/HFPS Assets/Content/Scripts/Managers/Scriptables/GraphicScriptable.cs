using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphicScriptable : ScriptableObject
{
    public enum AntialiasingEnum { Disabled, m_2x, m_4x, m_8x }
    public enum AnisotropicFilteringEnum { Disable, Enable, ForceEnable }
    public enum TextureQualityEnum { EightRes, QuarterRes, HalfRes, FullRes }
    public enum BlendWeightsEnum { OneBone, TwoBones, FourBones }
    public enum vSyncEnum { DontSync, EveryVBank, EverySecondVBank }
    public enum ShadowResolutionEnum { Low, Medium, High, VeryHigh }
    public enum ShadowQualityEnum { Disable, HardOnly, All }
    public enum ShadowProjectionEnum { CloseFit, StableFit }
    public enum ShadowCascadesEnum { NoCascades, TwoCascades, FourCascades }

    [Serializable]
    public class QualityLevels
    {
        public string QualityName;

        public AntialiasingEnum m_Antialiasing = AntialiasingEnum.m_8x;
        public AnisotropicFilteringEnum m_AnisotropicFiltering = AnisotropicFilteringEnum.ForceEnable;
        public TextureQualityEnum m_TextureQuality = TextureQualityEnum.FullRes;
        public BlendWeightsEnum m_BlendWeights = BlendWeightsEnum.FourBones;
        public vSyncEnum m_VSync = vSyncEnum.DontSync;
        public ShadowResolutionEnum m_ShadowResolution = ShadowResolutionEnum.VeryHigh;
        public ShadowQualityEnum m_ShadowQuality = ShadowQualityEnum.All;
        public ShadowProjectionEnum m_ShadowProjection = ShadowProjectionEnum.StableFit;
        public ShadowCascadesEnum m_ShadowCascades = ShadowCascadesEnum.NoCascades;
        [Range(0, 150)] public int m_ShadowDistance;
    }

    public List<QualityLevels> qualityLevels = new List<QualityLevels>();
}