using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public GameObject go_prefab; // 실제 설치 될 프리팹
    public GameObject go_PreviewPrefab; // 미리 보기 프리팹
}

public class BuildingTab : MonoBehaviour
{
    private bool isActivated = false;
    private bool isPreviewActivated = false; // 미리 보기 활성화 상태
    [Header("필요한 컴포넌트들")]
    [SerializeField] private GameObject Base_UI;
    [SerializeField] private Transform tf_Player;  // 플레이어 위치
    private GameObject go_Preview; // 미리 보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수 

    [SerializeField] private Craft[] craft_wall;  // 벽 탭에 있는 슬롯들. 

    [Header("레이어마스크 및 사거리")]
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    [Header("회전을 위한 변수들")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float previewRotationY = 0f;
    public void SlotClick(int _slotNumber)
    {
        go_Preview = Instantiate(craft_wall[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_wall[_slotNumber].go_prefab;
        isPreviewActivated = true;
        Base_UI.SetActive(false);
        previewRotationY = 0f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
        {
            OpenWindow();
            Debug.Log("탭");
            
        }
        if (isPreviewActivated)
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
        if (!isActivated)
        {
            isActivated = true;
            Base_UI.SetActive(true);
        }
        else
        {
            isActivated = false;
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
        Debug.Log(isPreviewActivated);
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            Instantiate(go_Prefab, hitInfo.point, Quaternion.Euler(0, previewRotationY, 0));
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }
    private void Cancel()
    {
        if (isPreviewActivated)
            Destroy(go_Preview);

        isActivated = false;
        isPreviewActivated = false;

        go_Preview = null;
        go_Prefab = null;

        Base_UI.SetActive(false);
    }
}
