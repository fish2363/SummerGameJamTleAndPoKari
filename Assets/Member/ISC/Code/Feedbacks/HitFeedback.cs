using System.Collections;
using Member.ISC.Code.Players;
using UnityEngine;

namespace Member.ISC.Code.Feedbacks
{
    public class HitFeedback : Feedback
    {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private float _delay;
        [SerializeField] private float cycleNum;

        [SerializeField] private Player player;
        
        private float _startTime;
        
        public override void CreateFeedback()
        {
            StartCoroutine(Blink());
        }

        private IEnumerator Blink()
        {
            while (_startTime < cycleNum)
            {
                yield return new WaitForSeconds(_delay);
                sprite.enabled = false;
                yield return new WaitForSeconds(_delay);
                sprite.enabled = true;
                _startTime++;
            }

            player.IsHitting = false;
            _startTime = 0;
        }

        public override void StopFeedback()
        {
        }
    }
}