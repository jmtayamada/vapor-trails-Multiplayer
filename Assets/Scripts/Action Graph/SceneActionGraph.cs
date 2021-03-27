using UnityEngine;
using XNode;
using System.Linq;
using System.Collections.Generic;

public class SceneActionGraph : SceneGraph<ActionGraph>, IStateUpdateListener {

    bool started = false;

    void Start() {
        started = true;
        Initialize();
    }

    public void OnStateUpdate() {
        Initialize();
    }

    void Initialize() {
        if (!started) return;

        foreach (ActionNode node in GetRootNodes()) {
            node.SetInput(Signal.positive);
        }
    }

    List<ActionNode> GetRootNodes() {
        Debug.Log(graph);
        foreach (Node n in graph.nodes) {
            Debug.Log(n);
        }
        return graph.nodes
            .ConvertAll<ActionNode>(x => (ActionNode) x)
            .Where(x => x!=null && x.IsRoot())
            .ToList();
    }
}
