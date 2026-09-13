using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceController : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer _cherryMesh;
    [SerializeField]
    private SkinnedMeshRenderer _eyesMesh;
    [SerializeField]
    private SkinnedMeshRenderer _headMesh;
    [SerializeField]
    private SkinnedMeshRenderer _noseMesh;
    [SerializeField]
    private SkinnedMeshRenderer _teethMesh;

    public float _moveSpeed = 3.0f;

    public void UpdateFaceBlends(float newValue)
    {
        float currentWeight = this._cherryMesh.GetBlendShapeWeight(0);

        float newWeight = Mathf.Lerp(currentWeight, newValue, this._moveSpeed * Time.deltaTime);

        this._cherryMesh.SetBlendShapeWeight(0, newWeight);
        this._eyesMesh.SetBlendShapeWeight(0, newWeight);
        this._headMesh.SetBlendShapeWeight(0, newWeight);
        this._noseMesh.SetBlendShapeWeight(0, newWeight);
        this._teethMesh.SetBlendShapeWeight(0, newWeight);
    }
}
