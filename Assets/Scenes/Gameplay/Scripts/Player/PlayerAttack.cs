using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Interfaces;

namespace ProyectG.Player.Attack
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private float range;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
                foreach (Collider2D hit in hits)
                {
                    if (hit.TryGetComponent(out IHittable hittable))
                    {
                        hittable.OnHit();
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
