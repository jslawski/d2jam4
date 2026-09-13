using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private HealthBar _healthBar;

    public float currentHealth = 1.0f;

    private float _healthPerMiss = 0.10f;
    private float _healthPerSwallow = 0.03f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddHealth()
    {
        this._healthBar.AddHealth(this._healthPerSwallow);
    }

    public void RemoveHealth()
    {
        this._healthBar.RemoveHealth(this._healthPerMiss);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
