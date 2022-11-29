using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UMM;

[ModMetaData("Custom Arms", "1.0.0", "Custom arms!", false, true)]
public class CustomArmMod : UKMod
{
    private static Harmony harmony;
    public override void OnModEnabled()
    {
        Debug.Log("Starting custom arms");
        harmony = new Harmony("tempy.customArms");
        harmony.PatchAll();
        StartCoroutine(LoadStockPrefabs());
    }

    public override void OnModDisabled()
    {
        CustomArmController.UnloadArms();
        harmony.UnpatchSelf();
        base.OnModDisabled();
    }

    public IEnumerator LoadStockPrefabs()
    {
        // Parallel go brrrrrrrrr
        AssetBundleRequest snakeRequest = UKAPI.LoadCommonAssetAsync("ProjectileMinosPrime.prefab");
        AssetBundleRequest minosChargeRequest = UKAPI.LoadCommonAssetAsync("MinosProjectileCharge.prefab");
        AssetBundleRequest gabeThrownSpearRequest = UKAPI.LoadCommonAssetAsync("GabrielThrownSpear.prefab");
        AssetBundleRequest gabeBreakRequest = UKAPI.LoadCommonAssetAsync("GabrielWeaponBreak.prefab");
        AssetBundleRequest zweiRequest = UKAPI.LoadCommonAssetAsync("GabrielZweihander.prefab");
        AssetBundleRequest fireRequest = UKAPI.LoadCommonAssetAsync("Fire.prefab");
        AssetBundleRequest chargeRequest = UKAPI.LoadCommonAssetAsync("ProjectileDecorative 2.prefab");
        AssetBundleRequest virtueRequest = UKAPI.LoadCommonAssetAsync("VirtueInsignia.prefab");

        yield return snakeRequest;
        if (snakeRequest.asset == null)
            Debug.LogError("Couldn't load the snake projectile");
        else
            CustomArmController.minosSnakeProjectilePrefab = snakeRequest.asset as GameObject;

        yield return minosChargeRequest;
        if (minosChargeRequest.asset == null)
            Debug.LogError("Couldn't load minos's charge");
        else
            CustomArmController.minosChargePrefab = minosChargeRequest.asset as GameObject;

        yield return gabeThrownSpearRequest;
        if (gabeThrownSpearRequest.asset == null)
            Debug.LogError("Couldn't load the thrown gabe spear");
        else
            CustomArmController.gabeSpearThrownPrefab = gabeThrownSpearRequest.asset as GameObject;

        yield return gabeBreakRequest;
        if (gabeBreakRequest.asset == null)
            Debug.LogError("Couldn't load the gabe break");
        else
            CustomArmController.gabeBreakPrefab = gabeBreakRequest.asset as GameObject;

        yield return zweiRequest;
        if (zweiRequest.asset == null)
            Debug.LogError("Couldn't load the zwei");
        else
            CustomArmController.gabeZweihanderPrefab = zweiRequest.asset as GameObject;

        yield return fireRequest;
        if (fireRequest.asset == null)
            Debug.LogError("Couldn't load the fire prefab");
        else
            CustomArmController.firePrefab = fireRequest.asset as GameObject;

        yield return chargeRequest;
        if (chargeRequest.asset == null)
            Debug.LogError("Couldn't load the charge projectile prefab");
        else
            CustomArmController.chargeProjectilePrefab = chargeRequest.asset as GameObject;

        yield return virtueRequest;
        if (virtueRequest.asset == null)
            Debug.LogError("Couldn't load the virtue charge prefab");
        else
            CustomArmController.virtueChargePrefab = virtueRequest.asset as GameObject;

        CustomArmController.LoadStockArms();
        yield break;
    }
}

public static class CustomArmController
{
    public static GameObject currentFistObject; // like if you want v1 holding a zweihander for example
    private static Dictionary<int, CustomArmInfo> allArms = new Dictionary<int, CustomArmInfo>();
    private static Dictionary<int, CustomArmInfo> allBlueArms = new Dictionary<int, CustomArmInfo>();
    private static Dictionary<int, CustomArmInfo> allRedArms = new Dictionary<int, CustomArmInfo>();

    public static GameObject minosSnakeProjectilePrefab;
    public static GameObject minosChargePrefab;
    public static GameObject gabeSpearThrownPrefab;
    public static GameObject gabeZweihanderPrefab;
    public static GameObject gabeBreakPrefab;
    public static GameObject firePrefab;
    public static GameObject chargeProjectilePrefab;
    public static GameObject virtueChargePrefab;

    public static CustomArmInfo currentArm;
    public static int blueArmVariations;
    public static int redArmVariations;
    public static int currentVariation = -1;

    public static void LoadStockArms()
    {
        if (minosSnakeProjectilePrefab && minosChargePrefab)
        {
            CustomArmInfo minosMultiArm = new CustomArmInfo();
            minosMultiArm.canUseDefaultAlt = false;
            minosMultiArm.armColor = new Color32(200, 200, 255, 255);

            int snakesToFire = 1;
            IEnumerator SwingRoutine(Punch punch)
            {
                Animator anim = punch.GetComponent<Animator>();
                if (anim.speed == 0)
                    anim.speed = 1;
                float speed = anim.speed;
                anim.speed = 0f;

                currentFistObject = GameObject.Instantiate(minosChargePrefab, punch.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find("Holder (1)"));
                currentFistObject.transform.localPosition = new Vector3(-0.21f, 0.064f, -0.017f);
                currentFistObject.transform.localEulerAngles = Vector3.zero;
                currentFistObject.transform.localScale = new Vector3(5f, 5f, 5f);
                float dt = 0f;
                while (dt < 0.5f)
                {
                    if (InputManager.Instance.InputSource.ChangeFist.IsPressed || !InputManager.Instance.InputSource.Punch.IsPressed || currentArm != minosMultiArm)
                    {
                        anim.speed = speed;
                        yield break;
                    }
                    dt += Time.deltaTime;
                    yield return null;
                }

                List<EnemyIdentifier> identifiers = EnemyTracker.Instance.GetCurrentEnemies();
                for (int i = 1; i <= snakesToFire; i++)
                {
                    GameObject newSnake = GameObject.Instantiate<GameObject>(minosSnakeProjectilePrefab, punch.transform.position + (2f * punch.transform.forward), Quaternion.identity);
                    Projectile projectile = newSnake.GetComponentInChildren<Projectile>();
                    projectile.playerBullet = true;
                    projectile.friendly = true;
                    projectile.damage = 4f;
                    projectile.undeflectable = false;
                    projectile.homingType = HomingType.None;
                    if (identifiers.Count > 0)
                    {
                        int toUse = i;
                        while (toUse >= identifiers.Count)
                            toUse = toUse - identifiers.Count;
                        EnemyIdentifier identifier = identifiers[toUse];
                        if (identifier != null)
                        {
                            foreach (Collider collider in identifier.GetComponentsInChildren<Collider>())
                                if (projectile.target == null || collider.transform.position.y > projectile.target.position.y)
                                {
                                    projectile.target = collider.transform;
                                    projectile.homingType = HomingType.Gradual;
                                }
                        }
                    }
                    newSnake.transform.SetParent(punch.transform);
                    newSnake.transform.localEulerAngles = Vector3.zero;
                    float angle = UnityEngine.Random.Range(2f, 5f);
                    int numpadIdx = i;
                    if (numpadIdx > 9)
                        numpadIdx = numpadIdx - 9;
                    if (CameraFrustumTargeter.Instance.CurrentTarget)
                        newSnake.transform.LookAt(CameraFrustumTargeter.Instance.CurrentTarget.bounds.center);
                    else
                        newSnake.transform.rotation = CameraController.Instance.transform.rotation;
                    if (numpadIdx == 1 || numpadIdx == 4 || numpadIdx == 7)
                        newSnake.transform.localEulerAngles += new Vector3(1 - angle, 0, 0);
                    angle = UnityEngine.Random.Range(2f, 5f);
                    if (numpadIdx == 1 || numpadIdx == 2 || numpadIdx == 3)
                        newSnake.transform.localEulerAngles += new Vector3(0, 1 - angle, 0);
                    angle = UnityEngine.Random.Range(2f, 5f);
                    if (numpadIdx == 3 || numpadIdx == 6 || numpadIdx == 9)
                        newSnake.transform.localEulerAngles += new Vector3(angle, 0, 0);
                    angle = UnityEngine.Random.Range(2f, 5f);
                    if (numpadIdx == 7 || numpadIdx == 8 || numpadIdx == 9)
                        newSnake.transform.localEulerAngles += new Vector3(0, angle, 0);
                    newSnake.transform.SetParent(null);
                }
                if (snakesToFire > 10)
                    StyleHUD.Instance.AddPoints(100 * (snakesToFire % 10), "<color=#C8C8FF >MULTI SERPENT</color>"); // for example, if i fire 20-29 snakes then it'll add 200 points, cuz remainder moment
                snakesToFire = 1;

                anim.speed = speed;
                yield break;
            }

            minosMultiArm.onSwing.AddListener(delegate (Punch punch, bool hitSomething)
            {
                FistControl.Instance.StartCoroutine(SwingRoutine(punch));
            });


            minosMultiArm.onHit.AddListener(delegate (Punch punch, Vector3 hit, Transform target)
            {
                EnemyIdentifierIdentifier identifier = target.GetComponent<EnemyIdentifierIdentifier>();
                if (identifier && identifier.eid.dead)
                    snakesToFire += 2;
            });

            AddArmInfo(minosMultiArm);
        }

        if (gabeZweihanderPrefab && gabeSpearThrownPrefab && firePrefab)
        {
            CustomArmInfo gabeArm = new CustomArmInfo();
            float parriedDamage = 0;
            const float damageThreshold = 35f;

            gabeArm.canUseDefaultAlt = true;
            gabeArm.armColor = new Color32(255, 255, 144, 255);
            gabeArm.onEquip.AddListener(delegate (FistControl fist)
            {
                currentFistObject = GameObject.Instantiate(gabeZweihanderPrefab, fist.currentArmObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find("Holder (1)"));
                currentFistObject.transform.localPosition = new Vector3(-0.163f, -0.011f, -0.071f);
                currentFistObject.transform.localEulerAngles = new Vector3(-58.065f, -29.183f, 3.885f);
                currentFistObject.transform.localScale = Vector3.one * 0.54699f;
                if (parriedDamage >= damageThreshold && !currentFistObject.transform.Find("Fire boi"))
                {
                    GameObject fire = GameObject.Instantiate(firePrefab, currentFistObject.transform);
                    fire.transform.localPosition = new Vector3(0f, 0f, 5.87f);
                    fire.transform.localEulerAngles = new Vector3(180f, 90f, 90f);
                    fire.name = "Fire Boi";
                }
            });
            gabeArm.onSwing.AddListener(delegate (Punch punch, bool hitSomething)
            {
                if (parriedDamage < damageThreshold)
                    return;
                GameObject newProjectile = null;
                newProjectile = GameObject.Instantiate<GameObject>(gabeSpearThrownPrefab, punch.transform.position + (2f * punch.transform.forward), CameraController.Instance.transform.rotation);
                if (CameraFrustumTargeter.Instance.CurrentTarget)
                    newProjectile.transform.LookAt(CameraFrustumTargeter.Instance.CurrentTarget.bounds.center);
                foreach (Projectile projectile in newProjectile.GetComponentsInChildren<Projectile>(true))
                {
                    projectile.friendly = true;
                    projectile.playerBullet = true;
                    projectile.undeflectable = false;
                    projectile.damage = damageThreshold / 3f;
                    foreach (Explosion explosion in projectile.GetComponentsInChildren<Explosion>(true))
                        explosion.damage = (int)(damageThreshold / 3f);
                    projectile.homingType = HomingType.None;
                }

                Traverse.Create(punch).Field("alreadyBoostedProjectile").SetValue(true);
                parriedDamage = 0;
                GameObject.Destroy(currentFistObject.transform.Find("Fire Boi").gameObject);

                GameObject newBreak = GameObject.Instantiate(gabeBreakPrefab, punch.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find("Holder (1)"));
                newBreak.transform.position = new Vector3(-0.163f, -0.011f, -0.071f);
                newBreak.transform.GetChild(0).localScale *= 2.5f;
                newBreak.transform.SetParent(null);
            });
            gabeArm.onHit.AddListener(delegate (Punch punch, Vector3 hit, Transform target)
            {
                GameObject newBreak = GameObject.Instantiate(gabeBreakPrefab, punch.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find("Holder (1)"));
                newBreak.transform.SetParent(null);
                newBreak.transform.position = hit;
                EnemyIdentifierIdentifier identifier = target.GetComponent<EnemyIdentifierIdentifier>();
                if (identifier)
                {
                    identifier.eid.DeliverDamage(identifier.gameObject, punch.transform.forward * 4500, hit, parriedDamage / 3f, true, 0f);
                }
                newBreak.transform.GetChild(0).localScale *= 2.5f;
            });
            gabeArm.onParry.AddListener(delegate (Punch punch, Projectile proj)
            {
                proj.playerBullet = true;
                parriedDamage += proj.damage;
                if (proj.damage >= damageThreshold)
                    StyleHUD.Instance.AddPoints(250, "<color=yellow>INSTA-CHARGE</color>");
                proj.gameObject.SetActive(false);
                GameObject newSword = GameObject.Instantiate(gabeZweihanderPrefab);
                Transform newBreak = newSword.transform.Find("GabrielWeaponBreak");
                newBreak.SetParent(null);
                newBreak.position = proj.transform.position;
                newBreak.GetChild(0).localScale *= 2.5f;
                GameObject.Destroy(newSword);
                if (parriedDamage >= damageThreshold && !currentFistObject.transform.Find("Fire boi"))
                {
                    GameObject fire = GameObject.Instantiate(firePrefab, currentFistObject.transform);
                    fire.transform.localPosition = new Vector3(0f, 0f, 5.87f);
                    fire.transform.localEulerAngles = new Vector3(180f, 90f, 90f);
                    fire.name = "Fire Boi";
                }
            });
            AddArmInfo(gabeArm);
        }

        if (chargeProjectilePrefab)
        {
            CustomArmInfo vortexArm = new CustomArmInfo();
            vortexArm.canUseDefaultAlt = false;
            vortexArm.armColor = new Color32(31, 68, 156, 255);
            vortexArm.type = FistType.Heavy;
            vortexArm.onEquip.AddListener(delegate (FistControl fist)
            {
                currentFistObject = GameObject.Instantiate(chargeProjectilePrefab, fist.currentArmObject.transform.Find("Armature").Find("clavicle").Find("wrist").Find("hand").Find("Holder (1)"));
                GameObject.Destroy(currentFistObject.GetComponent<Projectile>());
                GameObject.Destroy(currentFistObject.transform.Find("Sphere").gameObject);
                currentFistObject.GetComponent<MeshRenderer>().enabled = false;
                currentFistObject.transform.Find("ChargeEffect (1)").GetComponent<MeshRenderer>().enabled = false;
                currentFistObject.transform.localPosition = Vector3.zero;
                currentFistObject.transform.localEulerAngles = Vector3.zero;
                currentFistObject.transform.localScale = Vector3.one;

                Transform particleSystem = currentFistObject.transform.Find("ChargeEffect (1)").GetChild(0);
                particleSystem.localPosition = Vector3.zero;
                particleSystem.localScale = Vector3.one * 1.25f;
                currentFistObject.SetActive(false);
            });
            vortexArm.onStartRedAlt.AddListener(delegate (Punch punch)
            {
                IEnumerator heldRoutine()
                {
                    Animator anim = punch.GetComponent<Animator>();
                    yield return null;
                    if (!InputManager.Instance.InputSource.Punch.IsPressed)
                        yield break;
                    currentFistObject.SetActive(true);
                    float speed = anim.speed;
                    GunControl.Instance.NoWeapon();
                    anim.speed = 0f;
                    ProjectileParryZone ppz = punch.transform.parent.GetComponentInChildren<ProjectileParryZone>();
                    int suckedObjects = 0;

                    while (InputManager.Instance.InputSource.Punch.IsPressed && currentArm == vortexArm)
                    {
                        if (InputManager.Instance.InputSource.ChangeFist.IsPressed)
                        {
                            anim.speed = speed;
                            break;
                        }
                        Projectile proj = ppz.CheckParryZone();
                        if (proj != null && !proj.undeflectable && !proj.playerBullet)
                        {
                            if (!vortexArm.persistentObjects.Contains(proj.gameObject))
                            {
                                vortexArm.persistentObjects.Add(proj.gameObject);
                                proj.transform.SetParent(null);
                                proj.gameObject.SetActive(false);
                                proj.playerBullet = true;
                                proj.friendly = true;
                                proj.undeflectable = false;
                                proj.homingType = HomingType.None;
                                proj.target = null;
                                proj.hittingPlayer = false;
                                suckedObjects++;
                                if (suckedObjects >= 25)
                                {
                                    suckedObjects = 0;
                                    StyleHUD.Instance.AddPoints(50, "<color=cyan>HUGE SUCK</color>");
                                }
                            }
                        }
                        yield return null;
                    }
                    GunControl.Instance.YesWeapon();
                    anim.speed = speed;
                    if (currentFistObject != null)
                        currentFistObject.SetActive(false);
                }
                FistControl.Instance.StartCoroutine(heldRoutine());
            });
            vortexArm.onSwing.AddListener(delegate (Punch punch, bool hitSomething)
            {
                if (vortexArm.persistentObjects.Count <= 0)
                    return;
                foreach (GameObject persistent in vortexArm.persistentObjects)
                {
                    if (persistent == null)
                        continue;
                    persistent.transform.position = punch.transform.position + (2f * punch.transform.forward);
                    persistent.transform.rotation = CameraController.Instance.transform.rotation;
                    if (CameraFrustumTargeter.Instance.CurrentTarget)
                        persistent.transform.LookAt(CameraFrustumTargeter.Instance.CurrentTarget.bounds.center);
                    persistent.SetActive(true);
                    foreach (Projectile projecile in persistent.GetComponentsInChildren<Projectile>(true))
                        Traverse.Create(punch).Method("ParryProjectile", new object[] { projecile }).GetValue(projecile);
                }
                vortexArm.persistentObjects = new List<GameObject>();
                TimeController.Instance.ParryFlash();
            });
            AddArmInfo(vortexArm);
        }

        if (virtueChargePrefab)
        {
            CustomArmInfo virtueArm = new CustomArmInfo();
            virtueArm.canUseDefaultAlt = false;
            virtueArm.armColor = new Color32(181, 246, 255, 255);
            virtueArm.type = FistType.Heavy;
            List<EnemyIdentifier> allVirtueMarkedIdentifiers = new List<EnemyIdentifier>();
            virtueArm.onHit.AddListener(delegate (Punch punch, Vector3 hitPoint, Transform target)
            {
                EnemyIdentifier identifier = target.gameObject.GetComponentInParent<EnemyIdentifier>();
                if (identifier && !allVirtueMarkedIdentifiers.Contains(identifier))
                    allVirtueMarkedIdentifiers.Add(identifier);
            });

            IEnumerator heldRoutine(Punch punch)
            {
                Animator anim = punch.GetComponent<Animator>();
                if (anim.speed == 0)
                    anim.speed = 1;
                yield return null;
                if (!InputManager.Instance.InputSource.Punch.IsPressed)
                    yield break;
                float speed = anim.speed;
                anim.speed = 0f;
                List<VirtueInsignia> insignias = new List<VirtueInsignia>();
                foreach (EnemyIdentifier identifier in allVirtueMarkedIdentifiers)
                {
                    if (identifier != null && !identifier.dead)
                    {
                        GameObject newVirtueCharge = GameObject.Instantiate(virtueChargePrefab, identifier.transform.position, Quaternion.identity);
                        VirtueInsignia newInsignia = newVirtueCharge.GetComponent<VirtueInsignia>();
                        newInsignia.target = identifier.transform;
                        newInsignia.noTracking = false;
                        newInsignia.predictiveVersion = null;
                        newInsignia.tag = "Moving"; // it has to be a built-in tag, so it's moving i guess
                        insignias.Add(newInsignia);
                    }
                }

                foreach (VirtueInsignia insignia in insignias)
                    insignia.damage = Math.Min(5 * insignias.Count, 25);

                float dt = 0f;
                if (insignias.Count <= 0)
                {
                    anim.speed = speed;
                    yield break;
                }

                GameObject reticlePivotInsignia = GameObject.Instantiate(new GameObject(), punch.transform.Find("Armature").Find("clavicle").Find("wrist").Find("hand").Find("Holder (1)"));
                reticlePivotInsignia.transform.localPosition = new Vector3(-0.144f, -0.035f, 0.079f);
                reticlePivotInsignia.transform.localEulerAngles = new Vector3(0, -90f, 45f);
                reticlePivotInsignia.transform.localScale = new Vector3(0.35024f, 0.17512f, 0.35024f);
                GameObject reticleInsignia = GameObject.Instantiate(virtueChargePrefab, reticlePivotInsignia.transform); // a pivot is required so the reticle doesn't do "funky stuff" when spinning
                reticleInsignia.transform.localScale = Vector3.one;
                reticleInsignia.GetComponent<VirtueInsignia>().target = reticlePivotInsignia.transform;


                while (dt < 0.95f && InputManager.Instance.InputSource.Punch.IsPressed && currentArm == virtueArm)
                {
                    if (InputManager.Instance.InputSource.ChangeFist.IsPressed)
                    {
                        anim.speed = speed;
                        yield break;
                    }
                    dt += Time.deltaTime;
                    yield return null;
                }

                GameObject.Destroy(reticlePivotInsignia);
                GameObject.Destroy(reticleInsignia);
                anim.speed = speed;
                if (dt < 0.95f)
                {
                    for (int i = 0; i < insignias.Count; i++)
                    {
                        if (insignias[i] != null)
                            GameObject.Destroy(insignias[i].gameObject);
                    }
                }
                yield break;
            }

            virtueArm.onStartRedAlt.AddListener(delegate (Punch punch)
            {
                FistControl.Instance.StartCoroutine(heldRoutine(punch));
            });
            AddArmInfo(virtueArm);
        }

        //CustomArmInfo pushyArm = new CustomArmInfo();
        //pushyArm.canUseDefaultAlt = true;
        //pushyArm.armColor = new Color32(176, 11, 105, 255);
        //pushyArm.type = FistType.Heavy;
        //pushyArm.onStartRedAlt.AddListener(delegate (Punch punch)
        //{
        //    NewMovement.Instance.rb.velocity -= 100f * punch.transform.forward;
        //});
        //AddArmInfo(pushyArm);   
    }

    public static void UnloadArms()
    {
        GameObject.Destroy(currentFistObject);
        allArms = new Dictionary<int, CustomArmInfo>();
        allBlueArms = new Dictionary<int, CustomArmInfo>();
        allRedArms = new Dictionary<int, CustomArmInfo>();
        currentVariation = -1;
    }

    public static void AddArmInfo(CustomArmInfo info)
    {
        if (info.type == FistType.Standard)
        {
            info.variationNumber = blueArmVariations;
            allBlueArms.Add(info.variationNumber, info);
            blueArmVariations++;
        }
        else
        {
            info.variationNumber = redArmVariations;
            allRedArms.Add(info.variationNumber, info);
            redArmVariations++;
        }
        allArms.Add(info.variationNumber + (info.type == FistType.Standard ? 0 : 2048), info); // this solution will work for now, but the day we have 2048 blue arms it won't
    }

    public class CustomArmInfo
    {
        public bool canUseDefaultAlt;
        public Color armColor;
        public List<GameObject> persistentObjects = new List<GameObject>();

        public int variationNumber;
        public FistType type;
        public ArmEquipEvent onEquip = new ArmEquipEvent();
        public ArmEvent onDestroy = new ArmEvent();
        public ArmSwingEvent onSwing = new ArmSwingEvent();
        public ArmHitEvent onHit = new ArmHitEvent();
        public ArmEvent onStartRedAlt = new ArmEvent();
        public ArmParryEvent onParry = new ArmParryEvent();

        public CustomArmInfo() { }
        public class ArmEvent : UnityEvent<Punch> { }
        public class ArmSwingEvent : UnityEvent<Punch, bool> { }
        public class ArmEquipEvent : UnityEvent<FistControl> { }
        public class ArmHitEvent : UnityEvent<Punch, Vector3, Transform> { }
        public class ArmParryEvent : UnityEvent<Punch, Projectile> { }
    }

    #region HARMONY_PATCHES

    // The only reason these are classes is because that's what I'm used to, yes I know it's weird but sorry

    [HarmonyPatch(typeof(FistControl), nameof(FistControl.ArmChange))]
    public static class Inject_CustomArms
    {
        public static void Prefix(ref int orderNum)
        {
            if (orderNum == 1)
            {
                if (currentVariation + 1 < blueArmVariations)
                    orderNum = 0;
                else
                    currentVariation = -2;
            }
            else if (orderNum == 0)
            {
                if (currentVariation + 1 < redArmVariations)
                    orderNum = 1;
                else
                    currentVariation = -2;
            }
        }

        public static void Postfix(int orderNum, FistControl __instance)
        {
            if (currentFistObject != null)
                GameObject.Destroy(currentFistObject);

            if (orderNum == 0)
            {
                currentVariation++;
                if (currentVariation + 1 > blueArmVariations)
                {
                    currentVariation = -1;
                    currentArm = null;
                }
                else
                {
                    if (allBlueArms.ContainsKey(currentVariation))
                    {
                        currentArm = allBlueArms[currentVariation];
                        currentArm.onEquip.Invoke(__instance);
                        __instance.fistIcon.color = currentArm.armColor;
                    }
                }
            }
            else
            {
                currentVariation++;
                if (currentVariation + 1 > redArmVariations)
                {
                    currentVariation = -1;
                    currentArm = null;
                }
                else
                {
                    if (allRedArms.ContainsKey(currentVariation))
                    {
                        currentArm = allRedArms[currentVariation];
                        currentArm.onEquip.Invoke(__instance);
                        __instance.fistIcon.color = currentArm.armColor;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Punch), "CheckForProjectile")]
    public static class Ensure_CorrectParry
    {
        public static bool Prefix(ref bool __result)
        {
            if (currentArm == null || currentVariation == -1)
                return true;
            __result = currentArm.canUseDefaultAlt;
            return __result;
        }
    }

    [HarmonyPatch(typeof(Punch), "ParryProjectile")]
    public static class Ensure_CorrectParryProjectile
    {
        public static bool Prefix()
        {
            return currentArm == null || currentVariation == -1 || currentArm.canUseDefaultAlt;
        }

        public static void Postfix(Projectile proj, Punch __instance)
        {
            if (currentArm != null && currentArm.canUseDefaultAlt)
            {
                currentArm.onParry.Invoke(__instance, proj);
            }
        }
    }

    [HarmonyPatch(typeof(Punch), "BlastCheck")]
    public static class Ensure_ShockWaveHeld
    {
        public static bool Prefix(Punch __instance)
        {
            if (currentArm != null)
                currentArm.onStartRedAlt.Invoke(__instance);
            return currentArm == null || currentArm.canUseDefaultAlt;
        }
    }

    [HarmonyPatch(typeof(Punch), "Start")]
    public static class Ensure_MinosCustomArm
    {
        public static void Postfix()
        {
            currentVariation = -1;
            currentArm = null;
            if (currentFistObject)
                GameObject.Destroy(currentFistObject);
            foreach (CustomArmInfo info in allArms.Values)
            {
                if (info.persistentObjects != null)
                    foreach (GameObject go in info.persistentObjects)
                        if (go != null)
                            GameObject.Destroy(go); // end end end end 
                info.persistentObjects = new List<GameObject>();
            }
        }
    }

    [HarmonyPatch(typeof(Punch), "PunchStart")]
    public static class Inject_CustomArmsPunch
    {
        public static void Postfix(Punch __instance)
        {
            if (currentArm != null)
                currentArm.onSwing.Invoke(__instance, (bool)Traverse.Create(__instance).Field("hitSomething").GetValue());
        }
    }

    [HarmonyPatch(typeof(Punch), "PunchSuccess")]
    public static class Inject_CustomArmsHit
    {
        public static void Postfix(Punch __instance, Vector3 point, Transform target)
        {
            if (currentArm != null)
                currentArm.onHit.Invoke(__instance, point, target);
        }
    }

    [HarmonyPatch(typeof(VirtueInsignia), "OnTriggerEnter")]
    public static class Ensure_InsigniaEnemiesDamaged
    {
        public static bool Prefix(VirtueInsignia __instance, Collider other)
        {
            if (__instance.CompareTag("Moving")) // tfw you can only use built in tags
            {
                other.GetComponentInParent<EnemyIdentifier>()?.DeliverDamage(other.GetComponentInParent<EnemyIdentifier>().gameObject, __instance.transform.up * 4500, other.transform.position, __instance.damage, true, 0f);
                __instance.gameObject.tag = "Untagged"; // this is to make it not deal continuos damage
                return false;
            }
            return true;
        }
    }
    #endregion
}