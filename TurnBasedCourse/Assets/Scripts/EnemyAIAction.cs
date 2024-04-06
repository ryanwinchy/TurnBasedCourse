using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Simple class to hold enemy AI action data. Could use constructor, but public vars so can just setup when make new instance with the new { } syntax.
//NO MONOBEHAVIOUR, as we want normal class we can setup with 'new' keyword. Eg new EnemyAIAction {gridPosition = (2,2), aiScore = 10,};
public class EnemyAIAction
{

    public GridPosition gridPosition;
    public int aiScore;                   //Value the specified action has, prioirty ai will do it. Will rank them and do best.
}
