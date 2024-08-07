using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayerStatBar : MonoBehaviour
{
    private Character currentCharacter;    
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;

    public bool isRecovering;
    private void Update()
    {
        // Set Delay fill effect
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime;
        }

        if(isRecovering )
        {
            double percentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = (float)percentage;
        }
    }


    /// <summary>
    /// 接收Health的变化百分比
    /// </summary>
    /// <param name="percentage">百分比计算：Current Health/ Max Health</param>
    public void OnHealthChange(float percentage)
    {
        healthImage.fillAmount = percentage;
    }

    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
}
