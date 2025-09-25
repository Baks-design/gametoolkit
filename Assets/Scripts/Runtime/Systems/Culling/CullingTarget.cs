using Alchemy.Inspector;
using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;
using UnityEngine.Events;

namespace GameToolkit.Runtime.Systems.Culling
{
    public class CullingTarget : MonoBehaviour, IUpdatable
    {
        [SerializeField, Required]
        Renderer objectRenderer;

        [field: SerializeField]
        public float BoundarySphereRadius { get; private set; } = 1f;

        [SerializeField]
        float fadeDuration = 2f;

        [SerializeField]
        CullingBehavior cullingMode = CullingBehavior.FadeInOut;

        [SerializeField]
        bool isPriorityObject;

        [SerializeField]
        UnityEvent onCulled,
            onVisible;

        MaterialPropertyBlock mpb;
        MonoBehaviour[] scripts;
        CountdownTimer fadeTimer;
        ICullingServices cullingServices;
        float startAlpha;
        float currentAlpha;
        float targetAlpha;
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");
        IUpdateServices updateServices;

        void Awake()
        {
            objectRenderer = gameObject.GetComponentInChildren<Renderer>();

            scripts = GetComponents<MonoBehaviour>();
            for (var i = 0; i < scripts.Length; i++)
                if (scripts[i] == this)
                    scripts[i] = null;

            fadeTimer = new CountdownTimer(fadeDuration);
        }

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);

            ServiceLocator.Global.Get(out cullingServices);
            if (isPriorityObject)
                onVisible?.Invoke();
            else
                cullingServices.Register(this);

            currentAlpha = GetAlpha();
        }

        public void ProcessUpdate(float deltaTime)
        {
            if (!fadeTimer.IsRunning)
                return;

            var t = 1f - Mathf.Clamp01(fadeTimer.Progress);
            var a = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetAlpha(a);
        }

        void BeginFadeTo(float target, bool deactivate)
        {
            if (!objectRenderer)
                return;

            mpb ??= new MaterialPropertyBlock();

            startAlpha = currentAlpha;
            targetAlpha = Mathf.Clamp01(target);

            if (deactivate && targetAlpha <= 0f)
            {
                fadeTimer.OnTimerStop = () =>
                {
                    if (objectRenderer)
                        objectRenderer.enabled = false;
                };
            }
            else
                fadeTimer.OnTimerStop = () => { };

            fadeTimer.Reset(fadeDuration);
            fadeTimer.Start();
        }

        void EnableScripts(bool v)
        {
            for (var i = 0; i < scripts.Length; i++)
            {
                var s = scripts[i];
                if (s == null)
                    continue;
                s.enabled = v;
            }
        }

        public void ToggleOn()
        {
            if (fadeTimer.IsRunning)
                fadeTimer.Stop();

            if (isPriorityObject)
            {
                onVisible?.Invoke();
                return;
            }

            switch (cullingMode)
            {
                case CullingBehavior.FadeInOut:
                    if (objectRenderer && !objectRenderer.enabled)
                        objectRenderer.enabled = true;
                    BeginFadeTo(1f, deactivate: false);
                    break;
                case CullingBehavior.ToggleScripts:
                    EnableScripts(true);
                    break;
            }
        }

        public void ToggleOff()
        {
            if (fadeTimer.IsRunning)
                fadeTimer.Stop();

            if (isPriorityObject)
                return;

            switch (cullingMode)
            {
                case CullingBehavior.FadeInOut:
                    BeginFadeTo(0f, deactivate: true);
                    break;
                case CullingBehavior.ToggleScripts:
                    EnableScripts(false);
                    break;
            }

            onCulled?.Invoke();
        }

        float GetAlpha()
        {
            if (!objectRenderer)
                return 1f;

            var m = objectRenderer.sharedMaterial;
            if (!m)
                return 1f;

            if (m.HasProperty(BaseColorId))
                return m.GetColor(BaseColorId).a;
            if (m.HasProperty(ColorId))
                return m.GetColor(ColorId).a;

            return 1f;
        }

        void SetAlpha(float a)
        {
            if (!objectRenderer)
                return;

            var m = objectRenderer.sharedMaterial;
            if (!m)
                return;

            currentAlpha = Mathf.Clamp01(a);

            mpb ??= new MaterialPropertyBlock();
            objectRenderer.GetPropertyBlock(mpb);

            if (m.HasProperty(BaseColorId))
            {
                var c = m.GetColor(BaseColorId);
                c.a = currentAlpha;
                mpb.SetColor(BaseColorId, c);
            }
            else if (m.HasProperty(ColorId))
            {
                var c = m.GetColor(ColorId);
                c.a = currentAlpha;
                mpb.SetColor(ColorId, c);
            }

            objectRenderer.SetPropertyBlock(mpb);
        }

        void OnDisable()
        {
            if (!isPriorityObject)
                cullingServices.Deregister(this);

            updateServices.Unregister(this);
        }
    }
}
