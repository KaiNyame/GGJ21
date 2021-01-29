using System;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.Rendering;

namespace CellShading.Editor {
    public class CellShading : BaseShaderGUI {
        private readonly struct CellProperties {
            public readonly MaterialProperty SpecColor;
            public readonly MaterialProperty SpecGlossTex;
            public readonly MaterialProperty Smoothness;
            public readonly MaterialProperty BumpMapProp;
            public readonly MaterialProperty BumpScaleProp;
            public readonly MaterialProperty OcclusionStrength;
            public readonly MaterialProperty OcclusionMap;
            public readonly MaterialProperty HeightTex;
            public readonly MaterialProperty HeightMapDepth;
            public readonly MaterialProperty HeightMapFidelity;
            public readonly MaterialProperty LightRampSize;
            public readonly MaterialProperty[] LightRampColors;
            public readonly MaterialProperty[] LightRampValues;
            public readonly MaterialProperty LightRampSmoothness;
            public readonly MaterialProperty LightColorWeight;
            public readonly MaterialProperty AccentLightWidth;
            public readonly MaterialProperty AccentLightWeight;
            public readonly MaterialProperty StepSmoothness;
            public readonly MaterialProperty DotSize;
            public readonly MaterialProperty DotRotation;
            public readonly MaterialProperty HatchingWidth;
            public readonly MaterialProperty HatchingOffsetPerLayer;
            public readonly MaterialProperty HatchingRotation;
            public readonly MaterialProperty HatchingCross;
            public readonly MaterialProperty HatchingFrequency;
            public readonly MaterialProperty RenderModeFlag;

            public CellProperties(MaterialProperty[] properties) {
                SpecColor = FindProperty("_SpecColor", properties, false);
                SpecGlossTex = FindProperty("_SpecGlossMap", properties, false);
                Smoothness = FindProperty("_Smoothness", properties, false);
                BumpMapProp = FindProperty("_BumpMap", properties, false);
                BumpScaleProp = FindProperty("_BumpScale", properties, false);
                OcclusionStrength = FindProperty("_OcclusionStrength", properties, false);
                OcclusionMap = FindProperty("_OcclusionMap", properties, false);
                HeightTex = FindProperty("_HeightMap", properties, false);
                HeightMapDepth = FindProperty("_HeightMapDepth", properties, false);
                HeightMapFidelity = FindProperty("_HeightMapFidelity", properties, false);
                LightColorWeight = FindProperty("_LightColorWeight", properties, false);
                AccentLightWidth = FindProperty("_AccentLightWidth", properties, false);
                AccentLightWeight = FindProperty("_AccentLightWeight", properties, false);
                StepSmoothness = FindProperty("_StepSmoothness", properties, false);
                LightRampSmoothness = FindProperty("_LightRampSmoothness", properties, false);
                LightRampSize = FindProperty("_LightRampSize", properties, false);
                DotSize = FindProperty("_DotSize", properties, false);
                DotRotation = FindProperty("_DotRotation", properties, false);
                HatchingWidth = FindProperty("_HatchingWidth", properties, false);
                HatchingOffsetPerLayer = FindProperty("_HatchingOffsetPerLayer", properties, false);
                HatchingRotation = FindProperty("_HatchingRotation", properties, false);
                HatchingCross = FindProperty("_HatchingCross", properties, false);
                HatchingFrequency = FindProperty("_HatchingFrequency", properties, false);
                RenderModeFlag = FindProperty("_RenderMode", properties, false);

                LightRampColors = new MaterialProperty[RampMax];
                LightRampValues = new MaterialProperty[RampMax];
                for (var i = 0; i < RampMax; i++) {
                    LightRampColors[i] = FindProperty($"_LightRampColors{i}", properties, false);
                    LightRampValues[i] = FindProperty($"_LightRampValues{i}", properties, false);
                }
            }
        }
        
        private static class LabelText {
            public static readonly GUIContent SpecularMap = new GUIContent("Specular Map",
                "Sets and configures the map and color for the Specular workflow.");

            public static readonly GUIContent Smoothness = new GUIContent("Smoothness",
                "Controls the spread of highlights and reflections on the surface.");

            public static readonly GUIContent Occlusion = new GUIContent("Occlusion Map",
                "Sets an occlusion map to simulate shadowing from ambient lighting.");
            
            public static readonly GUIContent Height = new GUIContent("Height Map",
                "Sets a height map to simulate depth using parallax depth mapping (can be expensive).");
            
            public static readonly GUIContent HeightFidelity = new GUIContent("Height Map Fidelity",
                "Sets the number of samples to use when calculating depth. (lower = faster, higher = more detailed");
            
            public static readonly GUIContent LightRamp = new GUIContent("Light Ramp",
                "Sets a light ramp that defines the mapping between smooth lighting and your desired shading.");

            public static readonly GUIContent LightRampSmoothness = new GUIContent("Light Smoothness",
                "Controls how smoothness the light ramp is between colors.");

            public static readonly GUIContent LightColorWeight = new GUIContent("Light Color Influence",
                "Controls how much the a light's color can affect the final color of the material.");
            
            public static readonly GUIContent AccentLightingWeight = new GUIContent("Accent Lighting",
                "How much accent lighting to add to the material's lighting.");
            
            public static readonly GUIContent AccentLightingWidth = new GUIContent("Accent Lighting Width",
                "The overall width of the accent lighting.");

            public static readonly GUIContent StepSmoothness = new GUIContent("Accent Light Smoothness",
                "The smoothness of the backface and side lighting.");
            
            public static readonly GUIContent DotSize = new GUIContent("Dot Size", 
                "The maximum size of the dots that appear across the screen");
            
            public static readonly GUIContent DotRotation = new GUIContent("Dot Rotation", 
                "The rotation of the dots across the screen.");
            
            public static readonly GUIContent CrossHatching = new GUIContent("Cross Hatch", 
                "Use a cross patter for the hatching style.");
            
            public static readonly GUIContent HatchingFrequency = new GUIContent("Hatching Line Frequency", 
                "The frequency of the lines in the hatching style.");
            
            public static readonly GUIContent HatchingWidth = new GUIContent("Hatching Line Width", 
                "The width of the lines in the hatching style.");
            
            public static readonly GUIContent HatchingSpacing = new GUIContent("Hatching Spacing", 
                "The spacing between the lines in the hatching style.");
            
            public static readonly GUIContent HatchingRotation = new GUIContent("Hatching Rotation", 
                "The rotation of the hatching lines across the screen.");

            public static readonly GUIContent Style = new GUIContent("Style", 
                "The style of the cell shading.");
        }
        
        // Properties
        private CellProperties _cellProperties;
        
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int Emission = Shader.PropertyToID("_Emission");
        private static readonly int AlphaClip = Shader.PropertyToID("_AlphaClip");
        private static readonly int Surface = Shader.PropertyToID("_Surface");
        private static readonly int Blend = Shader.PropertyToID("_Blend");
        private static readonly int SpecGlossMap = Shader.PropertyToID("_SpecGlossMap");
        private static readonly int MetallicSpecGlossMap = Shader.PropertyToID("_MetallicSpecGlossMap");
        private static readonly int HeightMap = Shader.PropertyToID("_HeightMap");
        private static readonly int RenderMode = Shader.PropertyToID("_RenderMode");
        
        private const int RampMax = 8;
        
        private readonly string[] _styles = new string[] { "Bands", "Dots", "Hatching" };

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties) {
            base.FindProperties(properties);
            _cellProperties = new CellProperties(properties);
        }

        // material changed check
        public override void MaterialChanged(Material material) {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords);
            if(material.HasProperty("_HeightMap"))
                CoreUtils.SetKeyword(material, "_HEIGHTMAP", material.GetTexture(HeightMap));

            if (!material.HasProperty("_RenderMode")) return;
            var mode = material.GetFloat(RenderMode);
            CoreUtils.SetKeyword(material, "_DOTS_MODE", Mathf.RoundToInt(mode) == 1);
            CoreUtils.SetKeyword(material, "_HATCHING_MODE", Mathf.RoundToInt(mode) == 2);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material) {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck()) {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
            base.DrawSurfaceOptions(material);
        }

        private static void SliderValue(MaterialProperty prop, GUIContent label, float min, float max) {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            var value = EditorGUILayout.Slider(label, prop.floatValue, min, max);
            if (EditorGUI.EndChangeCheck()) prop.floatValue = value;
            EditorGUI.showMixedValue = false;
        }

        private void LightRampGradient() {
            var count = Mathf.RoundToInt(_cellProperties.LightRampSize.floatValue);
            
            var gradient = new Gradient();
            if (count > 0) {

                var keys = new GradientColorKey[count];
                for (var i = 0; i < count; i++) {
                    keys[i].color = _cellProperties.LightRampColors[i].colorValue;
                    keys[i].time = _cellProperties.LightRampValues[i].floatValue;
                }

                gradient.colorKeys = keys;
            }
            
            gradient.mode = GradientMode.Fixed;
            
            EditorGUI.BeginChangeCheck();

            gradient = EditorGUILayout.GradientField(LabelText.LightRamp, gradient, true);

            if (!EditorGUI.EndChangeCheck()) return;
            
            var newCount = Math.Min(gradient.colorKeys.Length, RampMax);

            for (var i = 0; i < newCount; i++) {
                _cellProperties.LightRampColors[i].colorValue = gradient.colorKeys[i].color;
                _cellProperties.LightRampValues[i].floatValue = gradient.colorKeys[i].time;
            }

            _cellProperties.LightRampSize.floatValue = Mathf.Round(newCount);
        }

        private void RenderModeInputs() {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            var mode = Mathf.Clamp(Mathf.RoundToInt(_cellProperties.RenderModeFlag.floatValue), 0, _styles.Length - 1);
            
            EditorGUI.BeginChangeCheck();
            mode = EditorGUILayout.Popup(LabelText.Style, mode, _styles);
            if (EditorGUI.EndChangeCheck()) _cellProperties.RenderModeFlag.floatValue = mode;

            if (mode == 1) {
                SliderValue(_cellProperties.DotSize, LabelText.DotSize, 0, 1);
                SliderValue(_cellProperties.DotRotation, LabelText.DotRotation, 0, 1);
            }
            else if (mode == 2) {
                var cross = _cellProperties.HatchingCross.floatValue > 0;
                
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = _cellProperties.HatchingCross.hasMixedValue;
                cross = EditorGUILayout.ToggleLeft(LabelText.CrossHatching, cross);
                if (EditorGUI.EndChangeCheck()) _cellProperties.HatchingCross.floatValue = cross ? 1 : 0;
                EditorGUI.showMixedValue = false;
                
                SliderValue(_cellProperties.HatchingFrequency, LabelText.HatchingFrequency, 0, 1);
                SliderValue(_cellProperties.HatchingWidth, LabelText.HatchingWidth, 0, 1);
                SliderValue(_cellProperties.HatchingOffsetPerLayer, LabelText.HatchingSpacing, 0, 1);
                SliderValue(_cellProperties.HatchingRotation, LabelText.HatchingRotation, 0, 1);
            }
        }

        private void Inputs(MaterialEditor matEditor) {

            LightRampGradient();
            SliderValue(_cellProperties.LightRampSmoothness, LabelText.LightRampSmoothness, 0, 1);
            SliderValue(_cellProperties.LightColorWeight, LabelText.LightColorWeight, 0, 1);
            EditorGUILayout.Space();
            SliderValue(_cellProperties.AccentLightWeight, LabelText.AccentLightingWeight, 0, 1);
            SliderValue(_cellProperties.AccentLightWidth, LabelText.AccentLightingWidth, 0, 1);
            SliderValue(_cellProperties.StepSmoothness, LabelText.StepSmoothness, 0, 1);
            EditorGUILayout.Space();

            DrawSpecularArea(_cellProperties, matEditor);
            DrawNormalArea(matEditor, _cellProperties.BumpMapProp, _cellProperties.BumpScaleProp);

            if (_cellProperties.HeightTex != null) {
                matEditor.TexturePropertySingleLine(
                    LabelText.Height,
                    _cellProperties.HeightTex,
                    _cellProperties.HeightTex.textureValue != null ? _cellProperties.HeightMapDepth : null
                );
                if (_cellProperties.HeightTex.textureValue != null)
                    SliderValue(_cellProperties.HeightMapFidelity, LabelText.HeightFidelity, 0, 1);
            }

            if (_cellProperties.OcclusionMap != null) {
                matEditor.TexturePropertySingleLine(
                    LabelText.Occlusion,
                    _cellProperties.OcclusionMap,
                    _cellProperties.OcclusionMap.textureValue != null ? _cellProperties.OcclusionStrength : null
                );
            }
        }
        
        private static void DrawSpecularArea(CellProperties properties, MaterialEditor materialEditor) {
            TextureColorProps(materialEditor, LabelText.SpecularMap, properties.SpecGlossTex, properties.SpecColor);
            EditorGUI.indentLevel++;
            DrawSmoothnessArea(properties);
            EditorGUI.indentLevel--;
        }

        private static void DrawSmoothnessArea(CellProperties properties) {
            EditorGUI.indentLevel++;
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUI.showMixedValue = properties.Smoothness.hasMixedValue;
            var smoothness = EditorGUILayout.Slider(LabelText.Smoothness, properties.Smoothness.floatValue, 0f, 1f);
            if (EditorGUI.EndChangeCheck()) properties.Smoothness.floatValue = smoothness;
            EditorGUI.showMixedValue = false;
            
            EditorGUI.indentLevel--;
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material) {
            base.DrawSurfaceInputs(material);
            Inputs(materialEditor);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
            RenderModeInputs();
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader) {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
                material.SetColor(EmissionColor, material.GetColor(Emission));

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/")) {
                SetupMaterialBlendMode(material);
                return;
            }

            var surfaceType = SurfaceType.Opaque;
            var blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/")) {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat(AlphaClip, 1);
            }
            else if (oldShader.name.Contains("/Transparent/")) {
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat(Surface, (float)surfaceType);
            material.SetFloat(Blend, (float)blendMode);

            if (oldShader.name.Equals("Standard (Specular setup)")) {
                Texture texture = material.GetTexture(SpecGlossMap);
                if (texture != null) material.SetTexture(MetallicSpecGlossMap, texture);
            }

            MaterialChanged(material);
        }
    }
}