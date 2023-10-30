using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgenteML : Agent
{
    [SerializeField]
    private float _fuerzaMovimieto = 200;
    [SerializeField]
    private Transform _target, _isla;
    public bool _training = true;
    private Rigidbody _rb;
    public override void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        //MaxStep forma parte de la clase Agent
        if (!_training) MaxStep = 0;
    }
    public override void OnEpisodeBegin()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        MoverPosicionInicial();
    }
    /// <summary>
    /// El vectorAction nos sirve para construir un vector de desplazamiento
    /// [0]: X.
    /// [1]: Z.
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Construimos un vector con el vector recibido.
        Vector3 movimiento = new Vector3(actions.ContinuousActions[0],
            0f, actions.ContinuousActions[1]);
        //Sumamos el vector construido al rigidbody como fuerza 
        _rb.AddForce(movimiento * _fuerzaMovimieto * Time.deltaTime);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //Calcular cuanto nos queda hasta el objetivo
        Vector3 alObjetivo = _target.position - transform.position;
        //Un vector ocupa 3 observaciones. 
        sensor.AddObservation(alObjetivo.normalized);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {

        //var continuousActionsOut = actionsOut.ContinuousActions;
        //continuousActionsOut[0] = Input.GetAxis("Horizontal");
        //continuousActionsOut[1] = Input.GetAxis("Vertical");

        var continuousActionsOut = actionsOut.ContinuousActions;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        continuousActionsOut[0] = movement.x;
        continuousActionsOut[1] = movement.z;

        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");
        //Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        //actionsOut[0] = movement.x;
        //actionsOut[1] = movement.z;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_training)
        {
            if (other.CompareTag("target"))
            {
                AddReward(1f);
            }
            if (other.CompareTag("borders"))
            {
                AddReward(-0.1f);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (_training)
        {
            if (other.CompareTag("target"))
            {
                AddReward(0.5f);
            }
            if (other.CompareTag("borders"))
            {
                AddReward(-0.05f);
            }
        }
    }

    private void MoverPosicionInicial()
    {
        bool posicionEncontrada = false;
        int intentos = 100;
        Vector3 posicionPotencial = Vector3.zero;
        while (!posicionEncontrada || intentos >= 0)
        {
            intentos--;
            posicionPotencial = new Vector3(
                transform.parent.position.x + UnityEngine.Random.Range(-3f, 3f),
                0.555f,
                transform.parent.position.z + UnityEngine.Random.Range(-3f, 3f));
            //en el caso de que tengamos mas cosas en el escenario checker que no choca
            Collider[] colliders = Physics.OverlapSphere(posicionPotencial, 0.05f);
            if (colliders.Length == 0)
            {
                transform.position = posicionPotencial;
                posicionEncontrada = true;
            }
        }
    }

}

