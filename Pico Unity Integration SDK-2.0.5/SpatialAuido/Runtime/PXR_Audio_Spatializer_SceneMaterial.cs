//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using UnityEngine;

public class PXR_Audio_Spatializer_SceneMaterial : MonoBehaviour
{
    [SerializeField] 
    public PXR_Audio.Spatializer.AcousticsMaterial materialPreset = PXR_Audio.Spatializer.AcousticsMaterial.AcousticTile;
    private PXR_Audio.Spatializer.AcousticsMaterial lastMaterialPreset = PXR_Audio.Spatializer.AcousticsMaterial.AcousticTile;
    
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float[] absorption = new float[4];
    
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float scattering = 0.0f;
    
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float transmission = 0.0f;

    private void OnValidate()
    {
        if (lastMaterialPreset != materialPreset) // material_preset is changed
        {
            if (materialPreset != PXR_Audio.Spatializer.AcousticsMaterial.Custom)
            {
                PXR_Audio.Spatializer.Api.GetAbsorptionFactor(materialPreset, absorption);
                PXR_Audio.Spatializer.Api.GetScatteringFactor(materialPreset, ref scattering);
                PXR_Audio.Spatializer.Api.GetTransmissionFactor(materialPreset, ref transmission);
            }

            lastMaterialPreset = materialPreset;
        }
        else if (materialPreset != PXR_Audio.Spatializer.AcousticsMaterial.Custom) // material_preset is not changed, but acoustic properties are changed manually
        {
            materialPreset = PXR_Audio.Spatializer.AcousticsMaterial.Custom;
            lastMaterialPreset = PXR_Audio.Spatializer.AcousticsMaterial.Custom;
        }
    }
}
