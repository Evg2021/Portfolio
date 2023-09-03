using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnvironmentControllerSynchronizer : NetworkBehaviour
{
    public List<string> Test;

    private List<EnvironmentState> environmentStates = new List<EnvironmentState>();
    private EnvironmentControllerBase[] controllers;

    private void Awake()
    {
        controllers = FindObjectsOfType<EnvironmentControllerBase>();

        InitializeDoors();
        
        if (IsServer || IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        }
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        string[] names = new string[environmentStates.Count];
        bool[] states = new bool[environmentStates.Count];

        for (int i = 0; i < environmentStates.Count; i++)
        {
            names[i] = environmentStates[i].Name;
            states[i] = environmentStates[i].State;
        }

        SyncEnvironmentStatesClientRpc(obj, environmentStates.ToArray());
    }

    private void Start()
    {
        StartCoroutine(FindInteractorOwner());
    }

    private void InitializeDoors()
    {
        for (uint i = 0; i < controllers.Length; i++)
        {
            controllers[i].Initialize(i);
        }
    }

    private IEnumerator FindInteractorOwner()
    {
        Interactor ownerInteractor = null;
        while (!ownerInteractor)
        {
            var controller = FindObjectsOfType<PlayerController>().Where(h => h.IsOwner).FirstOrDefault();
            if (controller)
            {
                ownerInteractor = controller.GetComponent<Interactor>();
                if (ownerInteractor)
                {
                    ownerInteractor.OnInteractEnvironment += OwnerInteractor_OnInteractEnvironment;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void OwnerInteractor_OnInteractEnvironment(EnvironmentControllerBase controller)
    {
        if (IsClient)
        {
            var message = new EnvironmentState()
            {
                Name = controller.name,
                State = controller.CurrentValue
            };
            InteractServerRpc(controller.name, controller.CurrentValue, NetworkManager.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc(string name, bool state, ulong id)
    {
        //string name = Encoding.UTF8.GetString(bytes);
        var message = new EnvironmentState()
        {
            Name = name,
            State = state
        };

        var oldState = environmentStates.FirstOrDefault(h => h.Name == message.Name);
        if (oldState.Name == message.Name)
        {
            environmentStates.Remove(oldState);
        }

        environmentStates.Add(message);
        Test.Add(message.Name);

        InteractOnOtherClientRpc(name, id);
    }

    [ClientRpc]
    private void InteractOnOtherClientRpc(string name, ulong id)
    {
        if (NetworkManager.LocalClientId != id)
        {
            var controllerObject = GameObject.Find(name);
            if (controllerObject && controllerObject.TryGetComponent<EnvironmentControllerBase>(out var controller))
            {
                controller.Interact();
            }
        }
    }

    [ClientRpc]
    private void SyncEnvironmentStatesClientRpc(ulong clientId, EnvironmentState[] states)
    {
        if (clientId == NetworkManager.LocalClientId && states.Length > 0)
        {
            Test = new List<string>();
            for (int i = 0; i < name.Length && i < states.Length; i++)
            {
                Test.Add(states[i].Name);

                var controllerObject = GameObject.Find(states[i].Name);
                if (controllerObject && controllerObject.TryGetComponent<EnvironmentControllerBase>(out var controller))
                {
                    controller.SetState(states[i].State);
                }
            }
        }
    }

    [Serializable]
    public struct EnvironmentState : INetworkSerializable
    {
        public string Name;
        public bool State;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref State);
        }
    }
}
