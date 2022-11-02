using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace CustomEditors
{
    [EditorToolbarElement(id, typeof(SceneView))]
    public class CanvasViewButtonBar : EditorToolbarButton, IAccessContainerWindow
    {
        public const string id = "UIButton";

        private bool _isHidden;

        public EditorWindow containerWindow { get; set; }

        public CanvasViewButtonBar()
        {
            icon = EditorGUIUtility.IconContent("Canvas Icon").image as Texture2D;

            clicked += OnClick;
        }

        private void OnClick()
        {
            if (!_isHidden)
                ShowUILayer();
            else
                HideUILayer();

            SceneView.RepaintAll();

            _isHidden = !_isHidden;

            if (containerWindow is SceneView view)
                view.FrameSelected();
        }

        private void ShowUILayer()
        {
            Canvas canvas = Object.FindObjectOfType<Canvas>();

            if (null == canvas)
                return;

            icon = EditorGUIUtility.IconContent("UnityEditor.SceneView").image as Texture2D;

            LayerMask layerNumberBinary = 1 << 5;
            Tools.visibleLayers = layerNumberBinary;

            SceneView sceneView = SceneView.sceneViews[0] as SceneView;
            sceneView.in2DMode = true;
            sceneView.camera.clearFlags = CameraClearFlags.SolidColor;
            sceneView.sceneViewState.showSkybox = false;

            sceneView.AlignViewToObject(canvas.transform);
        }

        private void HideUILayer()
        {
            icon = EditorGUIUtility.IconContent("Canvas Icon").image as Texture2D
            ;
            Tools.visibleLayers = -1;

            LayerMask layerNumberBinary = 1 << 5;
            LayerMask flippedVisibleLayers = ~Tools.visibleLayers;
            Tools.visibleLayers = ~(flippedVisibleLayers | layerNumberBinary);

            SceneView sceneView = SceneView.sceneViews[0] as SceneView;
            sceneView.in2DMode = false;

            GameObject centerOfMap = new GameObject();

            sceneView.AlignViewToObject(centerOfMap.transform);

            Object.DestroyImmediate(centerOfMap);
        }
    }

    [Overlay(typeof(SceneView), "UI Show-Hide")]
    public class CanvasViewToolBarButton : ToolbarOverlay
    {
        CanvasViewToolBarButton() : base(CanvasViewButtonBar.id) { }
    }
}