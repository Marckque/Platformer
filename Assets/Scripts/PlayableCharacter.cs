using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControllableParameters
{
    [Header("Movement")]
    public float maxVelocity = 1f;

    [Header("Acceleration"), Tooltip("If this is true, all acceleration values are useless.")]
    public bool accelerationIsOff;
    [Range(0f, 10f)]
    public float accelerationAmount = 1f;
    public AnimationCurve accelerationCurve;

    [Header("Deceleration"), Tooltip("If this is true, all deceleration values are useless.")]
    public bool decelerationIsOff;
    [Range(0f, 10f)]
    public float decelerationAmount = 1f;
    public AnimationCurve decelerationCurve;

    [Header("Jump")]
    public float jumpHeight = 1f;
}

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

    private Vector3 m_CurrentInput;
    private Vector3 m_Velocity;

    [SerializeField]
    private ControllableParameters m_ControllableParameters = new ControllableParameters();
    public ControllableParameters pControllable { get { return m_ControllableParameters; } }

    private AnimationCurve m_VelocityCurve;
    private bool m_InitialiseAcceleration = true;
    private bool m_InitialiseDeceleration;
    private float m_AccelerationTime;
    private float m_DecelerationTime;
    private float m_VelocityTime;
    private float m_VelocityMultiplier;
    private Vector3 m_LastInput;
    private Vector3 m_LastDirection;
    private Vector3 m_DesiredVelocity;

    protected void Update()
    {
        MovementUpdate();

        m_Velocity = (m_LastDirection * m_MovementSpeed * m_VelocityMultiplier);

        if (m_IsAffectedByGravity)
        {
            m_Velocity.y -= m_GravityAmount;
        }

        m_Actor.Move(m_Velocity * Time.deltaTime);

        m_Velocity = Vector3.zero;
    }

    private void MovementUpdate()
    {
        GetMovementInput();

        if (m_CurrentInput != Vector3.zero)
        {
            Acceleration();
        }
        else
        {
            Deceleration();
        }

        UpdateVelocityMultiplier();
        //RotateTowardsMovementDirection();
    }

    private void GetMovementInput()
    {
        m_CurrentInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f).normalized;

        if (m_CurrentInput != Vector3.zero)
        {
            m_LastDirection = m_CurrentInput;
        }
    }

    private void Acceleration()
    {
        if (m_InitialiseAcceleration)
        {
            m_AccelerationTime = 1f - m_DecelerationTime;
            m_InitialiseAcceleration = false;
            m_InitialiseDeceleration = true;
        }

        m_DecelerationTime = 0f;
        m_VelocityCurve = pControllable.accelerationCurve;

        if (m_AccelerationTime < 1f)
        {
            if (pControllable.accelerationIsOff)
            {
                m_AccelerationTime = 1f;
            }
            else
            {
                m_AccelerationTime += Time.deltaTime * pControllable.accelerationAmount;
            }
        }
        else
        {
            m_AccelerationTime = 1f;
        }
    }

    private void Deceleration()
    {
        if (m_InitialiseDeceleration)
        {
            m_DecelerationTime = 1f - m_AccelerationTime;
            m_InitialiseAcceleration = true;
            m_InitialiseDeceleration = false;
        }

        m_AccelerationTime = 0f;
        m_VelocityCurve = pControllable.decelerationCurve;

        if (m_DecelerationTime < 1f)
        {
            if (pControllable.decelerationIsOff)
            {
                m_DecelerationTime = 1f;
            }
            else
            {
                m_DecelerationTime += Time.deltaTime * pControllable.decelerationAmount;
            }
        }
        else
        {
            m_DecelerationTime = 1f;
        }
    }

    private void UpdateVelocityMultiplier()
    {
        m_VelocityTime = m_AccelerationTime > 0 ? m_AccelerationTime : m_DecelerationTime;
        m_VelocityMultiplier = m_VelocityCurve.Evaluate(m_VelocityTime);
    }

    private void RotateTowardsMovementDirection()
    {
        transform.eulerAngles = m_LastDirection.x < 0 ? new Vector3(0f, 180f, 0f) : Vector3.zero;
    }
}