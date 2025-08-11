using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Gun : MonoBehaviour
{
    public string WeaponType = "GUN"; 
    public Sprite gunImage;

    public int damage = 10;
    public float range = 100f;

    public float fireRate = 15f;

    public int reloadBulletCount; // 총알 재정전 개수.
    public int currentBulletCount; // 현재 탄알집에 남아있는 총알의 개수.
    public int carryBulletCount; // 현재 소유하고 있는 총알 개수.
    public float reloadTime = 2.05f;

    public Vector3 originPos;

    public ParticleSystem muzzleFlash;
    public Animator anim;
}
