using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI Behaviour/MonsterStats")]
[System.Serializable]
public class SO_MonsterStats : ScriptableObject {

    [Header("Movements")]
    [SerializeField]
    public float speed = 1f;
    [SerializeField]
    public float stopingDistance = 0.5f;

    [Header("Detection")]
    [SerializeField]
    public float lookRange = 40f;
    [SerializeField]
    public float lookShpereCastRadius = 1f;

    [Header("Attack")]
    [SerializeField]
    public float attackRange = 1f;
    [SerializeField]
    public float attackRate = 1f;
    [SerializeField]
    public float attackPoint = 15f;
    [SerializeField]
    public float searchDuration = 4f;
}
