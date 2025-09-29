using System;
using System.Collections.Generic;
using GameToolkit.Runtime.Game.Systems.Update;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Systems.Culling
{
    public class CullingManager : MonoBehaviour, IUpdatable, ICullingServices
    {
        [SerializeField]
        float maxCullingDistance = 100f;

        [SerializeField]
        LayerMask cullableLayers;

        [SerializeField]
        List<string> cullableTags = new();

        [SerializeField]
        float updateInterval = 0.1f;

        Camera cullingCamera;
        CullingGroup group;
        int count;
        float tPos;
        int[] tmp = new int[256];
        HashSet<string> tagSet;
        BoundingSphere[] spheres = new BoundingSphere[64];
        readonly List<CullingTarget> owners = new(64);
        readonly Dictionary<CullingTarget, int> map = new(64);
        IUpdateServices updateServices;

        public void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            SetupCulling();
        }

        void SetupCulling()
        {
            cullingCamera = Camera.main;

            group = new CullingGroup
            {
                onStateChanged = OnStateChanged,
                targetCamera = cullingCamera
            };
            group.SetBoundingSpheres(spheres);
            group.SetBoundingSphereCount(0);
            group.SetDistanceReferencePoint(cullingCamera.transform);
            group.SetBoundingDistances(new float[] { maxCullingDistance }); // single band
            //group.SetBoundingDistances(new float[]{ 10f, 25f, 60f });    // example of multiple bands
            tagSet = new(cullableTags);
        }

        void Start()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

        void OnDisable()
        {
            if (group == null)
                return;

            group.onStateChanged = null;
            group.Dispose();
            group = null;

            updateServices?.Unregister(this);
        }

        public void ProcessUpdate(float deltaTime)
        {
            tPos += deltaTime;
            if (tPos < updateInterval)
                return;

            for (var i = 0; i < count; i++)
            {
                var o = owners[i];
                if (!o)
                    continue;

                var s = spheres[i];
                s.position = o.transform.position;
                s.radius = o.BoundarySphereRadius;
                spheres[i] = s;
            }

            tPos = 0f;
        }

        public void Register(CullingTarget t)
        {
            if (!t)
                return;

            if (count == spheres.Length)
            {
                Array.Resize(ref spheres, count * 2);
                group.SetBoundingSpheres(spheres);
            }

            owners.Add(t);
            map[t] = count;
            spheres[count] = new BoundingSphere(t.transform.position, t.BoundarySphereRadius);
            count++;
            group.SetBoundingSphereCount(count);
        }

        public void Deregister(CullingTarget t)
        {
            if (group == null || !t || !map.TryGetValue(t, out var i))
                return;

            group.EraseSwapBack(i);
            CullingGroup.EraseSwapBack(i, spheres, ref count);
            var last = owners.Count - 1;
            var moved = owners[last];
            owners[i] = moved;
            owners.RemoveAt(last);
            if (moved)
                map[moved] = i;
            map.Remove(t);
            group.SetBoundingSphereCount(count);
        }

        void OnStateChanged(CullingGroupEvent e)
        {
            var cullingTarget = owners[e.index];
            if (!cullingTarget)
                return;

            if (!IsCullable(cullingTarget.gameObject))
            {
                cullingTarget.ToggleOn();
                return;
            }

            var inRange = e.currentDistance == 0;
            if (e.isVisible && inRange)
                cullingTarget.ToggleOn();
            else
                cullingTarget.ToggleOff();
        }

        bool IsCullable(GameObject obj) =>
            ((1 << obj.layer) & cullableLayers) != 0 && tagSet.Contains(obj.tag); // layer and tag check

        bool IsWithinDistance(Vector3 p) =>
            Vector3.Distance(cullingCamera.transform.position, p) <= maxCullingDistance;

        int GetBandTargets(int band, List<CullingTarget> outList, bool? visible = null)
        {
            if (tmp.Length < count)
                tmp = new int[count];
            var n = visible.HasValue
                ? group.QueryIndices(visible.Value, band, tmp, 0)
                : group.QueryIndices(band, tmp, 0);
            outList.Clear();
            for (var i = 0; i < n; i++)
                outList.Add(owners[tmp[i]]);
            return n;
        }

        public (int visible, int culled) Snapshot()
        {
            if (tmp.Length < count)
                tmp = new int[count];
            var vis = group.QueryIndices(true, tmp, 0);
            return (vis, count - vis);
        }
    }
}
