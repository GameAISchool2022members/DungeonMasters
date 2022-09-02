using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider gammaSlider;
    private ColorAdjustments colorAdjustments;
    public Volume Brightness;
    private VolumeParameter<float> postvalue = new VolumeParameter<float>();
    // Start is called before the first frame update
    void Start()
    {
        Brightness.sharedProfile.TryGet<ColorAdjustments>(out colorAdjustments);
      

        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
        if (!PlayerPrefs.HasKey("Brightness"))
        {
            PlayerPrefs.SetFloat("Brightness", 1);
            Load();
        }
        else
        {
            Load();
        }
    }
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }
    public void ChangeGamma()
    {
        postvalue.value = gammaSlider.value;
        colorAdjustments.postExposure.value = postvalue.value;
        colorAdjustments.postExposure.SetValue(postvalue);
        Save();
    }
    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        gammaSlider.value = PlayerPrefs.GetFloat("Brightness");
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("SoundVolume", volumeSlider.value);
        PlayerPrefs.SetFloat("Brightnesss", gammaSlider.value);
    }
    // Update is called once per frame
  
}
