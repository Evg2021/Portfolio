using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ScenarioHoldersList : MonoBehaviour
{
    public ScenarioHolder ActiveScenarioHolder => EnableScenario(_playerData.SituationData);

    private PlayerData _playerData;

    [Inject]
    private void Construct(PlayerData playerData)
    {
        _playerData = playerData;
    }

    private ScenarioHolder EnableScenario(SituationData situationData)
    {
        ScenarioHolder returnScenarioHolder = null;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out ScenarioHolder scenario))
            {
                bool equals = scenario.SituationData == situationData;

                if (equals)
                {
                    returnScenarioHolder = scenario;
                    continue;
                }

                Destroy(scenario.gameObject);
            } 
        }
        
        return returnScenarioHolder;
    }
}