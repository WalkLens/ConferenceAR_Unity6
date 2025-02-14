using MRTK.Tutorials.MultiUserCapabilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectTest : MonoBehaviour
{
    public string input_PIN;
    public void ConnectTester(string input)
    {
        PhotonLobbyConferenceAR.Lobby.JoinOrCreateRoom(input);
    }
}
