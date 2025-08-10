using TMPro;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public GameObject go_prefab; // 실제 설치 될 프리팹
    public GameObject go_PreviewPrefab; // 미리 보기 프리팹
    public int count; // 보유한 건축물 개수
}

public class BuildingTab : MonoBehaviour
{
    [Header("필요한 컴포넌트들")]
    [SerializeField] private GameObject Base_UI;
    [SerializeField] private Transform tf_Player;  // 플레이어 위치
    private GameObject go_Preview; // 미리 보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수 

    [SerializeField] private Craft[] craft_slot;  // 벽 탭에 있는 슬롯들. 
    [SerializeField] private TMP_Text[] countTexts;

    [Header("레이어마스크 및 사거리")]
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    private int currentSlotNum = -1; // 현재 선택한 슬롯 번호

    [Header("회전을 위한 변수들")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float previewRotationY = 0f;

    [Header("탭 전환을 위한 변수들")]
    [SerializeField] private GameObject[] slotList;
    private void Start()
    {
        //시작되면 슬롯 개수 초기화
        for (int i = 0; i < craft_slot.Length; i++)
        {
            UpdateSlotUI(i);
        }
    }

    private void UpdateSlotUI(int _slotNum)
    {
        if (_slotNum >= 0 && _slotNum < countTexts.Length)
            countTexts[_slotNum].text = craft_slot[_slotNum].count.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !GameManager.isPreviewActivated)
        {
            OpenWindow();
            Debug.Log("탭");
            
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
        if (!GameManager.isOpenCraftMenu)
        {
            GameManager.isOpenCraftMenu = true;
            Base_UI.SetActive(true);
        }
        else
        {
            GameManager.isOpenCraftMenu = false;
            Base_UI.SetActive(false);
        }
            
    }
    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;
                go_Preview.transform.position = _location;
            }
        }
    }

    private void Build()
    {
        if (GameManager.isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            Instantiate(go_Prefab, hitInfo.point, Quaternion.Euler(0, previewRotationY, 0));
            Destroy(go_Preview);
            GameManager.isOpenCraftMenu = false;
            GameManager.isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
            craft_slot[currentSlotNum].count--;
            UpdateSlotUI(currentSlotNum);
        }
    }
    private void Cancel()
    {
        if (GameManager.isPreviewActivated)
            Destroy(go_Preview);

        GameManager.isOpenCraftMenu = false;
        GameManager.isPreviewActivated = false;

        go_Preview = null;
        go_Prefab = null;

        Base_UI.SetActive(false);
    }
    public void SetSlot(int _slotNum, int _count)
    {
        craft_slot[_slotNum].count = _count;
        UpdateSlotUI(_slotNum);
    }
    public void SlotClick(int _slotNumber)
    {
        if (craft_slot[_slotNumber].count <= 0)
        {
            Debug.Log("해당 건축물이 없습니다.");
            return;
        }
        go_Preview = Instantiate(craft_slot[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_slot[_slotNumber].go_prefab;
        GameManager.isPreviewActivated = true;
        Base_UI.SetActive(false);
        previewRotationY = 0f;
        currentSlotNum = _slotNumber; // 현재 선택 슬롯 기억
    }
    public void ClickTab(int _tabNum)
    {
        //모든 탭 비활성화
        for (int i = 0; i < slotList.Length; i++)
        {
            slotList[i].SetActive(false);
        }
        //해당하는 탭 활성화
        slotList[_tabNum].SetActive(true);
    }
}
