using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TinyUFramework
{
    public class AudioControlUI : MonoBehaviour
    {
        [SerializeField] GameObject fxSliderObj;
        [SerializeField] GameObject musicSliderObj;
        [SerializeField] GameObject btnObj;
        [SerializeField] GameObject fxTextObj;
        [SerializeField] GameObject musicTextObj;

        Slider fxSlider;
        Slider musicSlider;
        Button btn;

        bool isPanelOn = false;

        private void Awake()
        {
            fxTextObj.SetActive(false);
            musicTextObj.SetActive(false);
            fxSliderObj.SetActive(false);
            musicSliderObj.SetActive(false);

            fxSlider = fxSliderObj.GetComponent<Slider>();
            musicSlider = musicSliderObj.GetComponent<Slider>();
            btn = btnObj.GetComponent<Button>();
        }

        private void OnEnable()
        {
            BindElements();
        }

        private void OnDisable()
        {
            UnBindElements();
        }



        void BindElements()
        {
            btn.onClick.AddListener(() =>
            {
                if (!isPanelOn)
                {
                    isPanelOn = true;
                    fxTextObj.SetActive(true);
                    musicTextObj.SetActive(true);
                    fxSliderObj.SetActive(true);
                    musicSliderObj.SetActive(true);
                }
                else
                {
                    isPanelOn = false;
                    fxTextObj.SetActive(false);
                    musicTextObj.SetActive(false);
                    fxSliderObj.SetActive(false);
                    musicSliderObj.SetActive(false);
                }
            });
            fxSlider.value = SoundLoaderManager.Instance.GetFxVolume();
            musicSlider.value = SoundLoaderManager.Instance.GetMusicVolume();
            fxSlider.onValueChanged.AddListener(FXValueChangeHandler);
            musicSlider.onValueChanged.AddListener(MusicValueChangeHandler);
        }

        void UnBindElements()
        {
            isPanelOn = false;
            fxTextObj.SetActive(false);
            musicTextObj.SetActive(false);
            fxSliderObj.SetActive(false);
            musicSliderObj.SetActive(false);
            btn.onClick.RemoveAllListeners();
            fxSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.RemoveAllListeners();
        }

        void FXValueChangeHandler(float value)
        {
            SoundLoaderManager.Instance.SetFXVolume(value);
        }

        void MusicValueChangeHandler(float value)
        {
            SoundLoaderManager.Instance.SetMusicVolume(value);
        }
    }
}
