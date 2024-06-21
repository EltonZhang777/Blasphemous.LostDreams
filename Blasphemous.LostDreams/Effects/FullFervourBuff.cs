using Blasphemous.Framework.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gameplay.GameControllers.Penitent;
using Framework.Managers;
using Framework.FrameworkCore.Attributes.Logic;
using HutongGames.PlayMaker.Actions;
using Gameplay.GameControllers.Entities;

namespace Blasphemous.LostDreams.Effects;

internal class FullFervourBuff : ModItemEffectOnEquip
{
    private RawBonus _defenseBonus;
    private float _damageBonusValue;

    private RB507Config _config;
    private enum EffectState
    {
        Inactive,
        Active
    }
    private EffectState _currentEffectState;

    private bool _isFervourFull
    {
        get
        {
            return Core.Logic.Penitent.Stats.Fervour.Current
                >= 0.99f * Core.Logic.Penitent.Stats.Fervour.CurrentMax;
        }
    }
    
    public FullFervourBuff(RB507Config config)
    {
        _config = config;
        _defenseBonus = new(_config.defenseBonus);
        _damageBonusValue = _config.damageBonus;
    }

    
    protected override void Update()
    {
        ApplyEffect();
        if (!_isFervourFull && (_currentEffectState == EffectState.Active)) { RemoveEffect(); }
    }

    protected override void ApplyEffect()
    {
        if (_isFervourFull && (_currentEffectState == EffectState.Inactive)) 
        {
            Core.Logic.Penitent.Stats.ContactDmgReduction.AddRawBonus(_defenseBonus);
            Core.Logic.Penitent.Stats.NormalDmgReduction.AddRawBonus(_defenseBonus);
            Core.Logic.Penitent.Stats.FireDmgReduction.AddRawBonus(_defenseBonus);
            Core.Logic.Penitent.Stats.MagicDmgReduction.AddRawBonus(_defenseBonus);
            Core.Logic.Penitent.Stats.LightningDmgReduction.AddRawBonus(_defenseBonus);
            Core.Logic.Penitent.Stats.ToxicDmgReduction.AddRawBonus(_defenseBonus);

            Main.LostDreams.EventHandler.OnEnemyDamaged += IncreaseDamage;

            _currentEffectState = EffectState.Active;
        }
    }

    protected override void RemoveEffect()
    {
        Core.Logic.Penitent.Stats.ContactDmgReduction.RemoveRawBonus(_defenseBonus);
        Core.Logic.Penitent.Stats.NormalDmgReduction.RemoveRawBonus(_defenseBonus);
        Core.Logic.Penitent.Stats.FireDmgReduction.RemoveRawBonus(_defenseBonus);
        Core.Logic.Penitent.Stats.MagicDmgReduction.RemoveRawBonus(_defenseBonus);
        Core.Logic.Penitent.Stats.LightningDmgReduction.RemoveRawBonus(_defenseBonus);
        Core.Logic.Penitent.Stats.ToxicDmgReduction.RemoveRawBonus(_defenseBonus);

        Main.LostDreams.EventHandler.OnEnemyDamaged -= IncreaseDamage;

        _currentEffectState = EffectState.Inactive;
    }

    private void IncreaseDamage(ref Hit hit)
    {
        hit.DamageAmount *= (1f + _damageBonusValue);
    }
}

/// <summary>
/// config for Unstained Jewel (RB507)
/// </summary>
public class RB507Config
{
    public float damageBonus = 0.1f;

    public float defenseBonus = 0.1f;

    public RB507Config() { }
}