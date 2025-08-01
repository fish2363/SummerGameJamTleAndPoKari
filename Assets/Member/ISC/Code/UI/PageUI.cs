using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Member.ISC.Code.UI
{
    public class PageUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] pageArr;
        [SerializeField] private Button nextBtn;
        [SerializeField] private Button backBtn;
        
        [SerializeField] private RectTransform trm;
        
        private int _currentPage = 1;

        private float _currentPageX;

        private Tween _tween;
        
        private void Start()
        {
            ButtonUpdate();
            _currentPageX = trm.anchoredPosition.x;
        }

        private void OnDestroy()
        {
            if (_tween.IsActive()) 
                _tween.Kill();
        }

        public void NextPage()
        {
            _currentPage++;
            ButtonUpdate();
            _currentPageX -= 2000;
            if (_tween.IsActive()) _tween.Kill();
            _tween = DOVirtual.Float(trm.anchoredPosition.x, _currentPageX, 0.5f, (x) => trm.anchoredPosition = new Vector2(x, trm.anchoredPosition.y)).SetEase(Ease.OutSine);
        }

        public void BackPage()
        {
            _currentPage--;
            ButtonUpdate();
            _currentPageX += 2000;
            if (_tween.IsActive()) _tween.Kill();
            _tween = DOVirtual.Float(trm.anchoredPosition.x, _currentPageX, 0.5f, (x) => trm.anchoredPosition = new Vector2(x, trm.anchoredPosition.y)).SetEase(Ease.OutSine);

        }
        
        private void ButtonUpdate()
        {
            if (_currentPage <= 1)
            {
                backBtn.gameObject.SetActive(false);
            }
            else
            {
                backBtn.gameObject.SetActive(true);
            }

            if (_currentPage >= pageArr.Length)
            {
                nextBtn.gameObject.SetActive(false);
            }
            else
            {
                nextBtn.gameObject.SetActive(true);
            }
        }
    }
}