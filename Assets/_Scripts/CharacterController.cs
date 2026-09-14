using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Vector3 _minScale;
    private Vector3 _maxScale;
    private Vector3 _targetScale;

    public float _minYPosition = -2.75f;
    public float _maxYPosition = 5.0f;    
    
    private float _targetYPosition = 0.0f;
    private Vector3 _targetPosition = Vector3.zero;

    private float _normalizedVolumeValue = 0;
    private float _targetVolumeValue = 0;

    private float _normalizedPitchValue = 0;

    private float moveSpeed = 3.0f;
    private float scaleSpeed = 10.0f;

    private FaceController _faceController;
    private MouthController _mouthController;

    private void Awake()
    {
        this._minScale = Vector3.one * 0.5f;
        this._maxScale = Vector3.one * 3.0f;

        this._targetScale = this._minScale;

        this._faceController = GetComponent<FaceController>();
        this._mouthController = GetComponentInChildren<MouthController>();
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
        this._mouthController.UpdateFaceColliders(this._normalizedVolumeValue);

        //this.ScaleDebugObject();               
    }

    private void ApplyPitchChanges()
    {
        if (this._normalizedVolumeValue <= MicrophoneManager.instance.noiseGate)
        {
            return;
        }

        this._normalizedPitchValue = MicrophoneManager.instance.GetCurrentNormalizedPitch();
        this._targetYPosition = Mathf.Lerp(this._minYPosition, this._maxYPosition, this._normalizedPitchValue);
        
        this._targetPosition = this.transform.localPosition;
        this._targetPosition.y = this._targetYPosition;

        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, this._targetPosition, this.moveSpeed * Time.deltaTime);
    }
}
