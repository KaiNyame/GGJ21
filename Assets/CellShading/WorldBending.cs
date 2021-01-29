using UnityEngine;
using UnityEngine.Rendering;

namespace Shaders.CellShading {
    [ExecuteAlways]
    public class WorldBending : MonoBehaviour {
        public bool bendingEnabled;
        [Range(-0.25f, 0.25f)]
        public float verticalBending = 0.015f;
        [Range(-0.025f, 0.25f)]
        public float horizontalBending = 0.005f;

        private bool _set;
        private static readonly int BendAmount = Shader.PropertyToID("_BendAmount");

        public void OnEnable() {
            if (!Application.isPlaying) return;

            RenderPipelineManager.beginCameraRendering += SetBendingProperties;
            RenderPipelineManager.endCameraRendering += ResetCulling;
            
            _set = true;
        }

        public void OnDisable() {
            if (!_set) return;
            
            RenderPipelineManager.beginCameraRendering -= SetBendingProperties;
            RenderPipelineManager.endCameraRendering -= ResetCulling;

            _set = false;
        }

        private void SetBendingProperties(ScriptableRenderContext ctx, Camera cam) {
            Shader.SetGlobalVector(BendAmount, new Vector4(horizontalBending, 0, verticalBending, 0));
            cam.cullingMatrix = Matrix4x4.Ortho(-99, 99, -99, 99, 0.001f, 99) * cam.worldToCameraMatrix;
        }

        private void ResetCulling(ScriptableRenderContext ctx, Camera cam) {
            cam.ResetCullingMatrix();
        }

        public void Update() {
            if (Application.isPlaying && bendingEnabled) {
                if (Shader.IsKeywordEnabled("_WORLD_BEND")) return;
                Shader.EnableKeyword("_WORLD_BEND");
            }
            else if (Shader.IsKeywordEnabled("_WORLD_BEND")) Shader.DisableKeyword("_WORLD_BEND");
        }
    }
}