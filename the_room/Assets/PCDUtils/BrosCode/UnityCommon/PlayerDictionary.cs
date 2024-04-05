using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDictionary : SingletonMono<PlayerDictionary> {
    public Dictionary<string, Transform> playerDict = new Dictionary<string, Transform>();

    static public void RegisterPlayer(string playerName, Transform player) {
        Instance.playerDict[playerName] = player;
    }

    static public Transform GetPlayer(string playerName) {
        if (!Instance.playerDict.ContainsKey(playerName))
            throw new System.Exception($"Must register player \"{playerName}\" before GetPlayer()");
        return Instance.playerDict[playerName];
    }

    static public List<string> GetAllPlayers() {
        return Instance.playerDict.Keys.ToList();
    }

    static public void SetAsLocalPlayer(string playerName) {
        PlayerIdentifier.Instance.SetPlayer(GetPlayer(playerName));
    }
}