using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public GameObject go_prefab; // 실제 설치 될 프리
    public GameObject go_PreviewPrefab; // 미리 보기 프리팹                                 
    public int craftMoney; //블록의 가격
}

public class BuildingTab : MonoBehaviour
{
    [Header("필요한 컴포넌트들")]
    [SerializeField] private GameObject Base_UI;
    [SerializeField] private GameObject Weapon_Change_UI;
    [SerializeField] private GameObject Range_Inditator;
    [SerializeField] private Transform tf_Player_Cam;  // 플레이어 위치
    private GameObject go_Preview; // 미리 보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수 

    [SerializeField] private Craft[] craft_slot;  // 벽 탭에 있는 슬롯들. 

    [Header("레이어마스크 및 사거리")]
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    //private int currentSlotNum = -1; // 현재 선택한 슬롯 번호

    [Header("회전을 위한 변수들")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float previewRotationY = 0f;

    [Header("탭 전환을 위한 변수들")]
    [SerializeField] private GameObject[] slotList;



    void Update()
    {
        if (GameManager.isPlayerDead || GameManager.isPause)
            return;

        if (Input.GetKeyDown(KeyCode.Tab) && !GameManager.isPreviewActivated)
        {
            OpenWindow();    
        }
        if (GameManager.isPreviewActivated)
        {
            PreviewPositionUpdate();
            MouseScroll();
        }

        if (Input.GetButtonDown("Fire1"))
            Build();

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
    }

    void MouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && go_Preview != null)
        {
            previewRotationY += scroll * rotationSpeed;
            go_Preview.transform.rotation = Quaternion.Euler(0, previewRotationY, 0);
        }
    }

    void OpenWindow()
    {
        if (!GameManager.isOpenShopMenu)
        {
            GameManager.isOpenShopMenu = true;
            Weapon_Change_UI.SetActive(false);
            Base_UI.SetActive(true);
        }
        else
        {
            GameManager.isOpenShopMenu = false;
            Weapon_Change_UI.SetActive(false);
            Base_UI.SetActive(false);
        }
            
    }
    private void PreviewPositionUpdate()
    {
        //건축 범위 알려주는 오브젝트 생성
        Range_Inditator.GetComponent<Transform>().position =  new Vector3(tf_Player_Cam.position.x, 0.6f, tf_Player_Cam.position.z);

        //건축물을 설치할 위치 감지
        if (Physics.Raycast(tf_Player_Cam.position, tf_Player_Cam.forward, out hitInfo, range, layerMask))
        {
            //위치가 감지되었으면
            if (hitInfo.transform != null)
            {
                go_Preview.GetComponent<PreviewObject>().israycastHit = true;
       
                Vector3 _location = hitInfo.point;
                go_Preview.transform.position = _location;
            }
        }
        //위치가 감지되지 않았으면
        else
        {
            go_Preview.GetComponent<PreviewObject>().israycastHit = false;
        }
    }

    //건물 생성
    private void Build()
    {
        if (GameManager.isPreviewActivated && go_Preview.GetComponent<PreviewObject>().IsBuildable())
        {
            Range_Inditator.SetActive(false);
            Instantiate(go_Prefab, hitInfo.point, Quaternion.Euler(0, previewRotationY, 0));
            Destroy(go_Preview);
            GameManager.isOpenShopMenu = false;
            GameManager.isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }

    //창 닫기
    private void Cancel()
    {
        if (GameManager.isPreviewActivated)
        {
            Range_Inditator.SetActive(false);
            Destroy(go_Preview);
        }

        GameManager.isOpenShopMenu = false;
        GameManager.isPreviewActivated = false;

        go_Preview = null;
        go_Prefab = null;

        Base_UI.SetActive(false);
    }

    //프리뷰 보여주기
    public void SlotClick(string craftName)
    {
        int _slotNumber;
        //무슨 블록을 설치할지 찾기
        for (int i = 0; i < craft_slot.Length; i++)
        {
            if (craftName == craft_slot[i].craftName)
            {
                _slotNumber = i;

                // 이부분에 player돈 과 관련한 조건문을 걸어서 player돈 이상이면 아래코드를 실행하고 아니면 돈 부족이라고 뜸
                if(GameManager.UseMoney(craft_slot[_slotNumber].craftMoney))
                {
                    Range_Inditator.SetActive(true);
                    go_Preview = Instantiate(craft_slot[_slotNumber].go_PreviewPrefab, tf_Player_Cam.position + tf_Player_Cam.forward, Quaternion.identity);
                    go_Prefab = craft_slot[_slotNumber].go_prefab;
                    GameManager.isPreviewActivated = true;
                    Base_UI.SetActive(false);
                    previewRotationY = 0f;
                    //currentSlotNum = _slotNumber; // 현재 선택 슬롯 기억
                }

                return;
            }

        }
    }


    public void ClickTab(int _tabNum)
    {
        if(Weapon_Change_UI.activeSelf)
            Weapon_Change_UI.SetActive(false);
        //모든 탭 비활성화
        for (int i = 0; i < slotList.Length; i++)
        {
            slotList[i].SetActive(false);
        }
        //해당하는 탭 활성화
        slotList[_tabNum].SetActive(true);
    }
    
}
