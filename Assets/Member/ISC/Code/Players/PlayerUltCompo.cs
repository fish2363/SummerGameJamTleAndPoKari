﻿using System;
using Ami.BroAudio;
using Member.CUH.Code.Entities;
using Member.KDH.Code.Bullet;
using Unity.Mathematics;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class PlayerUltCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private LayerMask whatIsBullet;
        
        [SerializeField] private int ultCombo;
        [SerializeField] private float ultRange;
        
        [SerializeField] private ParticleSystem parryParticle;

        [SerializeField] private SoundID ultSound;
        [SerializeField] private ParticleSystem ultChargeParticle;
        [SerializeField] private ParticleSystem ultUseParticle;
        
        private Player _player;

        private int _ultNum;
        private int _currentUltCombo;
        private int _idx = 1;
        
        public bool CanUlt { get; set; } = false;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        public void AfterInitialize()
        {
            _player.PlayerInput.OnUltPressed += HandleUltPressed;
            
            CanUlt = false;
        }

        private void OnDestroy()
        {
            _player.PlayerInput.OnUltPressed -= HandleUltPressed;
        }

        private void HandleUltPressed()
        {
            if (CanUlt)
            {
                ParticleSystem p = Instantiate(ultUseParticle, transform.position, Quaternion.identity);
                float size = ultRange + 1;
                p.gameObject.transform.localScale = new Vector3(size, size, size);
                p.Play();
                AllReflect();
            }

        }

        private void AllReflect()
        {
            Collider2D[] c = Physics2D.OverlapCircleAll(transform.position, ultRange, whatIsBullet);

            if (c.Length > 0)
            {
                ultSound.Play();
                foreach (Collider2D item in c)
                {
                    Bullet b = item.gameObject.GetComponent<Bullet>();
                    b.SetReflect(true);
                    parryParticle.Play();
                    Instantiate(parryParticle, b.transform);
                    Vector2 dir = _player.transform.position - b.transform.position;
                    b.Fire(-dir);
                }
                _ultNum--;
                CanUlt = false;
            }
        }

        public void SkillCheck()
        {
            int value = ComboManager.COMBO_CNT;
            if (value < ultCombo)
            {
                _idx = 1;
                _currentUltCombo = ultCombo;
            }
            if (CanUlt) return;
            
            
            if (ComboManager.COMBO_CNT >= _currentUltCombo)
            {
                ParticleSystem p = Instantiate(ultChargeParticle, transform.position, Quaternion.identity);
                p.Play();
                _ultNum++;
                if (_ultNum > 1)
                    _ultNum = 1;
                _idx++;
                _currentUltCombo = ultCombo * _idx;
                CanUlt = true;
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            
            Gizmos.DrawWireSphere(transform.position, ultRange);
            
            Gizmos.color = Color.white;
        }
        #endif
    }
}