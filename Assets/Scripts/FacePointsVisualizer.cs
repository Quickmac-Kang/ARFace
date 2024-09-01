using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;



public class FacePointsVisualizer : MonoBehaviour
{
    [HideInInspector]
    public bool isShowPoint = false;

    ARFaceManager arFaceManager;
    Dictionary<ARFace, List<GameObject>> faceTextObjects = new Dictionary<ARFace, List<GameObject>>();

    void Start()
    {
        arFaceManager = GetComponent<ARFaceManager>();
        arFaceManager.facesChanged += OnFacesChanged;
    }

   // public ARFaceManager arFManager{ get { return arFaceManager; } }
    void OnFacesChanged(ARFacesChangedEventArgs eventArgs)
    {
        foreach (ARFace face in eventArgs.added)
        {
            faceTextObjects[face] = new List<GameObject>();
            DisplayFacePoints(face);
        }

        foreach (ARFace face in eventArgs.updated)
        {
            UpdateFacePoints(face);
        }

        foreach (ARFace face in eventArgs.removed)
        {
            ClearFacePoints(face);
        }
    }

    void DisplayFacePoints(ARFace face)
    {
        var vertices = face.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosition = face.transform.TransformPoint(vertices[i]);

            GameObject text = new GameObject($"Point_{i}");
            text.transform.position = worldPosition;
            TextMesh textMesh = text.AddComponent<TextMesh>();
            textMesh.text = i.ToString();
            textMesh.characterSize = 0.0025f; // 글자 크기 조금 더 작게 조절
            textMesh.color = Color.yellow; 
            text.SetActive(isShowPoint);
            faceTextObjects[face].Add(text);
        }
    }

    void UpdateFacePoints(ARFace face)
    {
        var vertices = face.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosition = face.transform.TransformPoint(vertices[i]);
            faceTextObjects[face][i].transform.position = worldPosition;
            faceTextObjects[face][i].transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0, 180, 0);
        }
    }

    void ClearFacePoints(ARFace face)
    {
        if (faceTextObjects.ContainsKey(face))
        {
            foreach (var textObj in faceTextObjects[face])
            {
                Destroy(textObj);
            }
            faceTextObjects.Remove(face);
        }
    }

    public void ShowAllFacePoints(bool isShow)
    {
        isShowPoint = isShow;
        // 모든 텍스트 오브젝트를 제거
        foreach (var face in faceTextObjects.Keys)
        {
            foreach (var textObj in faceTextObjects[face])
            {
                textObj.SetActive(isShowPoint);
            }
        }

        // 딕셔너리 비우기
       // faceTextObjects.Clear();
    }
}
