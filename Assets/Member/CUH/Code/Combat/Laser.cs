using DG.Tweening;
using UnityEngine;

namespace Member.CUH.Code.Combat
{
    [RequireComponent(typeof(LineRenderer))]
    public class Laser : MonoBehaviour
    {
        [SerializeField] private GameObject laserWarning1;
        [SerializeField] private GameObject laserWarning2;
        [SerializeField] private LayerMask whatIsTarget;
        
        [Header("Setting Values")] 
        [SerializeField] private float laserMultiplier = 1f;
        [SerializeField] private float halfAngle = 90f;
        [SerializeField] private float chargingTime = 0.5f;
        [SerializeField] private float shootTime = 0.25f;
        
        private LineRenderer _laserLine;

        private void Awake()
        {
            laserWarning1.SetActive(false);
            laserWarning2.SetActive(false);
            _laserLine = GetComponent<LineRenderer>();
        }

        public void Shoot(Vector3 target, Transform owner)
        {
            _laserLine.positionCount = 0;
            _laserLine.widthMultiplier = laserMultiplier;
            laserWarning1.SetActive(true);
            laserWarning2.SetActive(true);
            
            laserWarning1.transform.rotation = Quaternion.Euler(0, 0, halfAngle);
            laserWarning2.transform.rotation = Quaternion.Euler(0, 0, -halfAngle);
            
            Vector3 targetDir = (target - owner.position).normalized;
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            Vector3 rotate = new Vector3(0, 0, angle - 90f);
            laserWarning1.transform.DOLocalRotate(rotate, chargingTime).SetEase(Ease.OutCubic);
            laserWarning2.transform.DOLocalRotate(rotate, chargingTime).SetEase(Ease.OutCubic).
                OnComplete(() =>
                {
                    laserWarning1.SetActive(false);
                    laserWarning2.SetActive(false);
                    _laserLine.positionCount = 2;
                    _laserLine.SetPosition(0, transform.position);
                    _laserLine.SetPosition(1, transform.position + targetDir * 30f);
                    RaycastHit2D hit = Physics2D.Raycast(transform.position,
                        targetDir, 30f, whatIsTarget);
                    if (hit.collider != null)
                    {
                        hit.collider.GetComponent<IDamageable>().ApplyDamage(1);
                    }

                    DOTween.To(() => _laserLine.widthMultiplier,
                        x => _laserLine.widthMultiplier = x, 0f, shootTime)
                        .OnComplete(() => Destroy(gameObject));

                });
        }
        
        private void OnDestroy()
        {
            laserWarning1.transform.DOKill();
            laserWarning2.transform.DOKill();
            _laserLine.DOKill();
        }
    }
}