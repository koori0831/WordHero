using LitMotion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;

namespace Work.ETC.LocationUI.Code
{
    public record struct OnShowLocationNameEvent(string LocationName) : IEvent;

    public class LocationNameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI locationNameText;
        [SerializeField] private Image locationNameLine;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float displayDuration = 2.5f;

        public void Awake()
        {
            Bus<OnShowLocationNameEvent>.Events += HandleShowLocationNameEvent;
            locationNameText.gameObject.SetActive(false);
            locationNameLine.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<OnShowLocationNameEvent>.Events -= HandleShowLocationNameEvent;
        }

        private void HandleShowLocationNameEvent(OnShowLocationNameEvent evt)
        {
            ShowLocationName(evt.LocationName);
        }

        public void ShowLocationName(string locationName)
        {
            locationNameText.text = locationName;
            locationNameText.gameObject.SetActive(true);
            locationNameLine.gameObject.SetActive(true);

            LMotion.Create(0f, 1f, fadeDuration)
                .WithEase(Ease.InCubic)
                .WithOnComplete(() =>
                {
                    LMotion.Create(1f, 0f, fadeDuration)
                        .WithEase(Ease.OutCubic)
                        .WithDelay(displayDuration)
                        .WithOnComplete(() =>
                        {
                            locationNameText.gameObject.SetActive(true);
                            locationNameLine.gameObject.SetActive(true);
                        })
                        .Bind((x) =>
                        {
                            Color textColor = locationNameText.color;
                            textColor.a = x;
                            locationNameText.color = textColor;
                            Color lineColor = locationNameLine.color;
                            lineColor.a = x;
                            locationNameLine.color = lineColor;
                        });
                }
                )
                .Bind((x) =>
                {
                    Color textColor = locationNameText.color;
                    textColor.a = x;
                    locationNameText.color = textColor;
                    Color lineColor = locationNameLine.color;
                    lineColor.a = x;
                    locationNameLine.color = lineColor;
                });


        }
    }
}