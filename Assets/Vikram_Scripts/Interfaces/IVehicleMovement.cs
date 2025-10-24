using UnityEngine;
using UnityEngine.Splines;

public interface IVehicleMovement
{
    public void StartMovement(VehicleManager vm,SplineContainer splineContainer,MovementDirection direction);
}
