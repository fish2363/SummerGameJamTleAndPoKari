using System;
using System.Collections.Generic;
using Member.ISC.Code.Players;
using UnityEngine;

namespace Member.ISC.Code.UI
{
    public class LifePanelUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private GameObject lifePanel;

        private List<GameObject> _lifePanels;

        private void Awake()
        {
            _lifePanels = new List<GameObject>();
        }

        private void Start()
        {
            InitLife();
        }

        private void InitLife()
        {
            for (int i = 0; i < playerHealth.currentHealth; i++)
            {
                GameObject obj = Instantiate(lifePanel, transform);
                
                _lifePanels.Add(obj);
            }
        }

        public void UpdateLife()
        {
            if (_lifePanels.Count <= 0)
            {
                Debug.Log("남은 체력 없음.");
                return;
            }

            for (int i = 0; i < _lifePanels.Count; i++)
            {
                if (playerHealth.currentHealth-1 < i)
                {
                    _lifePanels[i].SetActive(false);
                }
            }
        }
    }
}