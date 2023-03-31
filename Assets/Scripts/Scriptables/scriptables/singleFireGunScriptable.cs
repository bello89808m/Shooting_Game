using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using player;

[CreateAssetMenu(fileName = "gunSettings", menuName = "scriptables")]
public class singleFireGunScriptable : ScriptableObject
{
    [Header("Gun Settings")]
    public string gunName;
    public GameObject bullet;
    public float damage;
    public float shootDelay;

    [Header("Bullet Settings")]
    public float bulletSpeed;
    public float bulletDistance;

    [Header("Gun Recoil Settings")]
    public float recoilTargetRot;
    public float recoilTargetPush;
    public float maxPosRecoil;
    public float rotRecoilReturnSpeed;
    public float posRecoilReturnSpeed;

    [Header("Camera Recoil Settings")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float recoilReturn;
    public float snappiness;

    [Header("Sway Total")]
    public float swayXMultiplier;
    public float swayYMultiplier;
    public float swaySmooth;

    [Header("Gun Bob")]
    public float frequency;
    public float amplitude;

    [Header("Reload")]
    public int ammoMag;
    public int ammoCount;

    [Header("Gun Type")]
    public gunType guntype;

    public enum gunType
    {
        fullAuto,
        semiAuto,
    }
}
