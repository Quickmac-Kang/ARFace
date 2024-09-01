using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class FaceManager : MonoBehaviour
{

    //��ư ������Ʈ
    public Button togglePointButton; 
    public Button toggleHeadButton;
    public Button toggleEyeButton;
    public Button toggleNoseButton;
    public Button toggleMouthButton;


    //���̽� �Ǽ��縮 ���� ��ġ ��Ī / Verities�ε��� / ������ ������
    public List<string> names;
    public List<int> vertaxList;
    public List<GameObject> objects;

    //����׿� �ؽ�Ʈ
    public Text debugText;

    //AR Face Manager ������Ʈ ����.
    ARFaceManager arFaceManager;

    FacePointsVisualizer facePointsVisualizer;
    
    //���̽� ī��Ʈ�� 1�̻��� ��츦 ���.
    List<ARFace> faces = new List<ARFace>();
    //����ȭ �����͸� �����̳�ȭ �Ѵ�.
    Dictionary<string, GameObject> faceObjects = new Dictionary<string, GameObject>();
    //������ ������Ʈ ������ �����̳�
    Dictionary<string, GameObject> manageObjects = new Dictionary<string, GameObject>();
    //������Ʈ On / Off ���� ���. ������Ʈ�� Ȱ�� �Ѵ�.
    Dictionary<string, bool> showObjectes = new Dictionary<string, bool>();
    //������Ʈ ������Ʈ�� ��ġ ������ Ȱ�� �Ѵ�.
    Dictionary<string, int> vertaxIndexs = new Dictionary<string, int>();



    // Start is called before the first frame update
    void Start()
    {
        arFaceManager = GetComponent<ARFaceManager>();
        arFaceManager.facesChanged += OnFacesChanged;

        //���̽� vertices ������ �������� ���� FacePointsVisualizer ����.
        facePointsVisualizer = GetComponent<FacePointsVisualizer>();
        
        int index = 0;
        foreach (string name in names)
        {
            if (index >= objects.Count)
                break;
            faceObjects[name] = objects[index];
            manageObjects[name] = null;
            showObjectes[name] = false;
            vertaxIndexs[name] = vertaxList[index];
            index++;
        }

        togglePointButton.onClick.AddListener(ToggleButtonFacePoints);
        toggleHeadButton.onClick.AddListener(ToggleButtonHeadObject);
        toggleEyeButton.onClick.AddListener(ToggleButtonEyeObject);
        toggleNoseButton.onClick.AddListener(ToggleButtonNoseObject);
        toggleMouthButton.onClick.AddListener(ToggleButtonMouthObject);
    }

    void OnFacesChanged(ARFacesChangedEventArgs eventArgs)
    {
        foreach (ARFace face in eventArgs.added)
        {
            if (face != null && !faces.Contains(face))
            {
                faces.Add(face);
                debugText.text = "eventArgs.added";
            }
        }

        foreach (ARFace face in eventArgs.updated)
        {
            if (face != null && faces.Contains(face))
            { 
                int index = faces.IndexOf(face);
                if (index == -1) // ��ȿ�� �ε����� ã���� ��
                    return;
                faces[index] = face; // ����Ʈ�� ���� face�� ������Ʈ
                //debugText.text = "eventArgs.updated";
                foreach(string name in names)
                { 
                    UpdateFaceObject(name);
                }
            }
        }

        foreach (ARFace face in eventArgs.removed)
        {
            if (face != null && faces.Contains(face))
            {
                foreach (string name in names)
                {
                    if (manageObjects[name] != null)
                    {
                        Destroy(manageObjects[name]);
                        manageObjects[name] = null;
                    }
                }
                faces.Remove(face); // ����Ʈ���� ���� ����
                debugText.text = "eventArgs.removed";
            }
        }
    }

    void ToggleButtonFacePoints()
    {
        if (facePointsVisualizer != null)
        {
            if (facePointsVisualizer.isShowPoint)
                facePointsVisualizer.ShowAllFacePoints(false);
            else
                facePointsVisualizer.ShowAllFacePoints(true);
        }
    }
    void ToggleButtonHeadObject()
    {
        if (faces.Count == 0)
            return;
        ToggleFaceObject("Head");
    }
    void ToggleButtonEyeObject()
    {
        if (faces.Count == 0)
            return;
        ToggleFaceObject("Eye");
    }
    void ToggleButtonNoseObject()
    {
        if (faces.Count == 0)
            return;
        ToggleFaceObject("Nose");
    }
    void ToggleButtonMouthObject()
    {
        if (faces.Count == 0)
            return;
        ToggleFaceObject("Mouth");
    }


    void UpdateFaceObject(string objectName)
    {
        if (manageObjects[objectName] == null && showObjectes[objectName] == true)
        {
            // ���� 10��° ���ؽ� ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 worldPosition = faces[0].transform.TransformPoint(faces[0].vertices[vertaxIndexs[objectName]]);

            // "Head" ������Ʈ�� �ش� ��ġ�� ��ġ
            GameObject headObject = Instantiate(faceObjects[objectName], worldPosition, Quaternion.identity);
            // ���� ����: ������Ʈ�� ���� �ڽ����� �����Ͽ� ���� ����ٴϵ��� �� �� �ֽ��ϴ�.
            headObject.transform.SetParent(faces[0].transform);

            manageObjects[objectName] = headObject;
        }
    }

    void ToggleFaceObject(string objectName)
    {
        if (manageObjects[objectName] == null)
        {
            // ���� 10��° ���ؽ� ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 worldPosition = faces[0].transform.TransformPoint(faces[0].vertices[vertaxIndexs[objectName]]);

            // "Head" ������Ʈ�� �ش� ��ġ�� ��ġ
            GameObject headObject = Instantiate(faceObjects[objectName], worldPosition, Quaternion.identity);
            // ���� ����: ������Ʈ�� ���� �ڽ����� �����Ͽ� ���� ����ٴϵ��� �� �� �ֽ��ϴ�.
            headObject.transform.SetParent(faces[0].transform);

            manageObjects[objectName] = headObject;
        }
        else
        {
            Destroy(manageObjects[objectName]);
            manageObjects[objectName] = null;
        }

        showObjectes[objectName] = !showObjectes[objectName];
    }
}
