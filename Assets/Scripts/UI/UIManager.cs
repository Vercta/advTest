using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public PlayerStatBar playerStatBar;

    [Header("事件监听")]
    public CharacterEventSO healthEvent;

    #region 注册事件
    // 可以多个代码订阅同一事件
    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
    }

    #endregion
    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = (character.currentHealth / character.maxHealth);
        playerStatBar.OnHealthChange((float)percentage);
    }



}
