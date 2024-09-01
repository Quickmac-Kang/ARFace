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

    //버튼 오브젝트
    public Button togglePointButton; 
    public Button toggleHeadButton;
    public Button toggleEyeButton;
    public Button toggleNoseButton;
    public Button toggleMouthButton;


    //페이스 악세사리 정보 위치 명칭 / Verities인덱스 / 장착할 프리팹
    public List<string> names;
    public List<int> vertaxList;
    public List<GameObject> objects;

    //디버그용 텍스트
    public Text debugText;

    //AR Face Manager 콤포넌트 참조.
    ARFaceManager arFaceManager;

    FacePointsVisualizer facePointsVisualizer;
    
    //페이스 카운트가 1이상일 경우를 대비.
    List<ARFace> faces = new List<ARFace>();
    //직렬화 데이터를 컨테이너화 한다.
    Dictionary<string, GameObject> faceObjects = new Dictionary<string, GameObject>();
    //생성된 오브젝트 관리용 컨테이너
    Dictionary<string, GameObject> manageObjects = new Dictionary<string, GameObject>();
    //오브젝트 On / Off 상태 기록. 업데이트시 활용 한다.
    Dictionary<string, bool> showObjectes = new Dictionary<string, bool>();
    //오브젝트 업데이트시 위치 정보로 활용 한다.
    Dictionary<string, int> vertaxIndexs = new Dictionary<string, int>();



    // Start is called before the first frame update
    void Start()
    {
        arFaceManager = GetComponent<ARFaceManager>();
        arFaceManager.facesChanged += OnFacesChanged;

        //페이스 vertices 정보를 가져오기 위해 FacePointsVisualizer 참조.
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
                if (index == -1) // 유효한 인덱스를 찾았을 때
                    return;
                faces[index] = face; // 리스트의 기존 face를 업데이트
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
                faces.Remove(face); // 리스트에서 직접 삭제
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
            // 얼굴의 10번째 버텍스 위치를 월드 좌표로 변환
            Vector3 worldPosition = faces[0].transform.TransformPoint(faces[0].vertices[vertaxIndexs[objectName]]);

            // "Head" 오브젝트를 해당 위치에 배치
            GameObject headObject = Instantiate(faceObjects[objectName], worldPosition, Quaternion.identity);
            // 선택 사항: 오브젝트를 얼굴의 자식으로 설정하여 얼굴을 따라다니도록 할 수 있습니다.
            headObject.transform.SetParent(faces[0].transform);

            manageObjects[objectName] = headObject;
        }
    }

    void ToggleFaceObject(string objectName)
    {
        if (manageObjects[objectName] == null)
        {
            // 얼굴의 10번째 버텍스 위치를 월드 좌표로 변환
            Vector3 worldPosition = faces[0].transform.TransformPoint(faces[0].vertices[vertaxIndexs[objectName]]);

            // "Head" 오브젝트를 해당 위치에 배치
            GameObject headObject = Instantiate(faceObjects[objectName], worldPosition, Quaternion.identity);
            // 선택 사항: 오브젝트를 얼굴의 자식으로 설정하여 얼굴을 따라다니도록 할 수 있습니다.
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
