using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class MovementTests
{
    GameObject root;
    static void Set(object target,string name,object value) { target.GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Instance).SetValue(target,value); }
    [SetUp] public void Setup() { root=new GameObject("Test Player"); }
    [TearDown] public void Cleanup() { Object.DestroyImmediate(root); }
    [Test] public void HealthClampsAndKOOnlyFiresOnce()
    {
        var health=root.AddComponent<Health>(); int kos=0; health.OnKO.AddListener(()=>kos++);
        health.TakeDamage(25); Assert.AreEqual(75,health.CurrentHealth);
        health.Heal(200); Assert.AreEqual(100,health.CurrentHealth);
        health.TakeDamage(200); health.TakeDamage(20); Assert.AreEqual(0,health.CurrentHealth); Assert.AreEqual(1,kos);
        health.Heal(10); Assert.AreEqual(0,health.CurrentHealth);
        health.ResetHealth(); Assert.AreEqual(100,health.CurrentHealth);
    }
    [Test] public void InvalidDamageDoesNotChangeHealthOrEmitHit()
    {
        var health=root.AddComponent<Health>(); root.AddComponent<SphereCollider>(); var hit=root.AddComponent<Hurtbox>(); int hits=0; hit.OnHit.AddListener((d,p)=>hits++);
        foreach(var damage in new[]{0f,-1f,float.NaN,float.PositiveInfinity})hit.ApplyHit(damage,Vector3.zero);
        Assert.AreEqual(100,health.CurrentHealth); Assert.AreEqual(0,hits);
    }
    [Test] public void HitCarriesDamageAndPosition()
    {
        var health=root.AddComponent<Health>();root.AddComponent<SphereCollider>(); var hit=root.AddComponent<Hurtbox>();float amount=0;Vector3 point=Vector3.zero;
        hit.OnHit.AddListener((d,p)=>{amount=d;point=p;});hit.ApplyHit(30,Vector3.one);
        Assert.AreEqual(70,health.CurrentHealth);Assert.AreEqual(30,amount);Assert.AreEqual(Vector3.one,point);
    }
    [UnityTest] public IEnumerator BoundsClampXZAndPreserveHeight()
    {
        var bounds=root.AddComponent<ArenaBounds>();Set(bounds,"playerRoot",root.transform);
        root.transform.position=new Vector3(20,2,-20);yield return null;
        Assert.AreEqual(new Vector3(5,2,-5),root.transform.position);
    }
    [UnityTest] public IEnumerator DashMovesAndCooldownRejectsSecondDash()
    {
        var dash=root.AddComponent<DashController>();Set(dash,"playerRoot",root.transform);Set(dash,"viewTransform",root.transform);Set(dash,"dashDuration",0.05f);
        dash.TryDash();yield return new WaitForSeconds(0.15f);
        Assert.That(root.transform.position.z,Is.EqualTo(2).Within(0.01));dash.TryDash();yield return new WaitForSeconds(0.1f);
        Assert.That(root.transform.position.z,Is.EqualTo(2).Within(0.01));
    }
    [UnityTest] public IEnumerator DisableCancelsDashAndAllowsAnotherAfterCooldown()
    {
        var dash=root.AddComponent<DashController>();Set(dash,"playerRoot",root.transform);Set(dash,"viewTransform",root.transform);Set(dash,"cooldown",0f);
        dash.TryDash();yield return null;dash.enabled=false;var stopped=root.transform.position;yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(stopped,root.transform.position);dash.enabled=true;dash.TryDash();yield return new WaitForSeconds(0.3f);
        Assert.That(root.transform.position.z,Is.EqualTo(stopped.z+2).Within(0.01));
    }
    [UnityTest] public IEnumerator DashDoesNotLeaveArena()
    {
        var bounds=root.AddComponent<ArenaBounds>();Set(bounds,"playerRoot",root.transform);
        var dash=root.AddComponent<DashController>();Set(dash,"playerRoot",root.transform);Set(dash,"viewTransform",root.transform);Set(dash,"arenaBounds",bounds);
        root.transform.position=new Vector3(0,0,4.5f);dash.TryDash();yield return new WaitForSeconds(0.3f);
        Assert.That(root.transform.position.z,Is.EqualTo(5).Within(0.01));
    }
    [UnityTest] public IEnumerator HealthBarUsesAssignedPlayer()
    {
        var health=root.AddComponent<Health>();var ui=new GameObject("UI",typeof(RectTransform));ui.transform.SetParent(root.transform);ui.SetActive(false);
        var slider=ui.AddComponent<Slider>();var bar=ui.AddComponent<HealthBar>();Set(bar,"health",health);Set(bar,"slider",slider);ui.SetActive(true);
        yield return null;health.TakeDamage(25);Assert.That(slider.value,Is.EqualTo(0.75f).Within(0.001));
    }
    [UnityTest] public IEnumerator BodyFollowsHeadPositionButStaysUpright()
    {
        var head=new GameObject("Head");head.transform.SetParent(root.transform);head.transform.position=new Vector3(1,1.5f,2);head.transform.rotation=Quaternion.Euler(30,90,20);
        var body=new GameObject("Body");body.transform.SetParent(root.transform);var follow=body.AddComponent<BodyHurtboxFollow>();Set(follow,"head",head.transform);
        yield return null;Assert.That(body.transform.position.y,Is.EqualTo(0.95f).Within(0.001));Assert.That(Vector3.Dot(body.transform.up,Vector3.up),Is.GreaterThan(0.999f));
    }
#if UNITY_EDITOR
    [UnityTest] public IEnumerator GeneratedPrefabRespondsToKeyboardDashBinding()
    {
        var previousBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;
        var previousBackground=InputSystem.settings.backgroundBehavior;
        var previousRunInBackground=Application.runInBackground;
        InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        Application.runInBackground=true;
        Object.DestroyImmediate(root);
        root=Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ElementalFighter/Daryl/Prefabs/Daryl_MovementRig.prefab"));
        var keyboard=InputSystem.AddDevice<Keyboard>();
        try
        {
            yield return null;
            var before=root.transform.position;
            var dash=root.GetComponent<DashController>();
            var actionReference=(InputActionReference)typeof(DashController).GetField("dashAction",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(dash);
            Assert.NotNull(actionReference,"Dash reference");
            Assert.NotNull(actionReference.action,"Resolved dash action");
            var activeAction=(InputAction)typeof(DashController).GetField("activeDashAction",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(dash);
            Assert.IsTrue(activeAction.enabled,"Dash action enabled");
            bool performed=false;
            activeAction.performed+=context=>performed=true;
            Assert.That(activeAction.controls.Count,Is.GreaterThan(0),"Dash resolved controls");
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));
            InputSystem.Update();
            yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());
            InputSystem.Update();
            yield return new WaitForSeconds(0.3f);
            Assert.IsTrue(performed,"Keyboard binding performed");
            Assert.That(root.transform.position.z,Is.EqualTo(before.z+2f).Within(0.01));
            Assert.AreEqual(100,root.GetComponent<Health>().CurrentHealth);
            Assert.AreEqual(2,root.GetComponentsInChildren<Hurtbox>().Length);
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
            InputSystem.settings.editorInputBehaviorInPlayMode=previousBehavior;
            InputSystem.settings.backgroundBehavior=previousBackground;
            Application.runInBackground=previousRunInBackground;
        }
    }
#endif
}
