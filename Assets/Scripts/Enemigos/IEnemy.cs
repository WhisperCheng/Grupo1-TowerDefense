using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public void OnSearchingEnemy();
    public void OnAttack();
    public void OnAbandonAtacking();
    public void OnAssignDestination(Vector3 destination);
}
