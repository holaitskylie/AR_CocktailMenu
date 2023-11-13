using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class Track : MonoBehaviour
{
    public ARTrackedImageManager manager;
    //트랙킹한 이미지에 활성화 시킬 게임 오브젝트 리스트
    public List<GameObject> list1 = new List<GameObject> ();

    //이미지의 이름과 3d 오브젝트를 연결하는 딕셔너리
    Dictionary<string, GameObject> dict1 = new Dictionary<string, GameObject> ();


    public List<GameObject> uiPrefabs; // 활성화할 UI 프리팹 리스트
    private Dictionary<string, RectTransform> uiRectTransforms = new Dictionary<string, RectTransform>(); // UI 프리팹과 RectTransform의 매핑


    void Start()
    {
        foreach(GameObject obj in list1)
        {//list1의 각 게임 오브젝트를 dict1에 추가한다
            //obj.name : 인스펙터창에서 게임오브젝트의 이름
            //obj.name을 키로, obj를 값으로 가지는 새로운 항목을 dict1 딕셔너리에 추가한다
            //dict1은 이미지의 이름을 사용하여 해당 이미지에 대응하는 게임 오브젝트를 찾는 매핑 관계를 갖는다
            dict1.Add (obj.name, obj); 
        }


        // 각 UI 프리팹에 대한 RectTransform 참조 생성
        foreach (GameObject uiPrefab in uiPrefabs)
        {
            RectTransform uiRectTransform = Instantiate(uiPrefab, transform).GetComponent<RectTransform>();
            uiRectTransform.gameObject.SetActive(false); // 초기에는 비활성화 상태로 설정
            uiRectTransforms.Add(uiPrefab.name, uiRectTransform);
        }


    }

    private void OnEnable()
    {
        manager.trackedImagesChanged += OnChanged;
    }

    private void OnDisable()
    {
        manager.trackedImagesChanged -= OnChanged;
    }

    //AR환경에서 이미지가 추가되거나 업데이트 될 때 호출되는 콜백 메서드
    //ARTrackedImagesChangedEventArgs 객체는 추가된 이미지와 업데이트 된 이미지에 대한 정보를 제공
    //e는 이벤트가 발생할 때 마다 해당 인수에 이미지 추가, 이미지 업데이트 등의 정보가 포함되어있다
    private void OnChanged(ARTrackedImagesChangedEventArgs e) 
    {
        foreach(ARTrackedImage t in e.added)
        {
            UpdateImage (t);
        }
        foreach(ARTrackedImage t in e.updated)
        {
            UpdateImage (t);
        }
        // 제거된 이미지를 확인하고 연결된 게임 오브젝트를 비활성화합니다.
        foreach (ARTrackedImage t in e.removed)
        {
            DisableGameObject(t.referenceImage.name);
        }
    }

    //추적된 이미지를 업데이트하는 메서드
    private void UpdateImage(ARTrackedImage t)
    {
        //추적된 이미지의 이름을 가져온다
        string name = t.referenceImage.name;
        //dict1에서 해당 이름에 대응하는 게임 오브젝트를 찾는다
        GameObject obj = dict1[name];

        obj.transform.position = t.transform.position;
        obj.transform.rotation = t.transform.rotation;
        obj.SetActive(true);

        // UI 요소의 위치와 회전값 조정
        RectTransform uiRectTransform = obj.GetComponent<RectTransform>();
        if (uiRectTransform != null)
        {
            uiRectTransform.position = t.transform.position;
            uiRectTransform.rotation = t.transform.rotation;
            // 여기에서 추가적인 RectTransform 설정을 수행할 수 있습니다.
            // 예: 앵커 포인트, 피벗, 크기 조정 등
        }

    }

    private void DisableGameObject(string imageName)
    {
        if (dict1.TryGetValue(imageName, out GameObject obj))
        {
            obj.SetActive(false);

            RectTransform uiRectTransform = obj.GetComponent<RectTransform>();
            if (uiRectTransform != null)
            {
                uiRectTransform.gameObject.SetActive(false);
            }

        }
    }

}
