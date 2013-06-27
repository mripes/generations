using UnityEngine;
using System.Collections;

public class FoodTile : MapTile {

  public int startingFood = 1;
  public float foodEnergy = 0.5F;
  public float replenishTime = 2;

  public int availableFood;

  public bool CanConsumeFood(Agent agent) {
    return availableFood > 0;
  }

  public bool ConsumeFood(Agent agent) {
    if (CanConsumeFood(agent)) {
      availableFood --;
      StatusDidChange();
      if (availableFood < 1) {
        OutOfFood();
      }
      return true;
    }
    return false;
  }

  bool respawning;

  public override void Start() {
    base.Start();
    respawning = false;
    availableFood = startingFood;
    StatusDidChange();
  }

  void StatusDidChange() {
    if (availableFood > 0) {
      SetColor(Color.green);
    }
    else {
      SetColor(Color.red);
    }
  }

  void OutOfFood() {
    if (!respawning) {
      respawning = true;
      Invoke("ReplenishFood", replenishTime);
    }
  }

  void ReplenishFood() {
    availableFood = startingFood;
    StatusDidChange();
    respawning = false;
  }

  Material recoloredMaterial;
  void SetColor(Color color) {
      if (recoloredMaterial == null) {
        recoloredMaterial = new Material(renderer.material);
        renderer.material = recoloredMaterial;
      }
    recoloredMaterial.color = color;
  }

}