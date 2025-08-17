using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public List<Collider> colliderList = new List<Collider>(); // 충돌한 오브젝트들 저장할 리스트

    [SerializeField]
    private int layerGround; // 지형 레이어 (무시하게 할 것)
    private const int IGNORE_RAYCAST_LAYER = 2;  // ignore_raycast (무시하게 할 것)
    public bool israycastHit;

    [SerializeField]
    private Material green;
    [SerializeField]
    private Material red;


    void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        //건물을 설치할 위치가 감지되고  설치를 방해할 오브젝트가 감지되지 않았을 때 
        if (colliderList.Count == 0 && israycastHit)
            SetColor(green);
        else
            SetColor(red);
    }

    private void SetColor(Material mat)
    {
        foreach (Transform tf_Child in this.transform)
        {
            Material[] newMaterials = new Material[tf_Child.GetComponent<Renderer>().materials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = mat;
            }

            tf_Child.GetComponent<Renderer>().materials = newMaterials;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
        {
            colliderList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
        {
            colliderList.Remove(other);
        }
         
    }

    public bool IsBuildable()
    {
        //건물을 설치할 위치가 감지되고  설치를 방해할 오브젝트가 감지되지 않았을 때 
        //설치 가능하게 true를 반환
        return (colliderList.Count == 0 && israycastHit);
    }
}
