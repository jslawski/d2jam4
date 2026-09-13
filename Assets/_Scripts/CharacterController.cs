using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Vector3 _minScale;
    private Vector3 _maxScale;
    private Vector3 _targetScale;

    private float _minYPosition = -4.0f;
    private float _maxYPosition = 4.0f;
    private float _targetYPosition = 0.0f;
    private Vector3 _targetPosition = Vector3.zero;

    private float _normalizedVolumeValue = 0;
    private float _targetVolumeValue = 0;

    private float _normalizedPitchValue = 0;

    private float moveSpeed = 3.0f;
    private float scaleSpeed = 10.0f;

    private FaceController _faceController;

    private void Awake()
    {
        this._minScale = Vector3.one * 0.5f;
        this._maxScale = Vector3.one * 3.0f;

        this._targetScale = this._minScale;

        this._faceController = GetComponent<FaceController>();
    }

    private void Update()
    {
        this.ApplyLoudnessChanges();
        this.ApplyPitchChanges();
    }

    private void ApplyLoudnessChanges()
    {
        this._normalizedVolumeValue = MicrophoneManager.instance.GetNormalizedLoudness();

        this._faceController.UpdateFaceBlends(this._normalizedVolumeValue * 100.0f);

        //this.ScaleDebugObject();               
    }

    private void ScaleDebugObject()
    {
        this._targetScale = Vector3.Lerp(this._minScale, this._maxScale, this._normalizedVolumeValue);
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, this._targetScale, this.scaleSpeed * Time.deltaTime);        
    }

    private void ApplyPitchChanges()
    {
        this._normalizedPitchValue = MicrophoneManager.instance.GetCurrentNormalizedPitch();
        this._targetYPosition = Mathf.Lerp(this._minYPosition, this._maxYPosition, this._normalizedPitchValue);
        
        this._targetPosition = this.transform.localPosition;
        this._targetPosition.y = this._targetYPosition;

        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, this._targetPosition, this.moveSpeed * Time.deltaTime);
    }
}
