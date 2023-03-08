﻿using System.Collections.Generic;

using UnityEngine;

using Player;

namespace Mechanics
{
    public class PlayerSpikeResponse : MonoBehaviour, ISpikeResponse
    {
        private HashSet<Spike> _dogoDisabledSpikes = new HashSet<Spike>();

        private void Start()
        {
            PlayerCore.StateMachine.OnPlayerRespawn += RechargeSpikes;
            PlayerCore.StateMachine.StateTransition += OnPlayerStateChanged;
        }

        private void OnDisable()
        {
            PlayerCore.StateMachine.OnPlayerRespawn -= RechargeSpikes;
            PlayerCore.StateMachine.StateTransition -= OnPlayerStateChanged;
        }

        public void OnSpikeEnter(Spike spike)
        {
            if (PlayerCore.StateMachine.UsingDrill)
            {
                spike.Discharge(_dogoDisabledSpikes);
            }
            else if (spike.Charged)
            {
                bool shouldDie = true;
                var directionalSpike = spike as DirectionalSpike;
                if (directionalSpike != null)
                {
                    shouldDie = directionalSpike.ShouldDieFromVelocity(PlayerCore.Actor.velocity);
                }
                
                if (shouldDie) PlayerCore.Actor.Die(PlayerCore.Actor.transform.position);
            }
        }

        private void OnPlayerStateChanged()
        {
            if (!PlayerCore.StateMachine.UsingDrill)
            {
                RechargeSpikes();
            }
        }

        private void RechargeSpikes()
        {
            foreach (Spike spike in _dogoDisabledSpikes)
            {
                spike.Recharge();
            }
            _dogoDisabledSpikes.Clear();
        }
    }
}
