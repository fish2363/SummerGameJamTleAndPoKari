using UnityEngine;

namespace Member.CUH.Code.Entities.FSM
{
    [CreateAssetMenu(fileName = "StateSO", menuName = "SO/FSM/StateSO")]
    public class StateSO : ScriptableObject
    {
        public StateName stateName;
        public string className;
        public AnimParamSO animParam;
    }
}
