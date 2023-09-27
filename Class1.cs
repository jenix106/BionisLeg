using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BionisLeg
{
    public class BionisLegModule : LevelModule
    {
        public override IEnumerator OnLoadCoroutine()
        {
            foreach (Transform transform in level.customReferences.Find(match => match.name == "Climbable").transforms)
            {
                transform.gameObject.AddComponent<ClimbableComponent>();
            }
            foreach (Transform transform in level.customReferences.Find(match => match.name == "NoFallDamage").transforms)
            {
                transform.gameObject.AddComponent<WaterComponent>();
            }
            EventManager.onUnpossess += EventManager_onUnpossess;
            EventManager.onPossess += EventManager_onPossess;
            return base.OnLoadCoroutine();
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
            if(eventTime == EventTime.OnEnd)
            Player.local.head.cam.farClipPlane *= 5;
        }

        private void EventManager_OnPlayerSpawned()
        {
            Player.local.head.cam.farClipPlane *= 5;
        }

        private void EventManager_onUnpossess(Creature creature, EventTime eventTime)
        {
            Player.local.head.cam.farClipPlane /= 5;
        }

        public override void OnUnload()
        {
            base.OnUnload();
            EventManager.onUnpossess -= EventManager_onUnpossess;
            EventManager.onPossess -= EventManager_onPossess;
        }
    }
    public class ClimbableComponent : MonoBehaviour
    {
        public void OnCollisionStay(Collision c)
        {
            if (c.collider.GetComponentInParent<Player>() != null)
            {
                RagdollHandClimb.climbFree = true;
            }
        }
        public void OnCollisionExit(Collision c)
        {
            if (c.collider.GetComponentInParent<Player>() != null)
            {
                RagdollHandClimb.climbFree = false;
            }
        }
    }
    public class WaterComponent : MonoBehaviour
    {
        static bool fallDamage;
        static bool isStored;
        public void OnTriggerStay(Collider c)
        {
            if (c.GetComponentInParent<Player>() != null)
            {
                if (!isStored)
                {
                    fallDamage = Player.fallDamage;
                    isStored = true;
                }
                Player.fallDamage = false;
            }
        }
        public void OnTriggerExit(Collider c)
        {
            if (c.GetComponentInParent<Player>() != null)
            {
                Player.fallDamage = fallDamage;
                isStored = false;
            }
        }
    }
    public class SetTexture : StateMachineBehaviour
    {
        public List<Renderer> renderers;
        public Texture2D texture;
        public void Start()
        {
            if (Level.current?.customReferences.Find(match => match.name == "Water")?.transforms[0] != null)
                foreach (Transform reference in Level.current.customReferences.Find(match => match.name == "Water").transforms)
                {
                    if (!renderers.Contains(reference.GetComponent<Renderer>())) renderers.Add(reference.GetComponent<Renderer>());
                }
        }
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (renderers.IsNullOrEmpty())
                foreach (Transform reference in Level.current.customReferences.Find(match => match.name == "Water").transforms)
                {
                    if (!renderers.Contains(reference.GetComponent<Renderer>())) renderers.Add(reference.GetComponent<Renderer>());
                }
            if (!renderers.IsNullOrEmpty())
                foreach (Renderer renderer in renderers)
                {
                    if (renderer != null)
                        renderer.material.SetTexture("_BaseMap", texture);
                }
        }
    }
}
