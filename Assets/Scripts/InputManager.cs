using UnityEngine;

namespace TableDungeon
{
    public static class InputManager
    {
        public static readonly Controls Controls = new Controls();
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnInitialize()
        {
            Controls.Enable();
        }
    }
}