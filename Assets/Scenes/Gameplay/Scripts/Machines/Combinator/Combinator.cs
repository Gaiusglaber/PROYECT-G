using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using ProyectG.Gameplay.UI;

namespace ProyectG.Gameplay.Objects.Machines
{
    public class Combinator : Machine
    {
        #region EXPOSED_FIELDS
        [SerializeField] private UICombinator uiCombinator;
        #endregion

        #region OVERRIDES
        public override void Init(BaseView viewAttach)
        {
            base.Init(viewAttach);
        }

        public override void TriggerAnimation(string triggerId)
        {
            base.TriggerAnimation(triggerId);
        }

        public override void TriggerSoundEffect(string idSound)
        {
            base.TriggerSoundEffect(idSound);
        }
        #endregion
    }
}