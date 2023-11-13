using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class Track : MonoBehaviour
{
    public ARTrackedImageManager manager;
    //Ʈ��ŷ�� �̹����� Ȱ��ȭ ��ų ���� ������Ʈ ����Ʈ
    public List<GameObject> list1 = new List<GameObject> ();

    //�̹����� �̸��� 3d ������Ʈ�� �����ϴ� ��ųʸ�
    Dictionary<string, GameObject> dict1 = new Dictionary<string, GameObject> ();


    public List<GameObject> uiPrefabs; // Ȱ��ȭ�� UI ������ ����Ʈ
    private Dictionary<string, RectTransform> uiRectTransforms = new Dictionary<string, RectTransform>(); // UI �����հ� RectTransform�� ����


    void Start()
    {
        foreach(GameObject obj in list1)
        {//list1�� �� ���� ������Ʈ�� dict1�� �߰��Ѵ�
            //obj.name : �ν�����â���� ���ӿ�����Ʈ�� �̸�
            //obj.name�� Ű��, obj�� ������ ������ ���ο� �׸��� dict1 ��ųʸ��� �߰��Ѵ�
            //dict1�� �̹����� �̸��� ����Ͽ� �ش� �̹����� �����ϴ� ���� ������Ʈ�� ã�� ���� ���踦 ���´�
            dict1.Add (obj.name, obj); 
        }


        // �� UI �����տ� ���� RectTransform ���� ����
        foreach (GameObject uiPrefab in uiPrefabs)
        {
            RectTransform uiRectTransform = Instantiate(uiPrefab, transform).GetComponent<RectTransform>();
            uiRectTransform.gameObject.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ ���·� ����
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

    //ARȯ�濡�� �̹����� �߰��ǰų� ������Ʈ �� �� ȣ��Ǵ� �ݹ� �޼���
    //ARTrackedImagesChangedEventArgs ��ü�� �߰��� �̹����� ������Ʈ �� �̹����� ���� ������ ����
    //e�� �̺�Ʈ�� �߻��� �� ���� �ش� �μ��� �̹��� �߰�, �̹��� ������Ʈ ���� ������ ���ԵǾ��ִ�
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
        // ���ŵ� �̹����� Ȯ���ϰ� ����� ���� ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        foreach (ARTrackedImage t in e.removed)
        {
            DisableGameObject(t.referenceImage.name);
        }
    }

    //������ �̹����� ������Ʈ�ϴ� �޼���
    private void UpdateImage(ARTrackedImage t)
    {
        //������ �̹����� �̸��� �����´�
        string name = t.referenceImage.name;
        //dict1���� �ش� �̸��� �����ϴ� ���� ������Ʈ�� ã�´�
        GameObject obj = dict1[name];

        obj.transform.position = t.transform.position;
        obj.transform.rotation = t.transform.rotation;
        obj.SetActive(true);

        // UI ����� ��ġ�� ȸ���� ����
        RectTransform uiRectTransform = obj.GetComponent<RectTransform>();
        if (uiRectTransform != null)
        {
            uiRectTransform.position = t.transform.position;
            uiRectTransform.rotation = t.transform.rotation;
            // ���⿡�� �߰����� RectTransform ������ ������ �� �ֽ��ϴ�.
            // ��: ��Ŀ ����Ʈ, �ǹ�, ũ�� ���� ��
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
