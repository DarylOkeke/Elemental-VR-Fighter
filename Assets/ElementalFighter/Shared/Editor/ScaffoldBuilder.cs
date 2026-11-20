using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;

public static class ScaffoldBuilder
{
    const string Root = "Assets/ElementalFighter/";
    static void Ref(Object target, string name, Object value)
    {
        var serialized = new SerializedObject(target);
        serialized.FindProperty(name).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
    static GameObject Child(string name, Transform parent, Vector3 position)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        return obj;
    }
    static void SavePrefab(GameObject obj, string owner)
    {
        string path = Root + owner + "/Prefabs";
        Directory.CreateDirectory(path);
        PrefabUtility.SaveAsPrefabAssetAndConnect(obj, path + "/" + obj.name + ".prefab", InteractionMode.AutomatedAction);
    }
    [MenuItem("Elemental Fighter/Rebuild Shared Scaffold")]
    public static void Build()
    {
        if (File.Exists(Root + "Shared/Scenes/ElementalArena.unity")) throw new System.InvalidOperationException("Shared scene already exists. Preserve teammate edits; do not rebuild it.");
        Directory.CreateDirectory(Root + "Shared/Scenes");
        Directory.CreateDirectory(Root + "Shared/Settings");
        AssetDatabase.Refresh();
        EditorSettings.serializationMode = SerializationMode.ForceText;
        EditorBuildSettings.RemoveConfigObject("com.unity.input.settings.actions");
        PlayerSettings.companyName = "CS417 Team";
        PlayerSettings.productName = "Elemental VR Fighter";
        var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        settings.FindProperty("activeInputHandler").intValue = 1;
        settings.ApplyModifiedPropertiesWithoutUndo();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var light = new GameObject("Arena Light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.3f;
        light.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        RenderSettings.ambientLight = new Color(0.35f, 0.4f, 0.5f);
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Arena Floor";
        floor.transform.position = new Vector3(0f,-0.15f,0f);
        floor.transform.localScale = new Vector3(12f,0.3f,12f);
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = new Color(0.18f,0.25f,0.32f);
        AssetDatabase.CreateAsset(material, Root + "Shared/Settings/ArenaFloor.mat");
        floor.GetComponent<Renderer>().sharedMaterial = material;
        var rig = new GameObject("Daryl_MovementRig");
        rig.transform.position = new Vector3(0f,0f,-3f);
        var origin = rig.AddComponent<XROrigin>();
        origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
        var offset = Child("Camera Offset",rig.transform,Vector3.zero);
        origin.CameraFloorOffsetObject = offset;
        var cameraObj = Child("Main Camera",offset.transform,new Vector3(0f,1.65f,0f));
        cameraObj.tag = "MainCamera";
        var camera = cameraObj.AddComponent<Camera>();
        camera.nearClipPlane = 0.05f;
        cameraObj.AddComponent<AudioListener>();
        origin.Camera = camera;
        var driver = cameraObj.AddComponent<TrackedPoseDriver>();
        driver.positionInput = new InputActionProperty(new InputAction("Head Position", InputActionType.Value,"<XRHMD>/centerEyePosition"));
        driver.rotationInput = new InputActionProperty(new InputAction("Head Rotation", InputActionType.Value,"<XRHMD>/centerEyeRotation"));
        driver.trackingStateInput = new InputActionProperty(new InputAction("Head Tracking", InputActionType.Value,"<XRHMD>/trackingState"));
        var health = rig.AddComponent<Health>();
        var bounds = rig.AddComponent<ArenaBounds>();
        Ref(bounds,"playerRoot",rig.transform);
        var dash = rig.AddComponent<DashController>();
        Ref(dash,"playerRoot",rig.transform);
        Ref(dash,"viewTransform",camera.transform);
        Ref(dash,"arenaBounds",bounds);
        var input = ScriptableObject.CreateInstance<InputActionAsset>();
        var map = input.AddActionMap("Movement");
        var action = map.AddAction("Dash",InputActionType.Button);
        action.AddBinding("<XRController>{LeftHand}/primaryButton");
        action.AddBinding("<Keyboard>/space");
        File.WriteAllText(Root + "Daryl/Movement.inputactions",input.ToJson());
        AssetDatabase.ImportAsset(Root + "Daryl/Movement.inputactions");
        var loadedInput = AssetDatabase.LoadAssetAtPath<InputActionAsset>(Root+"Daryl/Movement.inputactions");
        var reference = InputActionReference.Create(loadedInput.FindAction("Movement/Dash",true));
        AssetDatabase.CreateAsset(reference,Root+"Daryl/DashAction.asset");
        Ref(dash,"dashAction",reference);
        var head = Child("Head Hurtbox",camera.transform,Vector3.zero);
        head.AddComponent<SphereCollider>().radius=0.16f;
        head.GetComponent<Collider>().isTrigger=true;
        var headHit = head.AddComponent<Hurtbox>();
        Ref(headHit,"health",health);
        var body = Child("Body Hurtbox",rig.transform,new Vector3(0f,1.1f,0f));
        var capsule=body.AddComponent<CapsuleCollider>(); capsule.height=0.9f; capsule.radius=0.22f; capsule.isTrigger=true;
        var bodyHit=body.AddComponent<Hurtbox>(); Ref(bodyHit,"health",health);
        var follow=body.AddComponent<BodyHurtboxFollow>(); Ref(follow,"head",camera.transform);
        var canvasObj = new GameObject("Health HUD",typeof(RectTransform),typeof(Canvas));
        canvasObj.transform.SetParent(camera.transform,false);
        var canvas=canvasObj.GetComponent<Canvas>(); canvas.renderMode=RenderMode.WorldSpace;
        var canvasRect=(RectTransform)canvasObj.transform;
        canvasRect.sizeDelta=new Vector2(300,50); canvasRect.localPosition=new Vector3(0f,-0.25f,1f); canvasRect.localScale=Vector3.one*0.0015f;
        var sliderObj=new GameObject("Health",typeof(RectTransform),typeof(Image),typeof(Slider));
        sliderObj.transform.SetParent(canvasObj.transform,false);
        var sliderRect=(RectTransform)sliderObj.transform; sliderRect.sizeDelta=new Vector2(280,24);
        sliderObj.GetComponent<Image>().color=new Color(0.15f,0.15f,0.15f);
        var fillObj=new GameObject("Fill",typeof(RectTransform),typeof(Image)); fillObj.transform.SetParent(sliderObj.transform,false);
        var fillRect=(RectTransform)fillObj.transform; fillRect.anchorMin=Vector2.zero; fillRect.anchorMax=Vector2.one; fillRect.offsetMin=Vector2.zero; fillRect.offsetMax=Vector2.zero;
        fillObj.GetComponent<Image>().color=new Color(0.2f,0.9f,0.5f);
        var slider=sliderObj.GetComponent<Slider>(); slider.fillRect=fillRect; slider.value=1f; slider.interactable=false;
        var bar=sliderObj.AddComponent<HealthBar>(); Ref(bar,"health",health); Ref(bar,"slider",slider);
        SavePrefab(rig,"Daryl");
        foreach(var entry in new[]{"Giovanni_ElementalCombat","Suhaas_CombosUltimates","Ethan_MultiplayerVersus"})
        {
            var obj=new GameObject(entry);
            SavePrefab(obj,entry.Split('_')[0]);
        }
        ConfigureXR();
        var scenePath=Root+"Shared/Scenes/ElementalArena.unity";
        EditorSceneManager.SaveScene(scene,scenePath);
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(scenePath,true)};
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        Debug.Log("ELEMENTAL_SCAFFOLD_CREATED");
    }
    static void ConfigureXR()
    {
        var perTarget=ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
        AssetDatabase.CreateAsset(perTarget,Root+"Shared/Settings/XRGeneralSettings.asset");
        EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey,perTarget,true);
        foreach(var target in new[]{BuildTargetGroup.Standalone,BuildTargetGroup.Android})
        {
            perTarget.CreateDefaultSettingsForBuildTarget(target);
            perTarget.CreateDefaultManagerSettingsForBuildTarget(target);
            XRPackageMetadataStore.AssignLoader(perTarget.ManagerSettingsForBuildTarget(target),"UnityEngine.XR.OpenXR.OpenXRLoader",target);
            var xr=OpenXRSettings.GetSettingsForBuildTargetGroup(target);
            if(xr!=null)
            {
                var touch=xr.GetFeature<OculusTouchControllerProfile>();
                if(touch!=null)touch.enabled=true;
                EditorUtility.SetDirty(xr);
            }
        }
        EditorUtility.SetDirty(perTarget);
    }
    public static void FinalizeAndValidate()
    {
        EditorBuildSettings.RemoveConfigObject("com.unity.input.settings.actions");
        string prefabPath=Root+"Daryl/Prefabs/Daryl_MovementRig.prefab";
        var rig=PrefabUtility.LoadPrefabContents(prefabPath);
        var driver=rig.GetComponentInChildren<TrackedPoseDriver>();
        driver.trackingStateInput=new InputActionProperty(new InputAction("Head Tracking",InputActionType.Value,"<XRHMD>/trackingState"));
        PrefabUtility.SaveAsPrefabAsset(rig,prefabPath);
        PrefabUtility.UnloadPrefabContents(rig);
        foreach(var target in new[]{BuildTargetGroup.Standalone,BuildTargetGroup.Android})
        {
            var xr=OpenXRSettings.GetSettingsForBuildTargetGroup(target);
            var touch=xr.GetFeature<OculusTouchControllerProfile>();
            if(touch==null)throw new System.InvalidOperationException("Missing Oculus Touch profile for "+target);
            touch.enabled=true;
            if(target==BuildTargetGroup.Android)
                foreach(var feature in xr.GetFeatures<UnityEngine.XR.OpenXR.Features.OpenXRFeature>())
                    if(feature.GetType().Name=="MetaQuestFeature")feature.enabled=true;
            EditorUtility.SetDirty(xr);
        }
        var scene=EditorSceneManager.OpenScene(Root+"Shared/Scenes/ElementalArena.unity");
        int prefabCount=0;
        foreach(var obj in scene.GetRootGameObjects())
        {
            if(PrefabUtility.IsAnyPrefabInstanceRoot(obj))prefabCount++;
            foreach(var child in obj.GetComponentsInChildren<Transform>(true))
                if(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject)!=0)throw new System.Exception("Missing script on "+child.name);
        }
        if(prefabCount!=4)throw new System.Exception("Expected four prefab roots, found "+prefabCount);
        if(Object.FindFirstObjectByType<Health>()==null)throw new System.Exception("Missing player health");
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("ELEMENTAL_VALIDATED_FOUR_PREFABS");
    }
}
