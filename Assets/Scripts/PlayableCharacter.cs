using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Actor m_Actor;

    [Header("Movement")]
    private float m_MovementSpeed = 5f;

    [Header("Gravity")]
    [SerializeField]
    private bool m_IsAffectedByGravity;
    [SerializeField, Range(0f, 10f)]
    private float m_GravityAmount;

    private Vector3 m_CurrentVelocity;

    protected void Update()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f).normalized;
        m_CurrentVelocity += move * m_MovementSpeed;

        if (m_IsAffectedByGravity)
        {
            m_CurrentVelocity.y -= m_GravityAmount;
        }

        m_Actor.Move(m_CurrentVelocity * Time.deltaTime);

        m_CurrentVelocity = Vector3.zero;
    }
}