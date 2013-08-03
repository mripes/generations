using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ReproductiveSystem : Organ {

  public NumericalTrait fertility = new NumericalTrait(0F, 1F); 

  int timesReproduced = 0;

  // Reproduction

  static float reproductionCost = 0.1F;
  static float reproductionThreshold = 0.5F;

  static int _maxTotalChildren = 10;
  static int _maxLitterSize = 5;

  public bool CanReproduce() {
    if (!agent.manager.PopulationCeilingExceeded()
        //&& agent.hungerCenter.timesEaten > 1 
        && agent.energy > agent.body.MaxEnergy() * reproductionThreshold 
        && timesReproduced < _maxTotalChildren) {
      return true;
    }
    return false;
  }

  public void ReproduceIfPossible() {
    if (CanReproduce()) {
      // Find other agents
      Agent[] otherAgents = agent.currentTile.AgentsHereExcluding(agent);
      if (otherAgents.Length > 0) {
        Agent[] fertileAgents = SelectFertileAgents(otherAgents);
        if (fertileAgents.Length > 0) {
          ReproduceWith(fertileAgents[Random.Range(0, fertileAgents.Length)]);          
        }
      }
    }
  }

  public Agent[] SelectFertileAgents(Agent[] agents) {
    ArrayList fertile = new ArrayList();
    foreach (Agent a in agents) {
        if (a.reproductiveSystem.CanReproduce())
            fertile.Add(a);
    }
    return fertile.ToArray( typeof( Agent ) ) as Agent[];
  }

  public Agent[] parents;
  public int generation = 0;

  public Agent[] ReproduceWith(Agent otherParent) {

    /*if (Random.value <= fertility.floatValue 
        || Random.value <= otherParent.reproductiveSystem.fertility.floatValue) {
      return new Agent[0];
    }
    */
    int kidsToHave = Mathf.CeilToInt(
        Random.Range(_maxLitterSize * fertility.floatValue,
                     _maxLitterSize * otherParent.reproductiveSystem.fertility.floatValue)
        );

    Agent[] children = new Agent[kidsToHave];

    for (int i=0; i < kidsToHave; i++) {
      if (CanReproduce() && otherParent.reproductiveSystem.CanReproduce()) {
        Agent child = agent.manager.BirthAgent();

        // agent.energy -= reproductionCost;
        timesReproduced++;
        // otherParent.energy -= reproductionCost;
        otherParent.reproductiveSystem.timesReproduced++;

        child.currentTile = agent.currentTile;

        Agent[] theParents = new Agent[2];
        theParents[0] = agent;
        theParents[1] = otherParent;
        child.CreateFromParents(theParents);

        int highestParentGeneration = theParents[0].reproductiveSystem.generation;
        if (theParents[1].reproductiveSystem.generation > highestParentGeneration)
          highestParentGeneration = theParents[1].reproductiveSystem.generation;

        child.reproductiveSystem.parents = theParents;
        child.reproductiveSystem.generation = highestParentGeneration + 1;

        agent.TriggerLifeEvent("Reproduced");
        agent.Notify(AgentNotificationType.Sex);
        otherParent.Notify(AgentNotificationType.Sex);

        child.Notify(AgentNotificationType.Birth);


        children[i] = child;
      }
    }

    return children;
  }

}
