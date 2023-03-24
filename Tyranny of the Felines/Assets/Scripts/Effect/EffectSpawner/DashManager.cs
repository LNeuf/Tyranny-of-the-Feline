using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashManager : MonoBehaviour
{
    private const int NumTimeSegments = 50;
    public void StartDash(DashInfo dashInfo)
    {
        StartCoroutine("Dash", dashInfo);
    }

    private IEnumerator Dash(DashInfo dashInfo)
    {
        for (int i = 0; i < NumTimeSegments; i++)
        {
            dashInfo.entity.Dash(dashInfo.direction * dashInfo.dashEffect.dashSpeed / NumTimeSegments);
            yield return new WaitForSeconds(dashInfo.dashEffect.dashDuration / NumTimeSegments);
        }
    }
}
